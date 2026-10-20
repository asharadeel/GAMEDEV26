// ============================================================
// Title:       MeleeData
// Version:     1.1
// Written by:  Ashar
// Description: Slot 2. Hits everything within a radius in front
//              of the player. Create > Game > Weapons > Melee
// ============================================================

using UnityEngine;

[CreateAssetMenu(fileName = "NewMelee", menuName = "Game/Weapons/Melee")]
public class MeleeData : WeaponData
{
    [Header("Melee")]
    public float damage = 5f;
    public float fireRate = 2f;      // hits per second
    public float radius = 1.5f;
}
