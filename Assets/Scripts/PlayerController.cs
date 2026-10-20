// ============================================================
// Title:       PlayerController
// Version:     1.2
// Written by:  Ashar
// Description: WASD movement, jump, sprint and the follow camera.
// ============================================================

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class CameraSettings
    {
        public Camera camera;

        public float pitch = 50f;    // tilt down (90 = straight top-down)
        public float yaw = 25f;      // turn left/right
        public float roll = 0f;
        public float distance = 50f; // how far back from the player
        public float orthographicSize = 8f; // zoom
    }

    [Header("Movement")]
    public float sprintMultiplier = 1.6f;
    public float jumpForce = 5f;

    public CameraSettings cameraSettings = new CameraSettings();

    private Rigidbody rb;
    private Collider col;
    private PlayerStats stats;
    private Vector3 input;
    private bool sprintHeld;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        stats = GetComponent<PlayerStats>();
        rb.freezeRotation = true; // stop the ball tipping/spinning from collisions
    }

    void Update()
    {
        // WASD / arrow keys: -1 to 1 on each axis
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        input = new Vector3(x, 0f, z).normalized;
        sprintHeld = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    void FixedUpdate()
    {
        // Move through physics so walls block us. Keep y so gravity still works.
        float speed = stats.Profile.Speed; // speed lives in the saved profile

        // Sprint only while moving and there's stamina left (PlayerStats drains it)
        stats.IsSprinting = sprintHeld && input != Vector3.zero && stats.Profile.Stamina > 0f;
        if (stats.IsSprinting)
            speed *= sprintMultiplier;

        rb.velocity = new Vector3(input.x * speed, rb.velocity.y, input.z * speed);
    }

    bool IsGrounded()
    {
        // Short ray down from the centre, just past the bottom of the collider
        float distance = col.bounds.extents.y + 0.1f;
        return Physics.Raycast(transform.position, Vector3.down, distance, ~0, QueryTriggerInteraction.Ignore);
    }

    void LateUpdate()
    {
        ApplyCamera();
    }

    // Runs in the editor whenever a value changes, so the camera updates without pressing Play
    void OnValidate()
    {
        ApplyCamera();
    }

    void ApplyCamera()
    {
        Camera cam = cameraSettings.camera;
        if (cam == null)
            return;

        // Point the camera at the player from the set angle and distance
        Quaternion rotation = Quaternion.Euler(cameraSettings.pitch, cameraSettings.yaw, cameraSettings.roll);
        cam.transform.rotation = rotation;
        cam.transform.position = transform.position - rotation * Vector3.forward * cameraSettings.distance;

        cam.orthographic = true;
        cam.orthographicSize = cameraSettings.orthographicSize;
    }
}
