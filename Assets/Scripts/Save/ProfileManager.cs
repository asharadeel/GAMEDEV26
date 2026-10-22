// ============================================================
// Title:       ProfileManager
// Version:     1.6
// Written by:  Ashar
// Updated by:  Ali (v1.6: coins, shooter upgrade bars)
// Description: The current player profile (one of 3 save slots),
//              plus create/edit/delete for the slots and buy/equip/
//              upgrade/refund helpers.
// ============================================================

using System.Collections.Generic;
using UnityEngine;

// The current player profile, usable from any script in any scene:
//   ProfileManager.Profile.Coins += 50;
//   ProfileManager.TryBuyWeapon(someWeapon);
// There are 3 slots, saved as profile_1.json to profile_3.json. The Main Menu
// picks one with LoadSlot. If a scene is played without picking one (testing
// in the editor), the old single save profile.json is used instead.
// Saves when the game closes.
public static class ProfileManager
{
    public const int MaxProfiles = 3;
    public const int NoSlot = -1;

    private static PlayerProfile profile;
    private static int currentSlot = NoSlot;
    private static bool hookedQuit;

    // 0-2, or NoSlot if no profile was picked on the Main Menu
    public static int CurrentSlot
    {
        get { return currentSlot; }
    }

    // True once a profile has been picked on the Main Menu
    public static bool HasProfile
    {
        get { return currentSlot != NoSlot; }
    }

    public static PlayerProfile Profile
    {
        get
        {
            if (profile == null)
            {
                profile = SaveSystem.Load(FileName(currentSlot), new PlayerProfile());
                if (!hookedQuit)
                {
                    Application.quitting += Save;
                    hookedQuit = true;
                }
            }
            return profile;
        }
    }

    // profile_1, profile_2, profile_3 (or profile with no slot)
    public static string FileName(int slot)
    {
        return slot == NoSlot ? "profile" : "profile_" + (slot + 1);
    }

    public static void Save()
    {
        if (profile != null)
            SaveSystem.Save(FileName(currentSlot), profile);
    }

    // Resets progress (stats, coins, items) but keeps the name, passkey and colour
    public static void Delete()
    {
        PlayerProfile fresh = new PlayerProfile();
        if (profile != null)
            fresh.CopyIdentity(profile);
        profile = fresh;
        Save();
    }

    // ---- Slots ----

    public static bool SlotExists(int slot)
    {
        return SaveSystem.Exists(FileName(slot));
    }

    // Reads a slot without switching to it (for the profile cards). Null if empty.
    public static PlayerProfile PeekSlot(int slot)
    {
        if (slot == currentSlot && profile != null)
            return profile;
        if (!SlotExists(slot))
            return null;
        return SaveSystem.Load<PlayerProfile>(FileName(slot), null);
    }

    // Switches to a slot: saves the old one, then the next use of Profile loads the new one
    public static void LoadSlot(int slot)
    {
        Save();
        AchievementManager.Save();
        currentSlot = slot;
        profile = null;
        AchievementManager.Reload();
    }

    public static void CreateProfile(int slot, string name, bool usePasskey, string passkey, Color colour)
    {
        WriteSlot(slot, PlayerProfile.CreateNew(name, usePasskey, passkey, colour));
    }

    public static void EditProfile(int slot, string name, Color colour)
    {
        PlayerProfile p = PeekSlot(slot);
        if (p == null)
            return;
        p.PlayerName = name;
        p.BallColour = colour;
        WriteSlot(slot, p);
    }

    // Deletes the slot's profile and its achievements
    public static void DeleteSlot(int slot)
    {
        SaveSystem.Delete(FileName(slot));
        AchievementManager.DeleteSave(slot);
        if (slot == currentSlot)
        {
            // Stop using it, otherwise it'd be saved again when the game closes
            currentSlot = NoSlot;
            profile = null;
            AchievementManager.Reload();
        }
    }

    static void WriteSlot(int slot, PlayerProfile p)
    {
        SaveSystem.Save(FileName(slot), p);
        if (slot == currentSlot)
            profile = p;
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

        Profile.Coins -= weapon.coinCost;
        Profile.AddOwned(weapon.id);
        return true;
    }

    // Starting items can't be refunded, and neither can what's equipped
    public static bool CanRefund(WeaponData weapon)
    {
        return weapon != null && !weapon.unlocked && Profile.HasBought(weapon.id) && !IsEquipped(weapon);
    }

    // Gives back a third of the price (not the upgrades) and resets the level
    public static bool TryRefund(WeaponData weapon)
    {
        if (!CanRefund(weapon))
            return false;

        Profile.Coins += weapon.RefundValue;
        Profile.RemoveOwned(weapon.id);
        return true;
    }

    // 1 to 3
    public static int GetLevel(WeaponData weapon)
    {
        if (weapon == null)
            return 1;
        return Mathf.Clamp(Profile.GetItemLevel(weapon.id), 1, WeaponData.MaxLevel);
    }

    public static bool CanUpgrade(WeaponData weapon)
    {
        return Owns(weapon) && GetLevel(weapon) < WeaponData.MaxLevel;
    }

    public static bool TryUpgrade(WeaponData weapon)
    {
        if (!CanUpgrade(weapon))
            return false;

        int next = GetLevel(weapon) + 1;
        int cost = weapon.UpgradeCost(next);
        if (Profile.Coins < cost)
            return false;

        Profile.Coins -= cost;
        Profile.SetItemLevel(weapon.id, next);
        return true;
    }

    // ---- Shooter upgrade bars ----

    // How many times a shooter's bar has been upgraded (0 to its max)
    public static int GetLevel(ShooterData shooter, ShooterStat stat)
    {
        if (shooter == null)
            return 0;
        return Mathf.Clamp(Profile.GetShooterLevel(shooter.id, stat), 0, shooter.MaxLevel(stat));
    }

    // A stat with the bought upgrades, e.g. GetStat(raygun, ShooterStat.Damage)
    public static float GetStat(ShooterData shooter, ShooterStat stat)
    {
        return shooter.Value(stat, GetLevel(shooter, stat));
    }

    public static bool CanUpgrade(ShooterData shooter, ShooterStat stat)
    {
        return Owns(shooter) && GetLevel(shooter, stat) < shooter.MaxLevel(stat);
    }

    public static bool TryUpgrade(ShooterData shooter, ShooterStat stat)
    {
        if (!CanUpgrade(shooter, stat))
            return false;

        int next = GetLevel(shooter, stat) + 1;
        int cost = shooter.UpgradeCost(stat, next);
        if (Profile.Coins < cost)
            return false;

        Profile.Coins -= cost;
        Profile.SetShooterLevel(shooter.id, stat, next);
        return true;
    }

    public static bool IsEquipped(WeaponData weapon)
    {
        if (weapon is ShooterData) return Profile.EquippedShooter == weapon.id;
        if (weapon is LazerData) return Profile.EquippedLazer == weapon.id;
        if (weapon is MeleeData) return Profile.EquippedMelee == weapon.id;
        if (weapon is SpecialData) return Profile.EquippedSpecial == weapon.id;
        return false;
    }

    // Enough coins and a high enough level
    public static bool CanAfford(ItemData item)
    {
        return item != null && Profile.Level >= item.levelRequired && Profile.Coins >= item.coinCost;
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

        Profile.Coins -= item.coinCost;
        return true;
    }

    // Sells one back for half its price
    public static bool TryRefundConsumable(ConsumableData item)
    {
        if (item == null || !Profile.RemoveFromInventory(item.id))
            return false;

        Profile.Coins += item.RefundValue;
        return true;
    }

    public static bool TryBuyInventorySlot(int coinCost)
    {
        if (Profile.InventorySlots >= PlayerProfile.MaxInventorySlots || Profile.Coins < coinCost)
            return false;

        Profile.Coins -= coinCost;
        Profile.InventorySlots++;
        return true;
    }
}
