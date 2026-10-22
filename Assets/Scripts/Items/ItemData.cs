// ============================================================
// Title:       ItemData
// Version:     1.2
// Written by:  Ashar
// Updated by:  Ali (v1.2: tokens renamed to coins)
// Description: Base for everything the player can own or buy.
// ============================================================

using UnityEngine;
using UnityEngine.Serialization;

// Base for everything the player can own or buy (weapons, food, drinks...).
public abstract class ItemData : ScriptableObject
{
    [Header("Item")]
    public string id = "";           // unique, used in the save file
    public string displayName = "";
    [FormerlySerializedAs("tokenCost")]
    public int coinCost = 100;       // price in coins (assets made before the rename still load their price)
    public int levelRequired = 1;    // player level needed to buy it
}
