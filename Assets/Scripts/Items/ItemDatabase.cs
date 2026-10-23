// ============================================================
// Title:       ItemDatabase
// Version:     2.2
// Written by:  Ashar
// Updated by:  Ali (v2.1: abilities)
//              Ashar (v2.2: ores and armour)
// Description: The list of every item in the game, split by type.
//              The save file only stores item ids ("lazer",
//              "beans"...), so the game uses this to turn an id
//              back into the real item asset.
//              Must live at Assets/Resources/ItemDatabase.asset.
//              New item? Add it to the right list here.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [Header("Weapons")]
    public List<ShooterData> shooters = new List<ShooterData>();
    public List<LazerData> lazers = new List<LazerData>();
    public List<MeleeData> melee = new List<MeleeData>();

    [Header("Abilities")]
    public List<AbilityData> abilities = new List<AbilityData>();

    [Header("Consumables")]
    public List<ConsumableData> food = new List<ConsumableData>();
    public List<ConsumableData> drinks = new List<ConsumableData>();

    [Header("Crafting and Armor")]
    public List<OreData> ores = new List<OreData>();
    public List<ArmorData> armor = new List<ArmorData>();

    //INSTANCE
    private static ItemDatabase instance;

    public static ItemDatabase Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<ItemDatabase>("ItemDatabase");
            return instance;
        }
    }

    //LOOKUPS
    public List<ItemData> AllItems()
    {
        List<ItemData> all = new List<ItemData>();
        all.AddRange(shooters);
        all.AddRange(lazers);
        all.AddRange(melee);
        all.AddRange(food);
        all.AddRange(drinks);
        all.AddRange(abilities);
        all.AddRange(ores);
        all.AddRange(armor);
        return all;
    }

    public T Get<T>(string id) where T : ItemData
    {
        foreach (ItemData item in AllItems())
        {
            if (item is T match && item.id == id)
                return match;
        }
        return null;
    }

    public List<T> GetAll<T>() where T : ItemData
    {
        List<T> result = new List<T>();
        foreach (ItemData item in AllItems())
        {
            if (item is T match)
                result.Add(match);
        }
        return result;
    }
}
