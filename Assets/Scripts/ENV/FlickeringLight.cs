using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The spot light to flicker. Auto-assigned if left empty.")]
    [SerializeField] private Light targetLight;

    [Header("Base Intensity")]
    [Tooltip("The steady intensity the light tries to return to.")]
    [SerializeField] private float baseIntensity = 3f;

    [Header("Flicker")]
    [Tooltip("How much the intensity can deviate from base (as a fraction).")]
    [Range(0f, 1f)]
    [SerializeField] private float flickerAmount = 0.7f;

    [Tooltip("How fast the flicker moves. Higher = more frantic.")]
    [SerializeField] private float flickerSpeed = 8f;

    [Tooltip("Noise seed offset so multiple lights don't flicker in sync.")]
    [SerializeField] private float noiseOffset = 0f;

    [Header("Random Spikes")]
    [Tooltip("Chance per second of a sudden bright/dim spike.")]
    [Range(0f, 5f)]
    [SerializeField] private float spikeChance = 0.5f;

    [Tooltip("How long a spike lasts, in seconds.")]
    [SerializeField] private float spikeDuration = 0.08f;

    [Tooltip("How strong a spike is (multiplier on flicker amount).")]
    [SerializeField] private float spikeStrength = 1.5f;

    [Header("Optional: Flicker Range Too")]
    [SerializeField] private bool flickerRange = false;
    [SerializeField] private float baseRange = 10f;

    private float seed;
    private float spikeTimer = 0f;
    private float currentSpike = 0f;

    private void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        // Random offset so multiple lights are out of phase
        seed = noiseOffset + Random.value * 1000f;

        // Cache initial range so we can flicker around it
        if (targetLight != null && flickerRange)
            baseRange = targetLight.range;
    }

    private void Update()
    {
        if (targetLight == null) return;

        // Organic noise-based flicker (0..1)
        float noise = Mathf.PerlinNoise(seed, Time.time * flickerSpeed);

        // Center the noise around 0 instead of 0..1, so it goes dim AND bright
        float flicker = (noise - 0.5f) * 2f; // -1..1

        // --- Random spike handling ---
        if (spikeTimer > 0f)
        {
            spikeTimer -= Time.deltaTime;
        }
        else if (Random.value < spikeChance * Time.deltaTime)
        {
            // Start a new spike
            spikeTimer = spikeDuration;
            currentSpike = Random.Range(-1f, 1f) * spikeStrength;
        }

        float spikeContribution = 0f;
        if (spikeTimer > 0f)
            spikeContribution = currentSpike * (spikeTimer / Mathf.Max(0.0001f, spikeDuration));

        // Combine flicker + spike
        float total = flicker + spikeContribution;
        total = Mathf.Clamp(total, -1f, 1f);

        // Apply to intensity
        targetLight.intensity = Mathf.Max(0f, baseIntensity * (1f + total * flickerAmount));

        // Optional: flicker range too
        if (flickerRange)
        {
            targetLight.range = Mathf.Max(0.1f, baseRange * (1f + total * flickerAmount * 0.5f));
        }
    }
}