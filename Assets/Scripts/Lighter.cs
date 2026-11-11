using UnityEngine;

/// <summary>
/// Makes a spot light follow where the camera looks, like a handheld light.
/// Attach to the Spot Light object (it should NOT be a child of the camera,
/// this script moves it every frame).
/// </summary>
[RequireComponent(typeof(Light))]
public class Lighter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;   // drag Main Camera here

    [Header("Position")]
    [Tooltip("Offset in camera space: x = right, y = up, z = forward. Slightly right/down feels like a held light.")]
    [SerializeField] private Vector3 localOffset = new Vector3(0.25f, -0.2f, 0.3f);

    [Header("Follow")]
    [Tooltip("Higher = snaps to the camera. Lower = light lags behind and sways. 0 = no smoothing (instant).")]
    [SerializeField] private float rotationSharpness = 12f;

    [Header("Toggle")]
    [SerializeField] private bool startOn = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.F;

    private Light spot;

    void Awake()
    {
        spot = GetComponent<Light>();
        spot.enabled = startOn;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            spot.enabled = !spot.enabled;
    }

    // LateUpdate runs after FirstPersonCamera has moved/rotated the camera this frame
    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Position: stick to the camera (with the offset in camera space)
        transform.position = cameraTransform.TransformPoint(localOffset);

        // Rotation: face where the camera faces
        if (rotationSharpness <= 0f)
        {
            transform.rotation = cameraTransform.rotation;
        }
        else
        {
            float t = 1f - Mathf.Exp(-rotationSharpness * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraTransform.rotation, t);
        }
    }
}