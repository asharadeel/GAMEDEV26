// ============================================================
// Title:       PlayerController
// Version:     1.7
// Written by:  Ashar
// Updated by:  cthullu96 (v1.7: input system package)
//              ai use: input system change written with claude (anthropic), owner cthullu96
// Description: WASD movement, jump, sprint and the follow camera.
//              Hold right click + drag to spin the camera 360,
//              scroll to zoom. WASD moves relative to the camera.
//              The ball takes the profile's colour.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class CameraSettings
    {
        public Camera camera;

        //angle
        public float pitch = 50f;    //90 = straight top-down
        public float yaw = 25f;
        public float roll = 0f;
        public float distance = 50f;
        public float orthographicSize = 10f; //bigger = see more

        [Header("Controls")]
        public float rotateSpeed = 4f;
        public float zoomStep = 1f;
        public float minZoom = 8f;
        public float maxZoom = 20f;
        public float startZoom = 10f;
    }

    //true while a menu (OverlayUI) is open
    public static bool uiEnabled;

    [Header("Movement")]
    public float sprintMultiplier = 1.6f;
    public float jumpForce = 5f;

    public CameraSettings cameraSettings = new CameraSettings();

    //components
    private Rigidbody rb;
    private Collider col;
    private PlayerStats stats;
    private Vector3 input;
    private bool sprintHeld;

    //STARTING PROCEDURES
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        stats = GetComponent<PlayerStats>();
        rb.freezeRotation = true; //stop the ball spinning from collisions

        //ball colour (left as is when testing a scene directly)
        Renderer ball = GetComponent<Renderer>();
        if (ProfileManager.HasProfile && ball != null)
            ball.material.color = ProfileManager.Profile.BallColour;

        CameraSettings c = cameraSettings;
        c.orthographicSize = Mathf.Clamp(c.startZoom, c.minZoom, c.maxZoom);
    }

    //LISTENERS
    void Update()
    {
        if (uiEnabled)
        {
            input = Vector3.zero;
            sprintHeld = false;
            return;
        }

        Vector2 move = GameInput.Move.ReadValue<Vector2>();
        float x = move.x;
        float z = move.y;

        //match the camera so W always moves up the screen
        input = Quaternion.Euler(0f, cameraSettings.yaw, 0f) * new Vector3(x, 0f, z).normalized;
        sprintHeld = GameInput.Sprint.IsPressed();

        if (GameInput.Jump.WasPressedThisFrame() && IsGrounded())
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        CameraInput();
    }

    //CAMERA
    void CameraInput()
    {
        CameraSettings c = cameraSettings;

        //0.1 matches the old mouse x axis speed
        if (GameInput.RotateCamera.IsPressed())
            c.yaw = Mathf.Repeat(c.yaw + GameInput.Look.ReadValue<Vector2>().x * 0.1f * c.rotateSpeed, 360f);

        //one step per notch, raw scroll is much bigger than 1 on some mice
        float scroll = GameInput.Zoom.ReadValue<Vector2>().y;
        if (scroll != 0f)
            scroll = Mathf.Sign(scroll);
        if (scroll != 0f)
            c.orthographicSize = Mathf.Clamp(c.orthographicSize - scroll * c.zoomStep, c.minZoom, c.maxZoom);
    }

    //MOVEMENT
    void FixedUpdate()
    {
        float speed = stats.Profile.Speed;

        //sprint only while moving with stamina left
        stats.IsSprinting = sprintHeld && input != Vector3.zero && stats.Profile.Stamina > 0f;
        if (stats.IsSprinting)
            speed *= sprintMultiplier;

        rb.velocity = new Vector3(input.x * speed, rb.velocity.y, input.z * speed); //keep y for gravity
    }

    bool IsGrounded()
    {
        float distance = col.bounds.extents.y + 0.1f;
        return Physics.Raycast(transform.position, Vector3.down, distance, ~0, QueryTriggerInteraction.Ignore);
    }

    //CAMERA FOLLOW
    void LateUpdate()
    {
        ApplyCamera();
    }

    //updates the camera in the editor without pressing Play
    void OnValidate()
    {
        ApplyCamera();
    }

    void ApplyCamera()
    {
        Camera cam = cameraSettings.camera;
        if (cam == null)
            return;

        Quaternion rotation = Quaternion.Euler(cameraSettings.pitch, cameraSettings.yaw, cameraSettings.roll);
        cam.transform.rotation = rotation;
        cam.transform.position = transform.position - rotation * Vector3.forward * cameraSettings.distance;

        cam.orthographic = true;
        cam.orthographicSize = cameraSettings.orthographicSize;
    }
}
