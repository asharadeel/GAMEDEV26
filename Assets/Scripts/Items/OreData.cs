// ============================================================
// Title:       OreData
// Version:     1.0
// Written by:  Ashar
// Description: A crafting ore (Stone, Iron, Steel...). Dropped by
//              mobs and bought in the shop, kept in the profile's
//              ore pouch (not the food/drink slots).
//              Create > Game > Ore
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOre", menuName = "Game/Ore")]
public class OreData : ItemData
{
    [Header("Ore")]
    public Color colour = Color.grey;   //pickup cube colour
}
