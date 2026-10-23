// ============================================================
// Title:       PlayerProfile
// Version:     2.1
// Written by:  Ashar
// Updated by:  cthullu96 (v1.6: portal piece and portal 2 progress)
//              Ali (v2.0: coins, shooter upgrade bars,
//              v2.1: abilities on Q/E and skill tree nodes, no special slot)
// Description: Everything about the player that gets saved:
//              name, passkey, ball colour, creation date, stats,
//              coins, owned items, item levels, shooter upgrades,
//              skill tree, current inventory and portal progress.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

//CURRENT INVENTORY
//only owned items go in here (ProfileManager.Equip checks), stored by id
[System.Serializable]
public class CurrentInventory
{
    public string shooter = "em_raygun";
    public string lazer = "lazer";
    public string melee = "punch";
    public string special = "";                                 // no longer used (abilities replaced specials)
    public string abilityQ = "fire";
    public string abilityE = "ice";
    public List<string> consumables = new List<string>();      // food/drink slots, "" = empty
}

//ITEM LEVEL
//items not listed are level 1
[System.Serializable]
public class ItemLevel
{
    public string id;
    public int level = 1;
}

//SHOOTER LEVELS
//0 = not upgraded, shooters not listed have nothing upgraded
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

//PLAYER PROFILE
//new stats: private [SerializeField] field + public property. item stats live in the item assets
[System.Serializable]
public class PlayerProfile
{
    public const int MaxInventorySlots = 7; // keys 4-9 and 0
    public const float MaxStat = 100f;      // health, stamina, hunger, thirst all go 0-100

    //key details (set on the Main Menu's profile select)
    [SerializeField] private string playerName = "";
    [SerializeField] private bool passwordProtected = false;
    [SerializeField] private string passkey = "";
    [SerializeField] private Color ballColour = Color.white;
    [SerializeField] private string createdAt = "";   // e.g. "22/10/2026 14:05"

    //games stats
    [SerializeField] private int level = 1;
    [SerializeField] private int tokens = 150;   // coins, still called tokens so old saves keep their balance

    [SerializeField] private float speed = 5f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float hunger = 100f;   // 100 = full, 0 = starving
    [SerializeField] private float thirst = 100f;   // 100 = full, 0 = dehydrated

    //owned items (ones ticked "unlocked" in their asset aren't listed)
    [SerializeField] private List<string> ownedItems = new List<string>();
    [SerializeField] private List<ItemLevel> itemLevels = new List<ItemLevel>();
    [SerializeField] private List<ShooterLevels> shooterLevels = new List<ShooterLevels>();
    [SerializeField] private List<string> skillNodes = new List<string>();   // unlocked skill tree nodes, e.g. "fire.2"

    //equipped / carried
    [SerializeField] private CurrentInventory currentInventory = new CurrentInventory();
    [SerializeField] private int selectedWeapon = 0;               // 0 shooter, 1 melee
    [SerializeField] private int inventorySlots = MaxInventorySlots; // usable consumable slots

    //portal progress
    [SerializeField] private bool hasPortalPiece = false;   // picked up from the level 1 boss
    [SerializeField] private bool portal2Open = false;      // placed the piece in the hub's portal 2

    //IDENTITY

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

    // used when resetting progress but keeping who the profile is
    public void CopyIdentity(PlayerProfile other)
    {
        playerName = other.playerName;
        passwordProtected = other.passwordProtected;
        passkey = other.passkey;
        ballColour = other.ballColour;
        createdAt = other.createdAt;
    }

    //PROGRESS

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

    //STATS

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

    //OWNED ITEMS

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

    // refunded: back to level 1, shooters lose their upgrades
    public void RemoveOwned(string itemId)
    {
        ownedItems.Remove(itemId);
        itemLevels.RemoveAll(l => l.id == itemId);
        shooterLevels.RemoveAll(l => l.id == itemId);
    }

    //ITEM LEVELS

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

    //SKILL TREE

    public bool HasSkillNode(string nodeId)
    {
        return skillNodes.Contains(nodeId);
    }

    public void AddSkillNode(string nodeId)
    {
        if (!HasSkillNode(nodeId))
            skillNodes.Add(nodeId);
    }

    //SHOOTER UPGRADES

    // 0 = not upgraded yet
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

    //CURRENT INVENTORY

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

    // 0 = Q, 1 = E
    public string GetAbility(int slot)
    {
        return slot == 0 ? currentInventory.abilityQ : currentInventory.abilityE;
    }

    public void SetAbility(int slot, string abilityId)
    {
        if (slot == 0)
            currentInventory.abilityQ = abilityId;
        else
            currentInventory.abilityE = abilityId;
    }

    public string EquippedSpecial
    {
        get { return currentInventory.special; }
        set { currentInventory.special = value; }
    }

    public int SelectedWeapon
    {
        get { return Mathf.Clamp(selectedWeapon, 0, 1); }   // old saves can still say 2 (the special slot)
        set { selectedWeapon = Mathf.Clamp(value, 0, 1); }
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

    // takes from the last slot it's in
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

    // first empty usable slot, false if full
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
