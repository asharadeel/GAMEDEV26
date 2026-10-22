// ============================================================
// Title:       WeaponData
// Version:     1.3
// Written by:  Ashar
// Description: Base for all weapon gear (shooters, lazers, melee,
//              specials). Holds unlock info, the model, and the
//              upgrade and refund prices (3 levels per weapon).
// ============================================================

using UnityEngine;

public abstract class WeaponData : ItemData
{
    [Header("Unlock")]
    [Tooltip("Ticked = every player starts with it, no need to buy")]
    public bool unlocked = false;

    [Header("Model")]
    public GameObject model;

    public const int MaxLevel = 3;

    // Level 1 comes with the item, level 2 costs half its price, level 3 costs 1.5x its price
    public int UpgradeCost(int toLevel)
    {
        if (toLevel == 2)
            return Mathf.CeilToInt(coinCost * 0.5f);
        if (toLevel == 3)
            return Mathf.CeilToInt(coinCost * 1.5f);
        return 0;
    }

    // A third of the price, rounded up. Always the same: upgrades (levels or shooter bars) never change it and aren't refunded
    public int RefundValue
    {
        get { return Mathf.CeilToInt(coinCost / 3f); }
    }
}
