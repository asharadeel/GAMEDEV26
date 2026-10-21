using UnityEngine;

// keeps a sign or label turned towards the camera
public class FaceCamera : MonoBehaviour
{
    private Camera cam;

    void LateUpdate()
    {
        if (cam == null)
            cam = Camera.main;

        if (cam != null)
            transform.rotation = cam.transform.rotation;
    }
}
