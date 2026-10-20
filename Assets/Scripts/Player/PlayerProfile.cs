// ============================================================
// Title:       PlayerProfile
// Version:     1.3
// Written by:  Ashar
// Description: Everything about the player that gets saved:
//              stats, tokens, owned items and current inventory.
// ============================================================

using System.Collections.Generic;
using UnityEngine;

// What the player has on them right now. Only owned items can go in here
// (ProfileManager.Equip checks that). Items are stored by id.
[System.Serializable]
public class CurrentInventory
{
    public string shooter = "em_raygun";
    public string lazer = "lazer";
    public string melee = "punch";
    public string special = "";                                 // none yet
    public List<string> consumables = new List<string>();      // food/drink slots, "" = empty
}

// Add new stats here: a private [SerializeField] field + a public property.
// Items are stored by id only; their stats live in the item assets.
[System.Serializable]
public class PlayerProfile
{
    public const int MaxInventorySlots = 7; // keys 4-9 and 0

    [SerializeField] private int level = 1;
    [SerializeField] private int tokens = 0;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float hunger = 100f;   // 100 = full, 0 = starving
    [SerializeField] private float maxThirst = 100f;
    [SerializeField] private float thirst = 100f;   // 100 = full, 0 = dehydrated

    // Everything bought (items ticked "unlocked" in their asset don't need to be here)
    [SerializeField] private List<string> ownedItems = new List<string>();

    // What's equipped / carried right now
    [SerializeField] private CurrentInventory currentInventory = new CurrentInventory();
    [SerializeField] private int selectedWeapon = 0;               // 0 shooter, 1 melee, 2 special
    [SerializeField] private int inventorySlots = MaxInventorySlots; // usable consumable slots

    // ---- Stats ----

    public int Level
    {
        get { return level; }
        set { level = Mathf.Max(1, value); }
    }

    public int Tokens
    {
        get { return tokens; }
        set { tokens = Mathf.Max(0, value); }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = Mathf.Max(0f, value); }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = Mathf.Max(1f, value); Health = health; }
    }

    public float Health
    {
        get { return health; }
        set { health = Mathf.Clamp(value, 0f, maxHealth); }
    }

    public float MaxStamina
    {
        get { return maxStamina; }
        set { maxStamina = Mathf.Max(1f, value); Stamina = stamina; }
    }

    public float Stamina
    {
        get { return stamina; }
        set { stamina = Mathf.Clamp(value, 0f, maxStamina); }
    }

    public float MaxHunger
    {
        get { return maxHunger; }
        set { maxHunger = Mathf.Max(1f, value); Hunger = hunger; }
    }

    public float Hunger
    {
        get { return hunger; }
        set { hunger = Mathf.Clamp(value, 0f, maxHunger); }
    }

    public float MaxThirst
    {
        get { return maxThirst; }
        set { maxThirst = Mathf.Max(1f, value); Thirst = thirst; }
    }

    public float Thirst
    {
        get { return thirst; }
        set { thirst = Mathf.Clamp(value, 0f, maxThirst); }
    }

    public void RefillAll()
    {
        health = maxHealth;
        stamina = maxStamina;
        hunger = maxHunger;
        thirst = maxThirst;
    }

    // ---- Owned items ----

    public IReadOnlyList<string> OwnedItems
    {
        get { return ownedItems; }
    }

    public bool HasBought(string itemId)
    {
        return ownedItems.Contains(itemId);
    }

    public void AddOwned(string itemId)
    {
        if (!HasBought(itemId))
            ownedItems.Add(itemId);
    }

    // ---- Current inventory ----

    public CurrentInventory CurrentInventory
    {
        get { return currentInventory; }
    }

    public string EquippedShooter
    {
        get { return currentInventory.shooter; }
        set { currentInventory.shooter = value; }
    }

    public string EquippedLazer
    {
        get { return currentInventory.lazer; }
        set { currentInventory.lazer = value; }
    }

    public string EquippedMelee
    {
        get { return currentInventory.melee; }
        set { currentInventory.melee = value; }
    }

    public string EquippedSpecial
    {
        get { return currentInventory.special; }
        set { currentInventory.special = value; }
    }

    public int SelectedWeapon
    {
        get { return selectedWeapon; }
        set { selectedWeapon = Mathf.Clamp(value, 0, 2); }
    }

    public int InventorySlots
    {
        get { return inventorySlots; }
        set { inventorySlots = Mathf.Clamp(value, 0, MaxInventorySlots); }
    }

    public string GetSlot(int index)
    {
        List<string> slots = currentInventory.consumables;
        if (index < 0 || index >= inventorySlots || index >= slots.Count)
            return "";
        return slots[index];
    }

    public void SetSlot(int index, string itemId)
    {
        if (index < 0 || index >= inventorySlots)
            return;

        List<string> slots = currentInventory.consumables;
        while (slots.Count <= index)
            slots.Add("");
        slots[index] = itemId;
    }

    // Puts the item in the first empty usable slot. False if full.
    public bool AddToInventory(string itemId)
    {
        for (int i = 0; i < inventorySlots; i++)
        {
            if (string.IsNullOrEmpty(GetSlot(i)))
            {
                SetSlot(i, itemId);
                return true;
            }
        }
        return false;
    }
}
