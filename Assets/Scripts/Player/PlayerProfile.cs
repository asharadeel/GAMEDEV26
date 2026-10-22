// ============================================================
// Title:       PlayerProfile
// Version:     2.0
// Written by:  Ashar
// Updated by:  cthullu96 (v1.6: portal piece and portal 2 progress)
//              Ali (v2.0: coins, shooter upgrade bars)
// Description: Everything about the player that gets saved:
//              name, passkey, ball colour, creation date, stats,
//              coins, owned items, item levels, shooter upgrades,
//              current inventory and portal progress.
// ============================================================

using System.Collections.Generic;
using System.Text.RegularExpressions;
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

// An owned item's upgrade level (items not listed are level 1)
[System.Serializable]
public class ItemLevel
{
    public string id;
    public int level = 1;
}

// A shooter's upgrade bars, 0 = not upgraded (shooters not listed have nothing upgraded)
[System.Serializable]
public class ShooterLevels
{
    public string id;
    public int lazer;        // 0 = the gun's 1st lazer, 1 = 2nd...
    public int damage;
    public int fireRate;
    public int bulletSpeed;
    public int reloadTime;

    public int Get(ShooterStat stat)
    {
        switch (stat)
        {
            case ShooterStat.Lazer: return lazer;
            case ShooterStat.Damage: return damage;
            case ShooterStat.FireRate: return fireRate;
            case ShooterStat.BulletSpeed: return bulletSpeed;
            default: return reloadTime;
        }
    }

    public void Set(ShooterStat stat, int level)
    {
        switch (stat)
        {
            case ShooterStat.Lazer: lazer = level; break;
            case ShooterStat.Damage: damage = level; break;
            case ShooterStat.FireRate: fireRate = level; break;
            case ShooterStat.BulletSpeed: bulletSpeed = level; break;
            default: reloadTime = level; break;
        }
    }
}

// Add new stats here: a private [SerializeField] field + a public property.
// Items are stored by id only; their stats live in the item assets.
[System.Serializable]
public class PlayerProfile
{
    public const int MaxInventorySlots = 7; // keys 4-9 and 0
    public const float MaxStat = 100f;      // health, stamina, hunger, thirst all go 0-100

    // Who this profile is (set on the Main Menu's profile select)
    [SerializeField] private string playerName = "";
    [SerializeField] private bool passwordProtected = false;
    [SerializeField] private string passkey = "";
    [SerializeField] private Color ballColour = Color.white;
    [SerializeField] private string createdAt = "";   // e.g. "22/10/2026 14:05"

    [SerializeField] private int level = 1;
    [SerializeField] private int tokens = 150;   // coins, starting amount for a new profile (still called tokens in the save so old saves keep their balance)

    [SerializeField] private float speed = 5f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float hunger = 100f;   // 100 = full, 0 = starving
    [SerializeField] private float thirst = 100f;   // 100 = full, 0 = dehydrated

    // Everything bought (items ticked "unlocked" in their asset don't need to be here)
    [SerializeField] private List<string> ownedItems = new List<string>();
    [SerializeField] private List<ItemLevel> itemLevels = new List<ItemLevel>();
    [SerializeField] private List<ShooterLevels> shooterLevels = new List<ShooterLevels>();

    // What's equipped / carried right now
    [SerializeField] private CurrentInventory currentInventory = new CurrentInventory();
    [SerializeField] private int selectedWeapon = 0;               // 0 shooter, 1 melee, 2 special
    [SerializeField] private int inventorySlots = MaxInventorySlots; // usable consumable slots

    [SerializeField] private bool hasPortalPiece = false;   // picked up from the level 1 boss
    [SerializeField] private bool portal2Open = false;      // placed the piece in the hub's portal 2

    // ---- Identity ----

    // A brand new profile, stamped with the date and time it was made
    public static PlayerProfile CreateNew(string name, bool usePasskey, string key, Color colour)
    {
        PlayerProfile p = new PlayerProfile();
        p.PlayerName = name;
        p.BallColour = colour;
        p.passwordProtected = usePasskey;
        p.passkey = usePasskey ? key : "";
        p.createdAt = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        return p;
    }

    // Passkeys are 3-30 characters
    public static bool IsValidPasskey(string key)
    {
        return key != null && Regex.IsMatch(key, "^.{3,30}$");
    }

    public string PlayerName
    {
        get { return playerName; }
        set { playerName = value == null ? "" : value.Trim(); }
    }

    public bool PasswordProtected
    {
        get { return passwordProtected; }
    }

    // True if the key is right (or the profile has no passkey)
    public bool CheckPasskey(string key)
    {
        return !passwordProtected || key == passkey;
    }

    public Color BallColour
    {
        get { return ballColour; }
        set { ballColour = value; }
    }

    public string CreatedAt
    {
        get { return createdAt; }
    }

    // Copies name, passkey, colour and creation date from another profile
    // (used when resetting progress but keeping who the profile is)
    public void CopyIdentity(PlayerProfile other)
    {
        playerName = other.playerName;
        passwordProtected = other.passwordProtected;
        passkey = other.passkey;
        ballColour = other.ballColour;
        createdAt = other.createdAt;
    }

    // progress

    public bool HasPortalPiece
    {
        get { return hasPortalPiece; }
        set { hasPortalPiece = value; }
    }

    public bool Portal2Open
    {
        get { return portal2Open; }
        set { portal2Open = value; }
    }

    // ---- Stats ----

    public int Level
    {
        get { return level; }
        set { level = Mathf.Max(1, value); }
    }

    public int Coins
    {
        get { return tokens; }
        set { tokens = Mathf.Max(0, value); }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = Mathf.Max(0f, value); }
    }

    public float Health
    {
        get { return health; }
        set { health = Mathf.Clamp(value, 0f, MaxStat); }
    }

    public float Stamina
    {
        get { return stamina; }
        set { stamina = Mathf.Clamp(value, 0f, MaxStat); }
    }

    public float Hunger
    {
        get { return hunger; }
        set { hunger = Mathf.Clamp(value, 0f, MaxStat); }
    }

    public float Thirst
    {
        get { return thirst; }
        set { thirst = Mathf.Clamp(value, 0f, MaxStat); }
    }

    public void RefillAll()
    {
        health = MaxStat;
        stamina = MaxStat;
        hunger = MaxStat;
        thirst = MaxStat;
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

    // Refunded: no longer owned and back to level 1 (shooters lose their upgrades)
    public void RemoveOwned(string itemId)
    {
        ownedItems.Remove(itemId);
        itemLevels.RemoveAll(l => l.id == itemId);
        shooterLevels.RemoveAll(l => l.id == itemId);
    }

    // ---- Item levels ----

    public int GetItemLevel(string itemId)
    {
        foreach (ItemLevel l in itemLevels)
        {
            if (l.id == itemId)
                return l.level;
        }
        return 1;
    }

    public void SetItemLevel(string itemId, int level)
    {
        foreach (ItemLevel l in itemLevels)
        {
            if (l.id == itemId)
            {
                l.level = level;
                return;
            }
        }
        itemLevels.Add(new ItemLevel { id = itemId, level = level });
    }

    // ---- Shooter upgrades ----

    // How many times one of a shooter's bars has been upgraded (0 = not yet)
    public int GetShooterLevel(string shooterId, ShooterStat stat)
    {
        foreach (ShooterLevels l in shooterLevels)
        {
            if (l.id == shooterId)
                return l.Get(stat);
        }
        return 0;
    }

    public void SetShooterLevel(string shooterId, ShooterStat stat, int level)
    {
        foreach (ShooterLevels l in shooterLevels)
        {
            if (l.id == shooterId)
            {
                l.Set(stat, level);
                return;
            }
        }
        ShooterLevels added = new ShooterLevels { id = shooterId };
        added.Set(stat, level);
        shooterLevels.Add(added);
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

    // How many of an item are in the usable slots
    public int CountInInventory(string itemId)
    {
        int count = 0;
        for (int i = 0; i < inventorySlots; i++)
        {
            if (GetSlot(i) == itemId)
                count++;
        }
        return count;
    }

    // Takes one of the item out (the last slot it's in). False if there's none
    public bool RemoveFromInventory(string itemId)
    {
        for (int i = inventorySlots - 1; i >= 0; i--)
        {
            if (GetSlot(i) == itemId)
            {
                SetSlot(i, "");
                return true;
            }
        }
        return false;
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
