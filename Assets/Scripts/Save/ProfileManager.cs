// ============================================================
// Title:       ProfileManager
// Version:     1.7
// Written by:  Ashar
// Updated by:  Ali (v1.6: coins, shooter upgrade bars, v1.7: abilities
//              and skill tree)
// Description: The current player profile (one of 3 save slots),
//              plus create/edit/delete for the slots and buy/equip/
//              upgrade/refund helpers, and the abilities' skill tree.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PROFILE MANAGER - e.g. ProfileManager.Profile.Coins += 50;
//no slot picked (testing in editor) uses the old profile.json. saves on quit
public static class ProfileManager
{
    public const int MaxProfiles = 3;
    public const int NoSlot = -1;

    //current profile
    private static PlayerProfile profile;
    private static int currentSlot = NoSlot;
    private static bool hookedQuit;

    // 0-2, or NoSlot
    public static int CurrentSlot
    {
        get { return currentSlot; }
    }

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

    public static string FileName(int slot)
    {
        return slot == NoSlot ? "profile" : "profile_" + (slot + 1);
    }

    public static void Save()
    {
        if (profile != null)
            SaveSystem.Save(FileName(currentSlot), profile);
    }

    // resets progress but keeps the name, passkey and colour
    public static void Delete()
    {
        PlayerProfile fresh = new PlayerProfile();
        if (profile != null)
            fresh.CopyIdentity(profile);
        profile = fresh;
        Save();
    }

    //SLOTS

    public static bool SlotExists(int slot)
    {
        return SaveSystem.Exists(FileName(slot));
    }

    // reads a slot without switching to it, null if empty
    public static PlayerProfile PeekSlot(int slot)
    {
        if (slot == currentSlot && profile != null)
            return profile;
        if (!SlotExists(slot))
            return null;
        return SaveSystem.Load<PlayerProfile>(FileName(slot), null);
    }

    // saves the old slot, next use of Profile loads the new one
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

    public static void DeleteSlot(int slot)
    {
        SaveSystem.Delete(FileName(slot));
        AchievementManager.DeleteSave(slot);
        if (slot == currentSlot)
        {
            // stop using it or it gets saved again on quit
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

    //WEAPONS

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

    // starting items and equipped items can't be refunded
    public static bool CanRefund(WeaponData weapon)
    {
        return weapon != null && !weapon.unlocked && Profile.HasBought(weapon.id) && !IsEquipped(weapon);
    }

    // a third of the price back (not the upgrades)
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

    //SHOOTER UPGRADE BARS

    // 0 to its max
    public static int GetLevel(ShooterData shooter, ShooterStat stat)
    {
        if (shooter == null)
            return 0;
        return Mathf.Clamp(Profile.GetShooterLevel(shooter.id, stat), 0, shooter.MaxLevel(stat));
    }

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

    // enough coins and a high enough level
    public static bool CanAfford(ItemData item)
    {
        return item != null && Profile.Level >= item.levelRequired && Profile.Coins >= item.coinCost;
    }

    // e.g. GetOwned<LazerData>()
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

    // empties the slot if it's the one equipped
    public static bool Unequip(WeaponData weapon)
    {
        if (weapon == null || !IsEquipped(weapon))
            return false;

        if (weapon is ShooterData) Profile.EquippedShooter = "";
        else if (weapon is LazerData) Profile.EquippedLazer = "";
        else if (weapon is MeleeData) Profile.EquippedMelee = "";
        else if (weapon is SpecialData) Profile.EquippedSpecial = "";
        return true;
    }

    //ABILITIES AND SKILL TREE

    // abilities are free at base stats, nodes unlock once bought
    public static bool IsUnlocked(AbilityData ability, int node)
    {
        return Profile.HasSkillNode(ability.NodeId(node));
    }

    // not unlocked yet and its parent node is (or no parent)
    public static bool CanUnlock(AbilityData ability, int node)
    {
        int parent = ability.nodes[node].parent;
        return !IsUnlocked(ability, node) && (parent < 0 || IsUnlocked(ability, parent));
    }

    public static bool TryUnlock(AbilityData ability, int node)
    {
        if (!CanUnlock(ability, node) || Profile.Coins < ability.nodes[node].cost)
            return false;

        Profile.Coins -= ability.nodes[node].cost;
        Profile.AddSkillNode(ability.NodeId(node));
        return true;
    }

    public static float GetStat(AbilityData ability, AbilityStat stat)
    {
        float value = ability.BaseValue(stat);
        for (int i = 0; i < ability.nodes.Count; i++)
        {
            if (ability.nodes[i].stat == stat && IsUnlocked(ability, i))
                value += ability.nodes[i].amount;
        }
        return value;
    }

    // 0 = Q, 1 = E, or null
    public static AbilityData GetAbility(int slot)
    {
        return ItemDatabase.Instance.Get<AbilityData>(Profile.GetAbility(slot));
    }

    // -1 if not on Q or E
    public static int AbilitySlot(AbilityData ability)
    {
        for (int slot = 0; slot < 2; slot++)
        {
            if (Profile.GetAbility(slot) == ability.id)
                return slot;
        }
        return -1;
    }

    // moves it if it was on the other key
    public static void EquipAbility(AbilityData ability, int slot)
    {
        int other = 1 - slot;
        if (Profile.GetAbility(other) == ability.id)
            Profile.SetAbility(other, "");
        Profile.SetAbility(slot, ability.id);
    }

    public static void UnequipAbility(int slot)
    {
        Profile.SetAbility(slot, "");
    }

    //INVENTORY

    public static bool TryBuyConsumable(ConsumableData item)
    {
        if (!CanAfford(item))
            return false;
        if (!Profile.AddToInventory(item.id))
            return false; // no free slot

        Profile.Coins -= item.coinCost;
        return true;
    }

    // sells one back for half price
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
