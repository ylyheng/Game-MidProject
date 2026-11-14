using UnityEngine;

/// <summary>
/// Hand anchor: POSITION follows the Player (smooth, no physics jitter),
/// ROTATION follows the Camera. Left/right turns can sway; up/down stays locked
/// so the held object never tilts toward or away from the view.
/// Attach to the empty hand object. It must NOT be a child of the camera or player.
/// </summary>
[DefaultExecutionOrder(100)]   // run after FirstPersonCamera
public class HandMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;   // drag Player here (movement)
    [SerializeField] private Transform cameraTransform;   // drag Main Camera here (rotation)

    [Header("Position")]
    [Tooltip("Same as Eye Offset on FirstPersonCamera, so the hand starts from eye height.")]
    [SerializeField] private Vector3 eyeOffset = new Vector3(0f, 1.7f, 0f);
    [Tooltip("Offset in camera space: x = right, y = up, z = forward.")]
    [SerializeField] private Vector3 localOffset = new Vector3(0.25f, -0.2f, 0.3f);

    [Header("Sway (0 = locked to the camera, no lag)")]
    [Tooltip("Left/right turn sway. Lower = more lag.")]
    [SerializeField] private float yawSharpness = 12f;
    [Tooltip("Up/down sway. Keep at 0 to stop the object tilting toward/away from you.")]
    [SerializeField] private float pitchSharpness = 0f;

    private float yaw;
    private float pitch;
    private bool initialized;

    private FirstPersonCamera cameraRig;

    void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform != null)
            cameraRig = cameraTransform.GetComponent<FirstPersonCamera>();
    }

    void LateUpdate()
    {
        if (cameraTransform == null || playerTransform == null) return;

        Quaternion camRot = cameraTransform.rotation;
        Vector3 camEuler = camRot.eulerAngles;

        if (!initialized)
        {
            yaw = camEuler.y;
            pitch = camEuler.x;
            initialized = true;
        }

        // Yaw (left/right) and pitch (up/down) are smoothed separately
        yaw = Smooth(yaw, camEuler.y, yawSharpness);
        pitch = Smooth(pitch, camEuler.x, pitchSharpness);

        // No roll, so the object never twists around the view axis
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Position: anchor on the player's eyes, offset along the camera's exact view
        // Use the camera's own eye height so the anchor sits exactly where the camera is
        // (includes the sit/stand height blend, so the hand follows when you sit)
        Vector3 eye = cameraRig != null ? cameraRig.CurrentEyeOffset : eyeOffset;
        transform.position = playerTransform.position + eye + camRot * localOffset;
    }

    private static float Smooth(float current, float target, float sharpness)
    {
        if (sharpness <= 0f) return target;
        float t = 1f - Mathf.Exp(-sharpness * Time.deltaTime);
        return Mathf.LerpAngle(current, target, t);
    }
}