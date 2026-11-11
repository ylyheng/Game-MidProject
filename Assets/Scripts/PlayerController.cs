using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 11f;
    [SerializeField] private float acceleration = 30f;   // how fast we reach target speed

    

    [Header("Lab wiring (drag your objects here)")]
    [SerializeField] private GameObject orbPrefab;      // TODO 5: used to spawn a bonus orb
    [SerializeField] private Transform cameraTransform; // drag Main Camera here

    [SerializeField] private GameManager gameManager;

    private Rigidbody rb;
    private float h;
    private float v;

    private bool isRunning;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Fallback if you forget to drag the camera in
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
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
        // (so looking up/down doesn't make you walk into the floor or fly)
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (right * h + forward * v).normalized;

        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        // Preserve current vertical velocity (gravity / falling / jumping)
        Vector3 vel = moveDir * currentSpeed;
        vel.y = rb.linearVelocity.y;
        

        // Hand the velocity to the physics engine — it resolves wall collisions for us
        rb.linearVelocity = vel;
    }


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit the wall: " + collision.gameObject.name);
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Orb"))
    //     {
    //         if (gameManager)
    //         {
    //             gameManager.Collect();
    //         }
    //         Orb orb = other.GetComponent<Orb>();

    //         if (orb != null)
    //         {
    //             orb.DestroyOrb();
    //         }
    //     }
    // }

}