// ============================================================
// Title:       ConsumableData
// Version:     1.2
// Written by:  Ashar
// Description: Food or drink that restores stats when used.
//              Refunds for half its price. Model = what it looks like
//              when a mob drops it.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Game/Consumable")]
public class ConsumableData : ItemData
{
    [Header("Dropped")]
    public GameObject model;   //Models/Consumables, empty = plain cube

    [Header("Restores")]
    public float hunger = 0f;
    public float thirst = 0f;
    public float stamina = 0f;
    public float health = 0f;

    //half price, rounded up
    public int RefundValue
    {
        get { return Mathf.CeilToInt(coinCost / 2f); }
    }
}
