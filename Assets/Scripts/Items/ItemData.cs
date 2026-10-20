// ============================================================
// Title:       ItemData
// Version:     1.1
// Written by:  Ashar
// Description: Base for everything the player can own or buy.
// ============================================================

using UnityEngine;

// Base for everything the player can own or buy (weapons, food, drinks...).
public abstract class ItemData : ScriptableObject
{
    [Header("Item")]
    public string id = "";           // unique, used in the save file
    public string displayName = "";
    public int tokenCost = 100;      // price in tokens
    public int levelRequired = 1;    // player level needed to buy it
}
