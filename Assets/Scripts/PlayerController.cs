// ============================================================
// Title:       PlayerController
// Version:     1.6
// Written by:  Ashar
// Description: WASD movement, jump, sprint and the follow camera.
//              Hold right click + drag to spin the camera 360,
//              scroll to zoom. WASD moves relative to the camera.
//              The ball takes the profile's colour.
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
        public float orthographicSize = 10f; // current zoom (bigger = see more)

        [Header("Controls")]
        public float rotateSpeed = 4f;       // degrees per mouse movement while right click is held
        public float zoomStep = 1f;          // size change per scroll notch
        public float minZoom = 8f;           // most zoomed in
        public float maxZoom = 20f;          // most zoomed out
        public float startZoom = 10f;        // zoom when the game starts
    }

    // True while a menu (OverlayUI) is open: no movement, jumping or camera control
    public static bool uiEnabled;

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

        // Ball colour from the profile picked on the Main Menu (left as is when testing a scene directly)
        Renderer ball = GetComponent<Renderer>();
        if (ProfileManager.HasProfile && ball != null)
            ball.material.color = ProfileManager.Profile.BallColour;

        CameraSettings c = cameraSettings;
        c.orthographicSize = Mathf.Clamp(c.startZoom, c.minZoom, c.maxZoom);
    }

    void Update()
    {
        if (uiEnabled)
        {
            input = Vector3.zero;   // stop where we are
            sprintHeld = false;
            return;
        }

        // WASD / arrow keys: -1 to 1 on each axis
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Turn the input to match the camera, so W always moves "up" the screen
        input = Quaternion.Euler(0f, cameraSettings.yaw, 0f) * new Vector3(x, 0f, z).normalized;
        sprintHeld = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        CameraInput();
    }

    void CameraInput()
    {
        CameraSettings c = cameraSettings;

        // Right click + drag left/right spins the camera all the way round
        if (Input.GetMouseButton(1))
            c.yaw = Mathf.Repeat(c.yaw + Input.GetAxis("Mouse X") * c.rotateSpeed, 360f);

        // Scroll up zooms in, scroll down zooms out
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f)
            c.orthographicSize = Mathf.Clamp(c.orthographicSize - scroll * c.zoomStep, c.minZoom, c.maxZoom);
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
