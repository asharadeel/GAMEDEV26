// ============================================================
// Title:       AbilityCard
// Version:     1.0
// Written by:  Ali
// Description: One ability in the Hub's Inventory (the AbilityCard
//              prefab). Drag the ability asset into Item. Shows its
//              name and which key it's on, with a Q and an E button:
//              equips it on that key (moving it if it was on the
//              other one), or unequips it if it's already there.
//              Every ability is owned, so it always shows.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

public class AbilityCard : MonoBehaviour
{
    public AbilityData item;

    [Header("Card parts")]
    public Text nameText;
    public GameObject equippedTag;   // fixed above the buttons so it lines up on every card
    public Text equippedText;        // "On Q" / "On E"
    public Button qButton;
    public Text qLabel;
    public Button eButton;
    public Text eLabel;

    void Awake()
    {
        qButton.onClick.AddListener(() => Toggle(0));
        eButton.onClick.AddListener(() => Toggle(1));
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        int slot = ProfileManager.AbilitySlot(item);
        nameText.text = "<b>" + item.displayName + "</b>\nAbility";
        equippedTag.SetActive(slot >= 0);
        equippedText.text = slot == 0 ? "On Q" : "On E";
        qLabel.text = slot == 0 ? "Unequip Q" : "Equip Q";
        eLabel.text = slot == 1 ? "Unequip E" : "Equip E";
    }

    void Toggle(int slot)
    {
        if (ProfileManager.AbilitySlot(item) == slot)
            ProfileManager.UnequipAbility(slot);
        else
            ProfileManager.EquipAbility(item, slot);

        // equipping this one replaces whatever was on that key, so update every card
        foreach (AbilityCard card in transform.parent.GetComponentsInChildren<AbilityCard>())
            card.Refresh();
    }
}
