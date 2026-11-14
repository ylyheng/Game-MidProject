using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AutoReplayAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("The audio clip to play and auto-replay.")]
    public AudioClip soundTrack;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Tooltip("Delay (in seconds) between end of clip and replay.")]
    public float replayDelay = 0f;

    [Tooltip("Start playing automatically when scene starts.")]
    public bool playOnStart = true;

    private AudioSource audioSource;
    private bool isReplaying = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Configure the AudioSource
        audioSource.clip = soundTrack;
        audioSource.loop = false; // We control replay manually
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    void Start()
    {
        if (playOnStart && soundTrack != null)
        {
            Play();
        }
    }

    void Update()
    {
        // Detect when the clip has finished
        if (!isReplaying && audioSource.isPlaying && !audioSource.loop)
        {
            if (audioSource.time >= audioSource.clip.length - 0.05f)
            {
                if (replayDelay > 0f)
                    StartCoroutine(ReplayAfterDelay());
                else
                    Replay();
            }
        }
    }

    public void Play()
    {
        if (soundTrack == null)
        {
            Debug.LogWarning("[AutoReplayAudio] No soundTrack assigned!");
            return;
        }

        audioSource.clip = soundTrack;
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
        isReplaying = false;
    }

    private void Replay()
    {
        if (replayDelay > 0f)
        {
            StartCoroutine(ReplayAfterDelay());
            return;
        }

        audioSource.Stop();
        audioSource.time = 0f;
        audioSource.Play();
    }

    private System.Collections.IEnumerator ReplayAfterDelay()
    {
        isReplaying = true;
        audioSource.Stop();
        yield return new WaitForSeconds(replayDelay);
        audioSource.clip = soundTrack;
        audioSource.time = 0f;
        audioSource.Play();
        isReplaying = false;
    }

    // Optional: allow changing the clip at runtime
    public void SetClip(AudioClip newClip)
    {
        soundTrack = newClip;
        audioSource.clip = newClip;
    }
}