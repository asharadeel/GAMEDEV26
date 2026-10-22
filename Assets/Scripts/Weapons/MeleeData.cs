// ============================================================
// Title:       MeleeData
// Version:     2.0
// Written by:  Ashar
// Description: Slot 2. Hits everything within a radius in front
//              of the player. Has 3 levels (1 = as bought, 2 and
//              3 = upgrades), each with its own model and stats.
//              Create > Game > Weapons > Melee
// ============================================================

using UnityEngine;

// One level of a melee weapon
[System.Serializable]
public class MeleeLevel
{
    public GameObject model;         // flashed briefly on a hit (can be empty)
    public float damage = 5f;
    public float fireRate = 2f;      // hits per second
    public float radius = 1.5f;
}

[CreateAssetMenu(fileName = "NewMelee", menuName = "Game/Weapons/Melee")]
public class MeleeData : WeaponData
{
    [Header("Levels (1st = level 1, 2nd = level 2, 3rd = level 3)")]
    public MeleeLevel[] levels = { new MeleeLevel(), new MeleeLevel(), new MeleeLevel() };

    public MeleeLevel AtLevel(int level)
    {
        return levels[Mathf.Clamp(level - 1, 0, levels.Length - 1)];
    }
}
