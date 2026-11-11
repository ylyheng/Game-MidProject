using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 11f;
    [SerializeField] private float acceleration = 30f;   // how fast we reach target speed
    [SerializeField] private float sitSpeedMultiplier = 0.4f;   // speed scale while sitting (0.4 = 40%)

    [Header("Lab wiring (drag your objects here)")]
    [SerializeField] private GameObject orbPrefab;      // TODO 5: used to spawn a bonus orb
    [SerializeField] private Transform cameraTransform; // drag Main Camera here
    [SerializeField] private FirstPersonCamera fpCamera; // optional — auto-found from cameraTransform

    [SerializeField] private GameManager gameManager;

    private Rigidbody rb;
    private float h;
    private float v;

    private bool isRunning;
    private bool isSitting;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Fallback if you forget to drag the camera in
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Fallback for the sit source
        if (fpCamera == null && cameraTransform != null)
            fpCamera = cameraTransform.GetComponent<FirstPersonCamera>();
    }

    void OnEnable()
    {
        if (fpCamera != null) fpCamera.OnSitChanged += HandleSitChanged;
    }

    void OnDisable()
    {
        if (fpCamera != null) fpCamera.OnSitChanged -= HandleSitChanged;
    }

    private void HandleSitChanged(bool sitting)
    {
        isSitting = sitting;
    }

    void Update()
    {
        // Poll input in Update so key presses are never missed
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    void FixedUpdate()
    {
        // Camera-relative directions, flattened onto the ground plane
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (right * h + forward * v).normalized;

        // Running is disabled while sitting; otherwise pick walk/run and scale by sit
        bool canRun = isRunning && !isSitting;
        float baseSpeed = canRun ? runSpeed : walkSpeed;
        float targetSpeed = baseSpeed * (isSitting ? sitSpeedMultiplier : 1f);

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        // Preserve current vertical velocity (gravity / falling / jumping)
        Vector3 vel = moveDir * currentSpeed;
        vel.y = rb.linearVelocity.y;

        rb.linearVelocity = vel;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit the wall: " + collision.gameObject.name);
    }
}