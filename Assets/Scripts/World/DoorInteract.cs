// ============================================================
// Title:       DoorInteract
// Version:     1.2
// Written by:  Ashar
// Updated by:  Ali (v1.1: PlayerNearDoor, so E opens doors instead of using an ability)
//              cthullu96 (v1.2: input system package)
//              ai use: input system change written with claude (anthropic), owner cthullu96
// Description: A door the player can use when close. Shows a ring
//              on the floor for the radius and an "E / Click"
//              prompt on screen. Press E or click the prompt to be
//              moved to the destination.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorInteract : MonoBehaviour
{
    [Header("Where it sends the player")]
    public Transform destination;

    [Header("Range")]
    public Transform interactPoint;      //defaults to this object
    public float radius = 4f;
    public float showRingWithin = 10f;

    [Header("Visuals")]
    public GameObject prompt;            //on-screen "E / Click" UI
    public Button promptButton;
    public GameObject radiusRing;
    public float promptHeight = 3f;

    private Transform player;
    private Camera cam;

    private static readonly HashSet<DoorInteract> inRangeDoors = new HashSet<DoorInteract>();

    //PlayerAbilities skips the E ability while this is true
    public static bool PlayerNearDoor
    {
        get { return inRangeDoors.Count > 0; }
    }

    //STARTING PROCEDURES
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

        //ring sprite is 1 unit wide
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
        if (inRange)
            inRangeDoors.Add(this);
        else
            inRangeDoors.Remove(this);

        SetRing(distance <= showRingWithin);
        SetPrompt(inRange);

        if (!inRange)
            return;

        //keep prompt above the door
        if (prompt != null && cam != null)
            prompt.transform.position = cam.WorldToScreenPoint(interactPoint.position + Vector3.up * promptHeight);

        if (GameInput.Interact.WasPressedThisFrame())
            Enter();
    }

    //TELEPORT
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
        Debug.Log("Entered door to " + destination.name);
    }

    //HELPERS
    float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    void OnDisable()
    {
        inRangeDoors.Remove(this);
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
