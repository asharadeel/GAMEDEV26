// ============================================================
// Title:       PlayerInventory
// Version:     1.3
// Written by:  Ashar
// Description: 10-slot inventory, I to open, 4-9/0 to eat/drink.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

// Goes on the Player. 10 slots:
//   1 = shooter, 2 = melee, 3 = special (weapon keys are handled by PlayerCombat)
//   4-9 and 0 = food/drink slots, pressing the key eats/drinks it
// I opens/closes the inventory panel.
[RequireComponent(typeof(PlayerStats))]
public class PlayerInventory : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.I;
    public GameObject inventoryPanel;

    // One label per slot, in order 1,2,3,4,5,6,7,8,9,0
    public Text[] slotLabels = new Text[10];

    private static readonly KeyCode[] ItemKeys =
    {
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7,
        KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
    };

    private PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey) && inventoryPanel != null)
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        for (int i = 0; i < ItemKeys.Length && !PlayerController.uiEnabled; i++)
        {
            if (Input.GetKeyDown(ItemKeys[i]))
                UseSlot(i);
        }

        if (inventoryPanel != null && inventoryPanel.activeSelf)
            RefreshLabels();
    }

    public void UseSlot(int index)
    {
        string id = stats.Profile.GetSlot(index);
        if (string.IsNullOrEmpty(id))
            return;

        ConsumableData item = ItemDatabase.Instance.Get<ConsumableData>(id);
        if (item == null)
            return;

        stats.Consume(item);
        stats.Profile.SetSlot(index, "");
    }

    void RefreshLabels()
    {
        PlayerProfile p = stats.Profile;
        ItemDatabase db = ItemDatabase.Instance;

        string shooter = NameOf(db.Get<ShooterData>(p.EquippedShooter));
        string lazer = NameOf(PlayerCombat.GetLazer(db.Get<ShooterData>(p.EquippedShooter)));
        SetLabel(0, "1", shooter + " - " + lazer, p.SelectedWeapon == 0);
        SetLabel(1, "2", NameOf(db.Get<MeleeData>(p.EquippedMelee)), p.SelectedWeapon == 1);
        SetLabel(2, "3", NameOf(db.Get<SpecialData>(p.EquippedSpecial)), p.SelectedWeapon == 2);

        for (int i = 0; i < ItemKeys.Length; i++)
        {
            string key = i == ItemKeys.Length - 1 ? "0" : (i + 4).ToString();
            SetLabel(i + 3, key, NameOf(db.Get<ConsumableData>(p.GetSlot(i))), false);
        }
    }

    void SetLabel(int index, string key, string itemName, bool selected)
    {
        if (index >= slotLabels.Length || slotLabels[index] == null)
            return;

        slotLabels[index].text = key + "\n" + itemName;
        slotLabels[index].color = selected ? Color.yellow : Color.white;
    }

    string NameOf(ItemData item)
    {
        return item != null ? item.displayName : "";
    }
}
