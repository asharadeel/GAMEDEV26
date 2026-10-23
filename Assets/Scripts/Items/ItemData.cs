// ============================================================
// Title:       ItemData
// Version:     1.2
// Written by:  Ashar
// Updated by:  Ali (v1.2: tokens renamed to coins)
// Description: Base for everything the player can own or buy.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ItemData : ScriptableObject
{
    [Header("Item")]
    public string id = ""; //unique, used in save file
    public string displayName = "";
    [FormerlySerializedAs("tokenCost")]
    public int coinCost = 100; //old tokenCost assets still load
    public int levelRequired = 1;
}
