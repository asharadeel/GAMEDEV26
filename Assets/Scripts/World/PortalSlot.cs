using UnityEngine;

// goes on level 2's portal in the hub
// the slot on its pillar starts dark grey
// once the player has the portal piece from the level 1 boss, walking up asks them to place it
// placing it turns the slot purple and opens the portal
// theres no level 2 yet so the open portal doesnt take you anywhere
public class PortalSlot : MonoBehaviour
{
    public Renderer slot;                // the square on the pillar
    public Renderer circle;              // the portal itself
    public Material openMaterial;        // purple, used for both when its open
    public RadiusUI approach;            // the ring in front of the portal
    public GameObject placePrompt;       // frame of the "place the piece?" pop up
    public HubPortal hubPortal;

    void Start()
    {
        PlayerProfile p = ProfileManager.Profile;

        if (p.Portal2Open)
            Open();
        else if (p.HasPortalPiece && approach != null)
            approach.ui = placePrompt;   // ask to place the piece instead of the normal locked pop up
    }

    // the place button on the pop up calls this
    public void Place()
    {
        PlayerProfile p = ProfileManager.Profile;
        p.HasPortalPiece = false;
        p.Portal2Open = true;
        ProfileManager.Save();

        if (placePrompt != null)
            placePrompt.SetActive(false);

        Open();
        FloatingText.Show(transform.position + Vector3.up * 6f, "Portal 2 opened!");
    }

    void Open()
    {
        if (slot != null)
            slot.sharedMaterial = openMaterial;
        if (circle != null)
            circle.sharedMaterial = openMaterial;

        // open but nothing to go to yet, so no pop ups and no locked message
        if (approach != null)
            approach.ui = null;
        if (hubPortal != null)
            hubPortal.lockedMessage = null;
    }

    // right click the component for these while testing
    [ContextMenu("Give Portal Piece (testing)")]
    void GivePiece()
    {
        ProfileManager.Profile.HasPortalPiece = true;
        ProfileManager.Profile.Portal2Open = false;
        ProfileManager.Save();
    }
}
