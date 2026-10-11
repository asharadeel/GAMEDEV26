using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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

    public float speed = 5f;

    public CameraSettings cameraSettings = new CameraSettings();

    private Rigidbody rb;
    private Vector3 input;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // stop the ball tipping/spinning from collisions
    }

    void Update()
    {
        // WASD / arrow keys: -1 to 1 on each axis
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        input = new Vector3(x, 0f, z).normalized;
    }

    void FixedUpdate()
    {
        // Move through physics so walls block us. Keep y so gravity still works.
        rb.velocity = new Vector3(input.x * speed, rb.velocity.y, input.z * speed);
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
