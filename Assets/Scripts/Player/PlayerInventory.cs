// ============================================================
// Title:       PlayerInventory
// Version:     1.5
// Written by:  Ashar
// Updated by:  Ali (v1.4: 3rd label shows the Q / E abilities)
//              cthullu96 (v1.5: input system package)
//              ai use: input system change written with claude (anthropic), owner cthullu96
// Description: 10-slot inventory, I to open, 4-9/0 to eat/drink.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//goes on the Player. weapon keys are handled by PlayerCombat
[RequireComponent(typeof(PlayerStats))]
public class PlayerInventory : MonoBehaviour
{
    //interface
    public GameObject inventoryPanel;
    public Text[] slotLabels = new Text[10];   //order 1,2,3,4,5,6,7,8,9,0

    private PlayerStats stats;

    //STARTING PROCEDURES
    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    //LISTENERS
    void Update()
    {
        if (GameInput.Inventory.WasPressedThisFrame() && inventoryPanel != null)
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        for (int i = 0; i < GameInput.UseSlot.Length && !PlayerController.uiEnabled; i++)
        {
            if (GameInput.UseSlot[i].WasPressedThisFrame())
                UseSlot(i);
        }

        if (inventoryPanel != null && inventoryPanel.activeSelf)
            RefreshLabels();
    }

    //USE ITEMS
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
        Debug.Log("Used an item.");
    }

    //INTERFACE
    void RefreshLabels()
    {
        PlayerProfile p = stats.Profile;
        ItemDatabase db = ItemDatabase.Instance;

        string shooter = NameOf(db.Get<ShooterData>(p.EquippedShooter));
        string lazer = NameOf(PlayerCombat.GetLazer(db.Get<ShooterData>(p.EquippedShooter)));
        SetLabel(0, "1", shooter + " - " + lazer, p.SelectedWeapon == 0);
        SetLabel(1, "2", NameOf(db.Get<MeleeData>(p.EquippedMelee)), p.SelectedWeapon == 1);
        SetLabel(2, "Q / E", NameOf(db.Get<AbilityData>(p.GetAbility(0))) + " / " + NameOf(db.Get<AbilityData>(p.GetAbility(1))), false);

        for (int i = 0; i < GameInput.UseSlot.Length; i++)
        {
            string key = i == GameInput.UseSlot.Length - 1 ? "0" : (i + 4).ToString();
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
