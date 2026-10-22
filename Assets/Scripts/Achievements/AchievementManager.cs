// ============================================================
// Title:       AchievementManager
// Version:     1.1
// Written by:  Davud Jabiyev
// Updated by:  Ashar (v1.1: one save per profile)
// Description: Keeps a counter for each AchievementStat and unlocks
//              achievements when their goal is reached. Other scripts
//              just call AchievementManager.Add(stat, 1) when
//              something happens (mob killed, item picked up...).
//              Progress is saved per profile (achievements_1.json
//              to achievements_3.json, or achievements.json when no
//              profile was picked), see ProfileManager.
//              OnUnlocked event is called for ui updates.
// ============================================================

using System.Collections.Generic;
using UnityEngine;

// What gets written to the save file
[System.Serializable]
public class AchievementSave
{
    public int[] counts = new int[0];                        // stat counts (AchievementStat), enum order
    public List<string> unlocked = new List<string>();       // ids of unlocked achievements
}

public static class AchievementManager
{
    // All the achievements in the game list
    public static List<AchievementData> All = new List<AchievementData>();

    // Stats
    private static Dictionary<AchievementStat, int> counters = new Dictionary<AchievementStat, int>();

    // Ids of the achievements unlocked
    private static List<string> unlocked = new List<string>();

    // Saving
    private static bool loaded = false;
    private static bool hookedQuit = false;

    // Event that other scripts (e.g. ui) can listen to
    public static event System.Action<AchievementData> OnUnlocked;

    // This will fill the achievement list and load saved progress
    private static void Setup()
    {
        // Likely already setup
        if (All.Count > 0) {
            return;
        }

        // Load from saved progress
        Load();

        // Kills
        All.Add(new AchievementData("kill1", "First Blood", "Kill 1 mob", AchievementStat.MobsKilled, 1));
        All.Add(new AchievementData("kill10", "Slayer", "Kill 10 mobs", AchievementStat.MobsKilled, 10));
        All.Add(new AchievementData("kill50", "Exterminator", "Kill 50 mobs", AchievementStat.MobsKilled, 50));

        // Loot
        All.Add(new AchievementData("pick1", "Finders Keepers", "Pick up 1 item", AchievementStat.ItemsPickedUp, 1));
        All.Add(new AchievementData("pick10", "Scavenger", "Pick up 10 items", AchievementStat.ItemsPickedUp, 10));
        All.Add(new AchievementData("pick30", "Hoarder", "Pick up 30 items", AchievementStat.ItemsPickedUp, 30));

        // Eating and drinking
        All.Add(new AchievementData("eat1", "Snack Time", "Eat or drink 1 item", AchievementStat.ItemsConsumed, 1));
        All.Add(new AchievementData("drink5", "Thirsty Work", "Drink 5 drinks", AchievementStat.DrinksConsumed, 5));
        All.Add(new AchievementData("food5", "Hungry Hunter", "Eat 5 food items", AchievementStat.FoodEaten, 5));
        All.Add(new AchievementData("eat20", "Well Fed", "Eat or drink 20 items", AchievementStat.ItemsConsumed, 20));

        // Full inventory
        All.Add(new AchievementData("full1", "Bag Full", "Try to pick up with a full inventory", AchievementStat.DropsMissed, 1));
        All.Add(new AchievementData("full5", "Pack Mule", "Walk over 5 pickups with a full inventory", AchievementStat.DropsMissed, 5));
    }

    // Loads in saved progress from the file
    private static void Load()
    {
        // Already loaded
        if (loaded) {
            return;
        }
        
        // Set loaded
        loaded = true;

        // Save when the game closes (only hook it once)
        if (!hookedQuit) {
            Application.quitting += Save;
            hookedQuit = true;
        }

        // Load the current profile's file, with fallback new AchievementSave()
        AchievementSave data = SaveSystem.Load(FileName(ProfileManager.CurrentSlot), new AchievementSave());

        // Copy loaded counts back into the dictionary
        for (int i = 0; i < data.counts.Length; i++) {
            counters[(AchievementStat)i] = data.counts[i];
        }
        unlocked = data.unlocked;
    }

    // Writes the progress to the file
    public static void Save()
    {
        if (!loaded) {
            return;
        }

        AchievementSave data = new AchievementSave();
        int statCount = System.Enum.GetValues(typeof(AchievementStat)).Length;
        data.counts = new int[statCount];
        for (int i = 0; i < statCount; i++) {
            data.counts[i] = GetCount((AchievementStat)i);
        }
        data.unlocked = unlocked;

        SaveSystem.Save(FileName(ProfileManager.CurrentSlot), data);
    }

    // achievements_1, achievements_2, achievements_3 (or achievements with no profile)
    public static string FileName(int slot)
    {
        if (slot == ProfileManager.NoSlot) {
            return "achievements";
        }
        return "achievements_" + (slot + 1);
    }

    // Called by ProfileManager when the profile changes: forget the old
    // progress and load the new profile's (doesn't save, ProfileManager does that first)
    public static void Reload()
    {
        counters.Clear();
        unlocked = new List<string>();
        loaded = false;
        Load();
    }

    // Deletes a profile's achievements file
    public static void DeleteSave(int slot)
    {
        SaveSystem.Delete(FileName(slot));
    }

    // This is called to increment stat
    public static void Add(AchievementStat stat, int amount)
    {
        Setup();

        // Add to the counter (start at 0 if it does not exist yet)
        if (!counters.ContainsKey(stat)) {
            counters[stat] = 0;
        }
        counters[stat] += amount;

        CheckAll();
    }

    // Returns count of a stat
    public static int GetCount(AchievementStat stat)
    {
        if (counters.ContainsKey(stat)) {
            return counters[stat];
        }
        return 0;
    }

    // Returns if achievement is unlcoked
    public static bool IsUnlocked(string id)
    {
        return unlocked.Contains(id);
    }

    // Run to update achievement unlocks with stat changes
    // (Unlocks every achievement whose goal was reached)
    private static void CheckAll()
    {
        // Iterate through achievements
        foreach (AchievementData a in All)
        {
            // Skip the ones we already unlocked
            if (IsUnlocked(a.id)) {
                continue;
            }

            // Check respective stat, unlock if passed goal
            if (GetCount(a.stat) >= a.goal) {
                unlocked.Add(a.id);

                // Tell the popup ui (call event)
                if (OnUnlocked != null) {
                    OnUnlocked(a);
                }

                // Save to file
                Save();
            }
        }
    }
}
