// ============================================================
// Title:       ConsumableData
// Version:     1.1
// Written by:  Ashar
// Description: Food or drink that restores stats when used.
//              Refunds for half its price.
// ============================================================

using UnityEngine;

// Food or drink. Goes in an inventory slot and is used up when eaten.
// Make new ones with: Right-click in Project > Create > Game > Consumable
[CreateAssetMenu(fileName = "NewConsumable", menuName = "Game/Consumable")]
public class ConsumableData : ItemData
{
    [Header("Restores")]
    public float hunger = 0f;
    public float thirst = 0f;
    public float stamina = 0f;
    public float health = 0f;

    // Half the price, rounded up
    public int RefundValue
    {
        get { return Mathf.CeilToInt(tokenCost / 2f); }
    }
}
