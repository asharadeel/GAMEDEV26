// ============================================================
// Title:       OreData
// Version:     1.1
// Written by:  Ashar
// Description: A crafting ore (Stone, Iron, Steel...). Only dropped
//              by mobs (OreDropper), used at the Blacksmith. Kept in
//              the profile's ore pouch (not the food/drink slots).
//              Create > Game > Ore
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOre", menuName = "Game/Ore")]
public class OreData : ItemData
{
    [Header("Ore")]
    public GameObject model;            //Models/Ores sphere, empty = plain cube
    public Color colour = Color.grey;   //plain cube colour when there's no model
}
