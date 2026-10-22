// ============================================================
// Title:       ProfileManager
// Version:     1.3
// Written by:  Ashar
// Description: The current player profile (one of 3 save slots),
//              plus create/edit/delete for the slots and buy/equip
//              helpers.
// ============================================================

using System.Collections.Generic;
using UnityEngine;

// The current player profile, usable from any script in any scene:
//   ProfileManager.Profile.Tokens += 50;
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

    // Resets progress (stats, tokens, items) but keeps the name, passkey and colour
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
