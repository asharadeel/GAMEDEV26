// ============================================================
// Title:       PlayerProfile
// Version:     2.7
// Written by:  Ashar
// Updated by:  cthullu96 (v1.6: portal piece and portal 2 progress)
//              Ali (v2.0: coins, shooter upgrade bars,
//              v2.1: abilities on Q/E and skill tree nodes, no special slot)
//              Ashar (v2.2: ore pouch and armour, v2.3: food slots are 4-9, 0 is the armour slot,
//              v2.4: welcome seen, shard count, v2.5: stored food, specials removed,
//              v2.6: new profiles start with 2 of each food/drink stored, v2.7: xp and levelling up)
// Description: Everything about the player that gets saved:
//              name, passkey, ball colour, creation date, stats,
//              coins, owned items, item levels, shooter upgrades,
//              skill tree, current inventory, portal progress,
//              ores and armour.
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

//ITEM COUNT
//how many of something (ores, spare armour)
[System.Serializable]
public class ItemCount
{
    public string id;
    public int count;
}

//SHOOTER LEVELS
//0 = not upgraded, shooters not listed have nothing upgraded
[System.Serializable]
public class ShooterLevels
{
    public string id;
    public int lazer;       
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
[System.Serializable]
public class PlayerProfile
{
    public const int MaxInventorySlots = 6;
    public const float MaxStat = 100f;    

    //key details (set on the Main Menu's profile select)
    [SerializeField] private string playerName = "";
    [SerializeField] private bool passwordProtected = false;
    [SerializeField] private string passkey = "";
    [SerializeField] private Color ballColour = Color.white;
    [SerializeField] private string createdAt = "";   // e.g. "22/10/2026 14:05"

    //games stats
    [SerializeField] private int level = 1;
    [SerializeField] private int xp = 0;          // towards the next level
    [SerializeField] private int tokens = 150;   

    [SerializeField] private float speed = 5f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float hunger = 100f;   
    [SerializeField] private float thirst = 100f;  

    //owned items (ones ticked "unlocked" in their asset aren't listed)
    [SerializeField] private List<string> ownedItems = new List<string>();
    [SerializeField] private List<ItemLevel> itemLevels = new List<ItemLevel>();
    [SerializeField] private List<ShooterLevels> shooterLevels = new List<ShooterLevels>();
    [SerializeField] private List<string> skillNodes = new List<string>();  

    //equipped / carried
    [SerializeField] private CurrentInventory currentInventory = new CurrentInventory();
    [SerializeField] private int selectedWeapon = 0;         
    [SerializeField] private int inventorySlots = MaxInventorySlots;

    //ores and armour
    [SerializeField] private List<ItemCount> ores = new List<ItemCount>();
    [SerializeField] private List<ItemCount> storedFood = new List<ItemCount>();   // food/drink kept out of the backpack (4-9 slots)
    [SerializeField] private List<ItemCount> armor = new List<ItemCount>();  
    [SerializeField] private string equippedArmor = "";
    [SerializeField] private float armorShield = 0f;                        

    //portal progress
    [SerializeField] private bool hasPortalPiece = false;  
    [SerializeField] private bool portal2Open = false;     
    [SerializeField] private bool welcomeSeen = false;    

    //IDENTITY
    public static readonly string[] StarterFood = { "beans", "water_bottle", "soda" };
    public const int StarterFoodAmount = 2;
    public const int XpPerLevel = 500;


    // A brand new profile, stamped with the date and time it was made
    public static PlayerProfile CreateNew(string name, bool usePasskey, string key, Color colour)
    {
        PlayerProfile p = new PlayerProfile();
        p.PlayerName = name;
        p.BallColour = colour;
        p.passwordProtected = usePasskey;
        p.passkey = usePasskey ? key : "";
        p.createdAt = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        //starter food, picked into the backpack in the Hub
        foreach (string id in StarterFood)
            p.AddStoredFood(id, StarterFoodAmount);
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

    public bool WelcomeSeen
    {
        get { return welcomeSeen; }
        set { welcomeSeen = value; }
    }

    //shards collected for the hub's shard portal: the piece the level 1 boss drops, once picked up
    public int ShardCount
    {
        get { return hasPortalPiece ? 1 : 0; }
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

    //xp needed to go from this level to the next (level 1: 500, level 2: 1000...)
    public int XpToNextLevel
    {
        get { return XpPerLevel * level; }
    }

    public int Xp
    {
        get { return xp; }
    }

    // adds xp and levels up as many times as it covers, returns how many levels were gained
    public int AddXp(int amount)
    {
        int gained = 0;
        xp += Mathf.Max(0, amount);
        while (xp >= XpToNextLevel)
        {
            xp -= XpToNextLevel;
            level++;
            gained++;
        }
        return gained;
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

    //ORES
    public int GetOreCount(string oreId)
    {
        return GetCount(ores, oreId);
    }

    public void AddOre(string oreId, int amount)
    {
        AddCount(ores, oreId, amount);
    }

    //false if there aren't enough
    public bool TakeOre(string oreId, int amount)
    {
        if (GetOreCount(oreId) < amount)
            return false;
        AddCount(ores, oreId, -amount);
        return true;
    }

    //STORED FOOD
    public int GetStoredFood(string itemId)
    {
        return GetCount(storedFood, itemId);
    }

    public void AddStoredFood(string itemId, int amount)
    {
        AddCount(storedFood, itemId, amount);
    }

    public bool TakeStoredFood(string itemId)
    {
        if (GetStoredFood(itemId) <= 0)
            return false;
        AddCount(storedFood, itemId, -1);
        return true;
    }

    //ARMOUR
    public string EquippedArmor
    {
        get { return equippedArmor; }
        set { equippedArmor = value == null ? "" : value; }
    }

    public float ArmorShield
    {
        get { return armorShield; }
        set { armorShield = Mathf.Max(0f, value); }
    }

    public int GetArmorCount(string armorId)
    {
        return GetCount(armor, armorId);
    }

    public void AddArmor(string armorId, int amount)
    {
        AddCount(armor, armorId, amount);
    }

    //COUNT HELPERS
    int GetCount(List<ItemCount> list, string id)
    {
        foreach (ItemCount c in list)
        {
            if (c.id == id)
                return c.count;
        }
        return 0;
    }

    void AddCount(List<ItemCount> list, string id, int amount)
    {
        foreach (ItemCount c in list)
        {
            if (c.id == id)
            {
                c.count = Mathf.Max(0, c.count + amount);
                return;
            }
        }
        if (amount > 0)
            list.Add(new ItemCount { id = id, count = amount });
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


    public int SelectedWeapon
    {
        get { return Mathf.Clamp(selectedWeapon, 0, 1); }   // old saves can still say 2 (the special slot)
        set { selectedWeapon = Mathf.Clamp(value, 0, 1); }
    }

    public int InventorySlots
    {
        get { return Mathf.Min(inventorySlots, MaxInventorySlots); }   // old saves had 7
        set { inventorySlots = Mathf.Clamp(value, 0, MaxInventorySlots); }
    }

    public string GetSlot(int index)
    {
        List<string> slots = currentInventory.consumables;
        if (index < 0 || index >= InventorySlots || index >= slots.Count)
            return "";
        return slots[index];
    }

    public void SetSlot(int index, string itemId)
    {
        if (index < 0 || index >= InventorySlots)
            return;

        List<string> slots = currentInventory.consumables;
        while (slots.Count <= index)
            slots.Add("");
        slots[index] = itemId;
    }

    public int CountInInventory(string itemId)
    {
        int count = 0;
        for (int i = 0; i < InventorySlots; i++)
        {
            if (GetSlot(i) == itemId)
                count++;
        }
        return count;
    }

    // takes from the last slot it's in
    public bool RemoveFromInventory(string itemId)
    {
        for (int i = InventorySlots - 1; i >= 0; i--)
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
        for (int i = 0; i < InventorySlots; i++)
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
