// ============================================================
// Title:       InventoryGrid
// Version:     1.0
// Written by:  Ali
// Description: Goes on the grid that holds the InventoryCards
//              (with a Grid Layout Group). Each time the window
//              opens it shows only the cards for weapons the
//              player owns, so bought weapons appear and refunded
//              ones disappear. Place one card per weapon by hand.
// ============================================================

using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (InventoryCard card in GetComponentsInChildren<InventoryCard>(true))
        {
            bool owned = ProfileManager.Owns(card.item);
            card.gameObject.SetActive(owned);
            if (owned)
                card.Refresh();
        }
    }
}
