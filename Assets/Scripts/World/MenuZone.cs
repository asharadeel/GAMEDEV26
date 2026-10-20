using UnityEngine;

// put this on a spot in the hub (shop, settings, intro wall)
// when the player walks close enough the menu opens
// when they walk away it closes again
public class MenuZone : MonoBehaviour
{
    public GameObject menu;            // the menu panel to show
    public float radius = 3f;          // how close the player has to be
    public float showRingWithin = 8f;  // the floor ring shows when this close
    public GameObject radiusRing;      // circle on the floor

    private Transform player;
    private bool wasInside;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;

        // ring picture is 1 unit wide so make it match the radius
        if (radiusRing != null)
            radiusRing.transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);

        if (menu != null)
            menu.SetActive(false);
    }

    void Update()
    {
        if (player == null)
            return;

        // distance on the floor only, height doesnt matter
        Vector3 a = player.position;
        Vector3 b = transform.position;
        a.y = 0f;
        b.y = 0f;
        float distance = Vector3.Distance(a, b);

        if (radiusRing != null)
            radiusRing.SetActive(distance <= showRingWithin);

        bool inside = distance <= radius;

        // just walked in so open the menu
        if (inside && !wasInside && menu != null)
            menu.SetActive(true);

        // just walked out so close the menu
        if (!inside && wasInside && menu != null)
            menu.SetActive(false);

        wasInside = inside;
    }
}
