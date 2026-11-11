using UnityEngine;
using TMPro;

/// <summary>
/// Finds the Interactable closest to where the camera is looking (within a cone),
/// shows a prompt for it, and calls Interact() when E is pressed.
/// Attach to the Player (or the camera).
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;   // drag Main Camera here
    [SerializeField] private TMP_Text promptText;         // UI label
    [SerializeField] private GameObject promptRoot;       // optional: parent object to show/hide

    [Header("Settings")]
    [SerializeField] private float interactRange = 4f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask interactMask = ~0;   // layers to search for interactables

    [Header("Aim Assist")]
    [Tooltip("Max angle (degrees) away from the screen center. 30 = forgiving, 45 = very forgiving.")]
    [SerializeField, Range(1f, 90f)] private float maxAngle = 30f;
    [Tooltip("Blocked by walls? Turn off to interact through walls.")]
    [SerializeField] private bool requireLineOfSight = true;

    [Header("Debug")]
    [SerializeField] private bool debugRay = false;

    private Interactable current;
    private readonly Collider[] hits = new Collider[32];

    void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        SetPrompt(null);
    }

    void Update()
    {
        FindTarget();

        if (current != null && Input.GetKeyDown(interactKey))
        {
            current.Interact(this);
            current = null;      // object may be destroyed by Interact()
            SetPrompt(null);
        }
    }

    private void FindTarget()
    {
        Vector3 origin = cameraTransform.position;
        Vector3 forward = cameraTransform.forward;

        Interactable best = null;
        float bestAngle = maxAngle;

        int count = Physics.OverlapSphereNonAlloc(origin, interactRange, hits, interactMask,
                                                  QueryTriggerInteraction.Collide);
        for (int i = 0; i < count; i++)
        {
            Interactable candidate = hits[i].GetComponentInParent<Interactable>();
            if (candidate == null) continue;

            // Aim at the closest point of the collider, so big objects are easy to target
            Vector3 point = hits[i].ClosestPoint(origin);
            Vector3 toTarget = point - origin;
            if (toTarget.sqrMagnitude < 0.0001f) toTarget = hits[i].bounds.center - origin;

            float angle = Vector3.Angle(forward, toTarget);
            if (angle > bestAngle) continue;

            if (requireLineOfSight && !HasLineOfSight(origin, hits[i].bounds.center, candidate))
                continue;

            best = candidate;
            bestAngle = angle;
        }

        if (debugRay) Debug.DrawRay(origin, forward * interactRange, Color.red);

        if (best != current)
        {
            current = best;
            SetPrompt(current != null ? "[" + interactKey + "] " + current.Prompt : null);
        }
    }

    // True if the first thing between the camera and the target is the target itself
    private bool HasLineOfSight(Vector3 origin, Vector3 targetPoint, Interactable target)
    {
        Vector3 dir = targetPoint - origin;
        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dir.magnitude + 0.1f,
                            ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.GetComponentInParent<Interactable>() == target;
        }
        return true;   // hit nothing (e.g. target is only a trigger collider) = clear
    }

    private void SetPrompt(string text)
    {
        bool show = !string.IsNullOrEmpty(text);

        if (promptText != null)
        {
            promptText.text = show ? text : "";
            promptText.gameObject.SetActive(show);
        }
        if (promptRoot != null) promptRoot.SetActive(show);
    }
}