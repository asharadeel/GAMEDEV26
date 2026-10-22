// ============================================================
// Title:       InventoryCard
// Version:     1.0
// Written by:  Ali
// Description: One weapon in the Hub's Inventory (the
//              InventoryCard prefab). Drag the weapon asset into
//              Item. Shows its name, type and whether it's
//              equipped, with one Equip / Unequip button.
//              The InventoryGrid it sits in hides it when the
//              weapon isn't owned.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

public class InventoryCard : MonoBehaviour
{
    public WeaponData item;

    [Header("Card parts")]
    public Text nameText;
    public GameObject equippedTag;   // "Equipped", fixed above the button so it lines up on every card
    public Button equipButton;
    public Text equipLabel;

    void Awake()
    {
        equipButton.onClick.AddListener(Toggle);
    }

    public void Refresh()
    {
        bool equipped = ProfileManager.IsEquipped(item);
        nameText.text = "<b>" + item.displayName + "</b>\n" + Kind();
        equippedTag.SetActive(equipped);
        equipLabel.text = equipped ? "Unequip" : "Equip";
    }

    void Toggle()
    {
        if (ProfileManager.IsEquipped(item))
            ProfileManager.Unequip(item);
        else
            ProfileManager.Equip(item);

        // equipping this one unequips whatever was in its slot, so update every card
        InventoryGrid grid = GetComponentInParent<InventoryGrid>();
        if (grid != null)
            grid.Refresh();
        else
            Refresh();
    }

    string Kind()
    {
        if (item is ShooterData) return "Shooter";
        if (item is MeleeData) return "Melee";
        if (item is SpecialData) return "Special";
        return "";
    }
}
