// ============================================================
// Title:       InventoryCard
// Version:     1.1
// Written by:  Ali
// Updated by:  Ashar (v1.1: equipped weapons can't be unequipped, only swapped)
// Description: One weapon in the Hub's Inventory (the
//              InventoryCard prefab). Drag the weapon asset into
//              Item. Shows its name, type and whether it's
//              equipped, with an Equip button. The equipped one
//              says "Equipped" and is greyed out: equip another
//              weapon to swap (you always need a shooter and melee).
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
        equipLabel.text = equipped ? "Equipped" : "Equip";
        equipButton.interactable = !equipped;
    }

    void Toggle()
    {
        if (ProfileManager.IsEquipped(item))
            return;   //can't go without one, equip another to swap
        ProfileManager.Equip(item);
        Debug.Log("Equipped " + item.displayName);

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
        return "";
    }
}
