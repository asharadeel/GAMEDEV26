// ============================================================
// Title:       RadiusUI
// Version:     2.2
// Written by:  cthullu96 (as MenuZone)
// Updated by:  Ashar (v2.0: renamed to RadiusUI, made reusable as the
//              Models/ShowUIonApproach prefab, ring always flat and sized.
//              v2.1: flat marker disc showing where to go,
//              v2.2: the red floor ring always shows)
// Description: Shows a UI when the player walks close and hides it
//              when they walk away. A red ring on the floor
//              always shows the radius.
//              Easiest use: drop the Models/ShowUIonApproach prefab
//              next to a sign and drag the UI's Frame into "UI".
// ============================================================

using UnityEngine;
using UnityEngine.Serialization;

public class RadiusUI : MonoBehaviour
{
    [Tooltip("The UI to show, normally a menu's Frame (see UI POLICY)")]
    [FormerlySerializedAs("menu")]
    public GameObject ui;

    public float radius = 3f;           // how close the player has to be to open it
    public float showRingWithin = 8f;   // not used any more, the ring always shows (kept for BossRoomBuilder)
    public GameObject radiusRing;       // circle on the floor (1 unit wide sprite)
    public GameObject marker;           // flat disc in the middle (half the ring's width) showing where to go

    private Transform player;
    private bool wasInside;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;

        SetUpRing();
        if (radiusRing != null)
            radiusRing.SetActive(true);   // always on
        SetUpMarker();

        if (ui != null)
            ui.SetActive(false);
    }

    void Update()
    {
        if (player == null)
            return;

        // Distance on the floor only, height doesn't matter
        Vector3 a = player.position;
        Vector3 b = transform.position;
        a.y = 0f;
        b.y = 0f;
        float distance = Vector3.Distance(a, b);

        bool inside = distance <= radius;

        // Just walked in: open
        if (inside && !wasInside && ui != null)
            ui.SetActive(true);

        // Just walked out: close
        if (!inside && wasInside && ui != null)
            ui.SetActive(false);

        wasInside = inside;
    }

    // Lay the ring flat on the floor and make it exactly the radius wide,
    // whatever the parent's rotation or scale is
    void SetUpRing()
    {
        if (radiusRing == null)
            return;

        Transform ring = radiusRing.transform;
        ring.rotation = Quaternion.Euler(90f, 0f, 0f);

        Vector3 parentScale = ring.parent != null ? ring.parent.lossyScale : Vector3.one;
        float size = radius * 2f;
        ring.localScale = new Vector3(size / Mathf.Max(parentScale.x, 0.0001f),
                                      size / Mathf.Max(parentScale.z, 0.0001f),
                                      1f);
    }

    // Flat blue disc on the floor in the middle of the zone, half the ring's width,
    // marking where to go. No collider, so it never blocks the player.
    void SetUpMarker()
    {
        if (marker == null)
            return;

        const float thickness = 0.03f;   // Unity's cylinder is 2 units tall, so scale y = thickness / 2
        Transform m = marker.transform;
        m.rotation = Quaternion.identity;
        Vector3 parentScale = m.parent != null ? m.parent.lossyScale : Vector3.one;
        m.localScale = new Vector3(radius / Mathf.Max(parentScale.x, 0.0001f),
                                   (thickness / 2f) / Mathf.Max(parentScale.y, 0.0001f),
                                   radius / Mathf.Max(parentScale.z, 0.0001f));
        m.position = transform.position + Vector3.up * (thickness / 2f);   // sits on the floor, just under the ring
    }

    // Shows the radius in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
