// ============================================================
// Title:       BossReward
// Version:     1.0
// Written by:  Ashar
// Description: What the player gets for killing a boss: 500 coins,
//              enough xp to go up a level (at least 500) and a few
//              random items (food/drink and ores). Food goes into
//              the backpack, or stored food if it's full. Saves the
//              profile and shows what was won above the boss.
//              Called from BossMob when it dies.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BossReward
{
    private const int Coins = 500;
    private const int MinXp = 500;
    private const int MinItems = 4;            //random items given, 4-6
    private const int MaxItems = 6;
    private const float FoodChance = 0.6f;     //each item: 60% food/drink, 40% ore

    //GIVE
    public static void Give(Vector3 position)
    {
        PlayerProfile p = ProfileManager.Profile;
        List<string> won = new List<string>();

        //coins
        p.Coins += Coins;
        won.Add("+" + Coins + " coins");

        //xp: always enough for one level
        int xp = Mathf.Max(MinXp, p.XpToNextLevel - p.Xp);
        int levels = p.AddXp(xp);
        won.Add("+" + xp + " XP");
        if (levels > 0)
            won.Add("Level up! Now level " + p.Level);

        //random items
        int count = Random.Range(MinItems, MaxItems + 1);
        for (int i = 0; i < count; i++)
        {
            string item = Random.value < FoodChance ? GiveFood(p) : GiveOre(p);
            if (item != null)
            {
                won.Add("+ " + item);
                SessionTracker.AddPickup(item);
            }
        }

        ProfileManager.Save();

        //show it, one line above the other
        for (int i = 0; i < won.Count; i++)
            FloatingText.Show(position + Vector3.up * (7f + i * 0.8f), won[i]);
        Debug.Log("Boss reward: " + string.Join(", ", won));
    }

    //ITEMS
    static string GiveFood(PlayerProfile p)
    {
        ItemDatabase db = ItemDatabase.Instance;
        List<ConsumableData> all = new List<ConsumableData>(db.food);
        all.AddRange(db.drinks);
        if (all.Count == 0)
            return null;

        ConsumableData item = all[Random.Range(0, all.Count)];
        if (!p.AddToInventory(item.id))
            p.AddStoredFood(item.id, 1);   //backpack full: keep it for the Hub
        return item.displayName;
    }

    static string GiveOre(PlayerProfile p)
    {
        List<OreData> ores = ItemDatabase.Instance.ores;
        if (ores.Count == 0)
            return null;

        //rarer ores are further down the list (stone, iron, steel): 60% / 30% / 10%
        float roll = Random.value;
        int index = roll < 0.6f ? 0 : roll < 0.9f ? 1 : 2;
        OreData ore = ores[Mathf.Min(index, ores.Count - 1)];
        int amount = Random.Range(1, 4);   //1-3
        p.AddOre(ore.id, amount);
        return ore.displayName + " x" + amount;
    }
}
