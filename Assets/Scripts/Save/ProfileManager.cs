// ============================================================
// Title:       ProfileManager
// Version:     1.2
// Written by:  Ashar
// Description: The one shared player profile, plus buy/equip helpers.
// ============================================================

using System.Collections.Generic;
using UnityEngine;

// The one player profile, usable from any script in any scene:
//   ProfileManager.Profile.Tokens += 50;
//   ProfileManager.TryBuyWeapon(someWeapon);
// Loads profile.json the first time it's used and saves when the game closes.
public static class ProfileManager
{
    private const string SaveFile = "profile";

    private static PlayerProfile profile;

    public static PlayerProfile Profile
    {
        get
        {
            if (profile == null)
            {
                profile = SaveSystem.Load(SaveFile, new PlayerProfile());
                Application.quitting += Save;
            }
            return profile;
        }
    }

    public static void Save()
    {
        if (profile != null)
            SaveSystem.Save(SaveFile, profile);
    }

    // Deletes the file and starts a fresh profile
    public static void Delete()
    {
        SaveSystem.Delete(SaveFile);
        profile = new PlayerProfile();
    }

    // ---- Weapons ----

    public static bool Owns(WeaponData weapon)
    {
        return weapon != null && (weapon.unlocked || Profile.HasBought(weapon.id));
    }

    public static bool TryBuyWeapon(WeaponData weapon)
    {
        if (weapon == null || Owns(weapon))
            return false;
        if (!CanAfford(weapon))
            return false;

        Profile.Tokens -= weapon.tokenCost;
        Profile.AddOwned(weapon.id);
        return true;
    }

    // Enough tokens and a high enough level
    public static bool CanAfford(ItemData item)
    {
        return item != null && Profile.Level >= item.levelRequired && Profile.Tokens >= item.tokenCost;
    }

    // Every owned item of a type, e.g. GetOwned<LazerData>() for a "pick your lazer" menu
    public static List<T> GetOwned<T>() where T : WeaponData
    {
        List<T> owned = new List<T>();
        foreach (T item in ItemDatabase.Instance.GetAll<T>())
        {
            if (Owns(item))
                owned.Add(item);
        }
        return owned;
    }

    // Puts an owned item into the current inventory
    public static bool Equip(WeaponData weapon)
    {
        if (!Owns(weapon))
            return false;

        if (weapon is ShooterData) Profile.EquippedShooter = weapon.id;
        else if (weapon is LazerData) Profile.EquippedLazer = weapon.id;
        else if (weapon is MeleeData) Profile.EquippedMelee = weapon.id;
        else if (weapon is SpecialData) Profile.EquippedSpecial = weapon.id;
        return true;
    }

    // ---- Inventory ----

    public static bool TryBuyConsumable(ConsumableData item)
    {
        if (!CanAfford(item))
            return false;
        if (!Profile.AddToInventory(item.id))
            return false; // no free slot

        Profile.Tokens -= item.tokenCost;
        return true;
    }

    public static bool TryBuyInventorySlot(int tokenCost)
    {
        if (Profile.InventorySlots >= PlayerProfile.MaxInventorySlots || Profile.Tokens < tokenCost)
            return false;

        Profile.Tokens -= tokenCost;
        Profile.InventorySlots++;
        return true;
    }
}
