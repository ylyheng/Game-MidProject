using UnityEngine;

/// <summary>
/// Plays a footstep sound each time FirstPersonCamera fires OnStep.
/// Attach to the Player (needs an AudioSource, Spatial Blend = 0).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FirstPersonCamera cameraRig;   // drag Main Camera here

    [Header("Clips")]
    [SerializeField] private AudioClip[] footstepClips;     // 3-5 variations

    [Header("Variation")]
    [SerializeField] private float volume = 0.6f;
    [SerializeField] private float pitchMin = 0.92f;
    [SerializeField] private float pitchMax = 1.08f;       // everything by default

    private AudioSource source;
    private int lastClipIndex = -1;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;   // 2D sound, it's the player's own steps

        if (cameraRig == null && Camera.main != null)
            cameraRig = Camera.main.GetComponent<FirstPersonCamera>();
    }

    void OnEnable()
    {
        if (cameraRig != null) cameraRig.OnStep += PlayStep; // register to OnStep Event
    }

    void OnDisable()
    {
        if (cameraRig != null) cameraRig.OnStep -= PlayStep;
    }

    private void PlayStep()
    {
        Debug.Log("playfoot step");
        if (footstepClips == null || footstepClips.Length == 0) return;

        // Pick a random clip, but never the same one twice in a row
        int index = Random.Range(0, footstepClips.Length);
        if (footstepClips.Length > 1 && index == lastClipIndex)
            index = (index + 1) % footstepClips.Length;
        lastClipIndex = index;

        source.pitch = Random.Range(pitchMin, pitchMax);
        source.PlayOneShot(footstepClips[index], volume);
    }

}