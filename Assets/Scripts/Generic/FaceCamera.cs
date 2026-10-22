using UnityEngine;

// keeps a sign or label turned towards the camera
// with Face Player Within set, while the player is that close it points straight at the camera itself
// (the camera follows the player, so the sign turns as they walk around it and always reads square-on),
// and turns back when they walk away
public class FaceCamera : MonoBehaviour
{
    public float facePlayerWithin = 0f;   // 0 = always face the camera's angle
    public float turnSpeed = 6f;          // how quickly it turns between the two

    private Camera cam;
    private Transform player;

    void LateUpdate()
    {
        if (cam == null)
            cam = Camera.main;
        if (cam == null)
            return;

        Quaternion target = cam.transform.rotation;

        if (facePlayerWithin > 0f)
        {
            if (player == null)
            {
                GameObject p = GameObject.FindWithTag("Player");
                if (p != null)
                    player = p.transform;
            }

            if (player != null)
            {
                Vector3 flat = transform.position - player.position;
                flat.y = 0f;
                Vector3 fromCamera = transform.position - cam.transform.position;
                if (flat.magnitude <= facePlayerWithin && fromCamera.sqrMagnitude > 0.01f)
                    target = Quaternion.LookRotation(fromCamera, Vector3.up);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
            return;
        }

        transform.rotation = target;
    }
}
