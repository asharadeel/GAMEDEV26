// ============================================================
// Title:       WeaponData
// Version:     1.2
// Written by:  Ashar
// Description: Base for all weapon gear (shooters, lazers, melee,
//              specials). Holds unlock info and the model.
// ============================================================

using UnityEngine;

public abstract class WeaponData : ItemData
{
    [Header("Unlock")]
    [Tooltip("Ticked = every player starts with it, no need to buy")]
    public bool unlocked = false;

    [Header("Model")]
    public GameObject model;
}
