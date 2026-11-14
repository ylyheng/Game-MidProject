using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AutoReplayAudio : MonoBehaviour
{
    // ------------------------------------------------------------------
    // One entry in the sound table. Add as many as you like in the Inspector.
    // ------------------------------------------------------------------
    [System.Serializable]
    public class NamedAudio
    {
        [Tooltip("Key used by code, e.g. \"idle\", \"chase\", \"kill\".")]
        public string name = "idle";

        public AudioClip clip;

        [Range(0f, 2f)]
        public float volume = 1f;

        [Tooltip("If true, Unity loops forever (no gap). If false, replayDelay is used.")]
        public bool loop = true;

        [Tooltip("Only used when loop = false. Seconds between end of clip and replay.")]
        public float replayDelay = 0f;
    }

    [Header("Sound Table (drag clips here)")]
    public List<NamedAudio> sounds = new List<NamedAudio>();

    [Header("Playback")]
    [Tooltip("Name of the sound to start with (must match a name above).")]
    public string defaultSoundName = "idle";

    [Tooltip("Crossfade time when switching sounds. 0 = instant cut.")]
    public float fadeDuration = 0.2f;

    [Tooltip("Forces 2D sound — same loudness everywhere. Uncheck to use 3D falloff.")]
    public bool force2D = true;

    private AudioSource audioSource;
    private NamedAudio current;
    private bool isReplaying;
    private Coroutine fadeRoutine;

    // ------------------------------------------------------------------
    // Unity lifecycle
    // ------------------------------------------------------------------
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = force2D ? 0f : 1f;
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(defaultSoundName))
            Play(defaultSoundName);
    }

    void Update()
    {
        // Manual replay only matters for non-looping clips.
        if (current == null || current.loop) return;
        if (isReplaying || fadeRoutine != null) return;
        if (!audioSource.isPlaying || audioSource.clip == null) return;

        if (audioSource.time >= audioSource.clip.length - 0.05f)
        {
            if (current.replayDelay > 0f)
                StartCoroutine(ReplayAfterDelay(current.replayDelay));
            else
            {
                audioSource.Stop();
                audioSource.time = 0f;
                audioSource.Play();
            }
        }
    }

    // ------------------------------------------------------------------
    // Public API — call from any script
    // ------------------------------------------------------------------

    /// <summary>Switch to a named looping sound (with optional crossfade).</summary>
    public void Play(string name)
    {
        NamedAudio entry = Find(name);
        if (entry == null || entry.clip == null)
        {
            Debug.LogWarning($"[AutoReplayAudio] No sound named '{name}' on {name}.");
            return;
        }

        // Already playing this exact one — don't restart it.
        if (current == entry && audioSource.isPlaying && fadeRoutine == null)
            return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        if (fadeDuration > 0f && audioSource.isPlaying)
            fadeRoutine = StartCoroutine(FadeTo(entry));
        else
            SwitchTo(entry);
    }

    /// <summary>Fire-and-forget — does not change the active looping sound.</summary>
    public void PlayOneShot(string name)
    {
        NamedAudio entry = Find(name);
        if (entry == null || entry.clip == null) return;
        audioSource.PlayOneShot(entry.clip, entry.volume);
    }

    /// <summary>Stop everything.</summary>
    public void Stop()
    {
        if (fadeRoutine != null) { StopCoroutine(fadeRoutine); fadeRoutine = null; }
        audioSource.Stop();
        current = null;
    }

    /// <summary>Change a sound's volume at runtime.</summary>
    public void SetVolume(string name, float volume)
    {
        NamedAudio entry = Find(name);
        if (entry != null) entry.volume = volume;
    }

    public bool Has(string name) => Find(name) != null;

    // ------------------------------------------------------------------
    // Internals
    // ------------------------------------------------------------------
    private void SwitchTo(NamedAudio entry)
    {
        current = entry;
        isReplaying = false;
        audioSource.clip = entry.clip;
        audioSource.volume = entry.volume;
        audioSource.loop = entry.loop;
        audioSource.time = 0f;
        audioSource.Play();
    }

    private IEnumerator FadeTo(NamedAudio entry)
    {
        float startVol = audioSource.volume;
        float t = 0f;

        // fade out current
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }

        // swap
        current = entry;
        isReplaying = false;
        audioSource.Stop();
        audioSource.clip = entry.clip;
        audioSource.loop = entry.loop;
        audioSource.volume = 0f;
        audioSource.time = 0f;
        audioSource.Play();

        // fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, entry.volume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = entry.volume;
        fadeRoutine = null;
    }

    private IEnumerator ReplayAfterDelay(float delay)
    {
        isReplaying = true;
        audioSource.Stop();
        yield return new WaitForSeconds(delay);
        if (current != null)
        {
            audioSource.clip = current.clip;
            audioSource.time = 0f;
            audioSource.Play();
        }
        isReplaying = false;
    }

    private NamedAudio Find(string name)
    {
        for (int i = 0; i < sounds.Count; i++)
            if (sounds[i].name == name) return sounds[i];
        return null;
    }
}