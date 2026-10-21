using UnityEngine;
using UnityEngine.Rendering;

// put this on the parent of all the cave wall pieces
// the walls on the camera side would block the view of the player
// so this hides them but they still cast shadows and still have colliders
public class WallHider : MonoBehaviour
{
    public float hideAngle = 60f; // walls this close to the camera side get hidden

    private Camera cam;
    private Transform[] pieces;
    private Renderer[][] renderers;

    void Start()
    {
        cam = Camera.main;

        // remember every wall piece and its renderers
        pieces = new Transform[transform.childCount];
        renderers = new Renderer[transform.childCount][];
        for (int i = 0; i < transform.childCount; i++)
        {
            pieces[i] = transform.GetChild(i);
            renderers[i] = pieces[i].GetComponentsInChildren<Renderer>();
        }
    }

    void LateUpdate()
    {
        if (cam == null)
            return;

        // flat direction pointing towards the camera
        Vector3 toCamera = -cam.transform.forward;
        toCamera.y = 0f;

        for (int i = 0; i < pieces.Length; i++)
        {
            Vector3 dir = pieces[i].position - transform.position;
            dir.y = 0f;

            bool hide = Vector3.Angle(dir, toCamera) < hideAngle;

            foreach (Renderer r in renderers[i])
            {
                if (hide)
                    r.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                else
                    r.shadowCastingMode = ShadowCastingMode.On;
            }
        }
    }
}
