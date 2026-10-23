// ============================================================
// Title:       ArmorData
// Version:     1.0
// Written by:  Ashar
// Description: One full armour piece. Gives a shield bar above
//              health that takes damage first. Buy several: when
//              one breaks the next goes on after Replace Delay
//              (better armour = shorter wait). Repair it for coins.
//              Create > Game > Armor
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArmor", menuName = "Game/Armor")]
public class ArmorData : ItemData
{
    [Header("Armor")]
    public GameObject model;
    public float healthBonus = 25f;     //size of the shield bar
    public float replaceDelay = 10f;    //seconds before the next one goes on after a break
    public int repairCost = 20;         //coins to fill the shield back up
}
