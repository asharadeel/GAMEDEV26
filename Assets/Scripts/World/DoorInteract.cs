// ============================================================
// Title:       DoorInteract
// Version:     1.0
// Written by:  Ashar
// Description: A door the player can use when close. Shows a ring
//              on the floor for the radius and an "E / Click"
//              prompt on screen. Press E or click the prompt to be
//              moved to the destination.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

public class DoorInteract : MonoBehaviour
{
    [Header("Where it sends the player")]
    public Transform destination;

    [Header("Range")]
    public Transform interactPoint;      // centre of the radius (defaults to this object)
    public float radius = 4f;            // close enough to use the door
    public float showRingWithin = 10f;   // ring appears when this close
    public KeyCode key = KeyCode.E;

    [Header("Visuals")]
    public GameObject prompt;            // on-screen "E / Click" UI
    public Button promptButton;          // clicking the prompt also uses the door
    public GameObject radiusRing;        // circle on the floor showing the radius
    public float promptHeight = 3f;      // how high above the interact point the prompt sits

    private Transform player;
    private Camera cam;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;
        cam = Camera.main;

        if (interactPoint == null)
            interactPoint = transform;

        if (promptButton != null)
            promptButton.onClick.AddListener(Enter);

        // Ring sprite is 1 unit wide, so scale it to the radius
        if (radiusRing != null)
            radiusRing.transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);

        SetPrompt(false);
        SetRing(false);
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = FlatDistance(player.position, interactPoint.position);
        bool inRange = distance <= radius;

        SetRing(distance <= showRingWithin);
        SetPrompt(inRange);

        if (!inRange)
            return;

        // Keep the prompt on screen above the door
        if (prompt != null && cam != null)
            prompt.transform.position = cam.WorldToScreenPoint(interactPoint.position + Vector3.up * promptHeight);

        if (Input.GetKeyDown(key))
            Enter();
    }

    public void Enter()
    {
        if (destination == null || player == null)
            return;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.position = destination.position;
        }
        player.position = destination.position;

        SetPrompt(false);
        SetRing(false);
    }

    float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    void SetPrompt(bool visible)
    {
        if (prompt != null && prompt.activeSelf != visible)
            prompt.SetActive(visible);
    }

    void SetRing(bool visible)
    {
        if (radiusRing != null && radiusRing.activeSelf != visible)
            radiusRing.SetActive(visible);
    }
}
