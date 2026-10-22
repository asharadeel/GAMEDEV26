using UnityEngine;

// the portal piece the boss drops, it just spins and floats
// the show ui on approach prefab under it opens the pop up when you walk onto it
// walking onto it also saves that the player has the piece, so portal 2 in the hub can be opened
public class PortalPiece : MonoBehaviour
{
    public Transform model;
    public float spinSpeed = 90f;
    public float bobHeight = 0.3f;
    public float bobSpeed = 2f;
    public float pickupRadius = 2.5f;

    private Vector3 start;
    private Transform player;
    private bool collected;

    void Start()
    {
        start = model.localPosition;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        model.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        model.localPosition = start + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        if (collected || player == null)
            return;

        Vector3 a = player.position;
        Vector3 b = transform.position;
        a.y = 0f;
        b.y = 0f;
        if (Vector3.Distance(a, b) <= pickupRadius)
        {
            collected = true;
            model.gameObject.SetActive(false);
            ProfileManager.Profile.HasPortalPiece = true;
            ProfileManager.Save();
        }
    }
}
