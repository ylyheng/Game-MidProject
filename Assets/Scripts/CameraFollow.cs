using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;      // drag the Player here
    [SerializeField] private Vector3 eyeOffset = new Vector3(0f, 1.7f, 0f);  // eye height above player pivot

    [Header("Mouse Look")]
    [SerializeField] private float sensitivity = 2f;       // degrees per mouse unit
    [SerializeField] private float minPitch = -85f;        // look down limit
    [SerializeField] private float maxPitch = 85f;         // look up limit
    [SerializeField] private bool invertY = false;
    [SerializeField] private bool rotatePlayerWithYaw = true;  // player body turns left/right with the camera

    [Header("Smoothing (0 = raw, instant)")]
    [SerializeField] private float lookSmoothing = 0f;     // try 10-25 for a softer feel

    [Header("Zoom (FOV)")]
    [SerializeField] private float defaultFov = 70f;
    [SerializeField] private float minFov = 30f;
    [SerializeField] private float maxFov = 90f;
    [SerializeField] private float zoomSpeed = 20f;
    [SerializeField] private float zoomSharpness = 8f;

    [Header("Head Bob (walking up/down)")]
    [SerializeField] private bool enableBob = true;
    [SerializeField] private float bobFrequency = 1.6f;   // steps per second at full speed
    [SerializeField] private float bobVertical = 0.06f;   // up/down amount (meters)
    [SerializeField] private float bobHorizontal = 0.03f; // slight side sway (meters)
    [SerializeField] private float bobFullSpeed = 12f;     // speed at which bob is at full strength
    [SerializeField] private float bobReturnSharpness = 10f; // how fast it settles when you stop

    /// <summary>Fired each time a footstep lands (bottom of the head bob).</summary>
    public event System.Action OnStep;

    private int lastStepIndex = -1;
    private Rigidbody targetRb;
    private float bobTimer;
    private Vector3 bobOffset;

    private float yaw;
    private float pitch;
    private float smoothYaw;
    private float smoothPitch;
    private float targetFov;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        targetFov = defaultFov;
        if (cam != null) cam.fieldOfView = defaultFov;

        // Start from the current facing
        Vector3 e = transform.eulerAngles;
        yaw = target != null ? target.eulerAngles.y : e.y;
        pitch = 0f;
        smoothYaw = yaw;
        smoothPitch = pitch;

        if (target != null) targetRb = target.GetComponent<Rigidbody>();

        LockCursor(true);
    }

    void Update()
    {
        // Esc frees the cursor, click locks it again
        if (Input.GetKeyDown(KeyCode.Escape)) LockCursor(false);
        if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0)) LockCursor(true);
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Mouse look (mouse delta is already per-frame, so no Time.deltaTime here)
        float mx = Input.GetAxis("Mouse X") * sensitivity;
        float my = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mx;
        pitch += invertY ? my : -my;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Scroll zoom changes field of view
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
            targetFov = Mathf.Clamp(targetFov - scroll * zoomSpeed * 10f, minFov, maxFov);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Optional smoothing
        if (lookSmoothing > 0f)
        {
            float t = 1f - Mathf.Exp(-lookSmoothing * Time.deltaTime);
            smoothYaw = Mathf.LerpAngle(smoothYaw, yaw, t);
            smoothPitch = Mathf.Lerp(smoothPitch, pitch, t);
        }
        else
        {
            smoothYaw = yaw;
            smoothPitch = pitch;
        }

        // Walking bob, driven by horizontal speed
        Vector3 desiredBob = Vector3.zero;
        if (enableBob && targetRb != null)
        {
            Vector3 flatVel = targetRb.linearVelocity;
            flatVel.y = 0f;
            float speedFactor = Mathf.Clamp01(flatVel.magnitude / bobFullSpeed);

            if (speedFactor > 0.01f)
            {
                bobTimer += Time.deltaTime * bobFrequency * Mathf.PI * 2f * speedFactor;

                // A step lands at the lowest point of the bob (sin = -1, at 3π/2 + 2πk)
                int stepIndex = Mathf.FloorToInt((bobTimer - 1.5f * Mathf.PI) / (2f * Mathf.PI));
                if (stepIndex > lastStepIndex)
                {
                    lastStepIndex = stepIndex;
                    OnStep?.Invoke();
                }
                desiredBob = new Vector3(
                    Mathf.Cos(bobTimer * 0.5f) * bobHorizontal,   // sway left/right every other step
                    Mathf.Sin(bobTimer) * bobVertical,            // up/down every step
                    0f) * speedFactor;
            }
        }
        // Ease toward the bob (and back to zero when you stop)
        bobOffset = Vector3.Lerp(bobOffset, desiredBob, bobReturnSharpness * Time.deltaTime);

        // Camera sits at the player's eyes, plus bob (rotated into camera yaw space)
        Quaternion yawRot = Quaternion.Euler(0f, smoothYaw, 0f);
        transform.position = target.position + eyeOffset + yawRot * bobOffset;
        transform.rotation = Quaternion.Euler(smoothPitch, smoothYaw, 0f);

        // Turn the player body left/right so WASD moves where you look
        if (rotatePlayerWithYaw)
            target.rotation = Quaternion.Euler(0f, smoothYaw, 0f);

        // Smooth FOV zoom
        if (cam != null)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, zoomSharpness * Time.deltaTime);
    }

    private void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}