using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// "Weeping angel" doll: freezes while the player is looking at it,
/// chases the player while unseen, and kills on catch.
/// Attach to the doll. Works best with a NavMeshAgent (baked NavMesh);
/// without one it walks in a straight line.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DollChaser : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;          // drag Player here
    [SerializeField] private Camera playerCamera;       // auto-finds Main Camera if empty

    [Header("Chase")]
    [Tooltip("Off = the doll just stands still. Turned on by SetChasing / SetAllChasing.")]
    [SerializeField] private bool chasingEnabled = false;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float killDistance = 1.2f;

    [Header("Being Seen")]
    [Tooltip("Farther than this and the doll counts as not seen (e.g. too dark/far).")]
    [SerializeField] private float maxSeeDistance = 40f;
    [Tooltip("Layers that can block the player's view of the doll (walls, doors...).")]
    [SerializeField] private LayerMask viewBlockers = ~0;

    [Header("On Kill")]
    [SerializeField] private UnityEvent onKillPlayer;
    [SerializeField] private bool reloadSceneOnKill = true;
    [SerializeField] private float reloadDelay = 1.5f;

    [Header("Audio (drag clips here)")]
    [Tooltip("Looping clip played while the doll is moving. Leave empty to disable.")]
    public AudioClip moveClip;

    [Tooltip("Clip played once when the doll catches the player. Leave empty to disable.")]
    public AudioClip killClip;

    [Range(0f, 1f)] public float moveVolume = 0.6f ;
    [Range(0f, 1f)] public float killVolume = 1f;

    [Header("Debug")]
    [SerializeField] private bool logStateChanges = false;

    [SerializeField] private NavMeshAgent agent;
    private Animator animator;
    private Renderer[] renderers;
    private AudioSource audioSource;
    private readonly Plane[] frustum = new Plane[6];

    private bool killed;
    private bool wasSeen;

    // ---------------------------------------------------------------
    // Registry
    // ---------------------------------------------------------------
    private static readonly List<DollChaser> all = new List<DollChaser>();
    public static IReadOnlyList<DollChaser> All => all;

    public static void SetAllChasing(bool enable)
    {
        for (int i = 0; i < all.Count; i++)
            all[i].SetChasing(enable);
    }

    public bool ChasingEnabled => chasingEnabled;

    public void SetChasing(bool enable)
    {
        if (chasingEnabled == enable) return;
        chasingEnabled = enable;
        if (logStateChanges) Debug.Log("[Doll] " + name + " chasing = " + enable);
    }

    void OnEnable()  { if (!all.Contains(this)) all.Add(this); }
    void OnDisable()
    {
        all.Remove(this);
        StopMoveSound();
    }

    void Awake()
    {

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("[Doll] No GameObject with tag 'Player' found!");
        }

        if (playerCamera == null)
            playerCamera = Camera.main;

        animator = GetComponentInChildren<Animator>();
        renderers = GetComponentsInChildren<Renderer>();

        // Same pattern as AutoReplayAudio: grab the AudioSource we require.
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;            // we drive the move loop ourselves
        audioSource.spatialBlend = 1f;      // 3D
        audioSource.clip = moveClip;
        audioSource.volume = moveVolume;

        if (agent != null) agent.speed = moveSpeed;
        if (playerCamera == null) playerCamera = Camera.main;
    }

    void Update()
    {
        if (killed || player == null || playerCamera == null) return;

        if (!chasingEnabled)
        {
            SetFrozen(true);
            return;
        }

        bool seen = IsSeenByPlayer();

        if (logStateChanges && seen != wasSeen)
            Debug.Log("[Doll] " + (seen ? "frozen (seen)" : "moving (unseen)"));
        wasSeen = seen;

        SetFrozen(seen);

        if (seen) return;

        Chase();

        Vector3 a = transform.position; a.y = 0f;
        Vector3 b = player.position;    b.y = 0f;
        if (Vector3.Distance(a, b) <= killDistance)
            KillPlayer();
        
        // play chassing sound
    }

    // ---------------------------------------------------------------
    // Seeing
    // ---------------------------------------------------------------
    private bool IsSeenByPlayer()
    {
        Bounds bounds = GetBounds();

        if (Vector3.Distance(playerCamera.transform.position, bounds.center) > maxSeeDistance)
            return false;

        GeometryUtility.CalculateFrustumPlanes(playerCamera, frustum);
        if (!GeometryUtility.TestPlanesAABB(frustum, bounds))
            return false;

        Vector3 up = Vector3.up * bounds.extents.y * 0.8f;
        return HasLineOfSight(bounds.center)
            || HasLineOfSight(bounds.center + up)
            || HasLineOfSight(bounds.center - up);
    }

    private bool HasLineOfSight(Vector3 point)
    {
        Vector3 camPos = playerCamera.transform.position;

        if (Physics.Linecast(camPos, point, out RaycastHit hit, viewBlockers,
                             QueryTriggerInteraction.Ignore))
        {
            return hit.collider.GetComponentInParent<DollChaser>() == this;
        }
        return true;
    }

    private Bounds GetBounds()
    {
        if (renderers == null || renderers.Length == 0)
            return new Bounds(transform.position + Vector3.up, Vector3.one);

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
        return b;
    }

    // ---------------------------------------------------------------
    // Freeze / chase
    // ---------------------------------------------------------------
    private void SetFrozen(bool frozen)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = frozen;
            if (frozen) agent.velocity = Vector3.zero;
        }
        if (animator != null) animator.speed = frozen ? 0f : 1f;

        UpdateMoveSound(!frozen);
    }


    private void Chase()
    {
        agent.isStopped = false;

        NavMeshPath path = new NavMeshPath();

        bool found = agent.CalculatePath(player.position, path);

        if (!found || path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.LogWarning(
                "[Doll] Cannot find complete path! Status: " + path.status
            );
            return;
        }
        else
        {
            Debug.Log("Found path");
        }

        agent.SetPath(path);
    }

    // ---------------------------------------------------------------
    // Audio (same style as AutoReplayAudio)
    // ---------------------------------------------------------------
    private void UpdateMoveSound(bool shouldPlay)
    {
        if (audioSource == null || moveClip == null) return;

        // Keep the source pointed at moveClip in case it changed in the Inspector.
        if (audioSource.clip != moveClip)
            audioSource.clip = moveClip;

        audioSource.volume = moveVolume + 0.5f;

        if (shouldPlay)
        {
            if (!audioSource.isPlaying)
            {
                if (audioSource.time > 0f) audioSource.UnPause();
                else                       audioSource.Play();
            }
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    private void StopMoveSound()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    // ---------------------------------------------------------------
    // Kill
    // ---------------------------------------------------------------
    private void KillPlayer()
    {
        killed = true;
        Debug.Log("[Doll] Caught the player!");

        StopMoveSound();

        // PlayOneShot plays over the (now stopped) loop, on the same source.
        if (audioSource != null && killClip != null)
            audioSource.PlayOneShot(killClip, killVolume + 0.5f);

        if (agent != null && agent.isOnNavMesh) agent.isStopped = true;

        onKillPlayer?.Invoke();

        if (reloadSceneOnKill)
            Invoke(nameof(ReloadScene), reloadDelay);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}