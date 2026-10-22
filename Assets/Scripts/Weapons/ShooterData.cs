// ============================================================
// Title:       ShooterData
// Version:     2.1
// Written by:  Ashar
// Description: Slot 1 gun (e.g. EM Raygun). The gun itself doesn't
//              shoot anything - the equipped lazer decides what
//              comes out. The lazers list is the gun's levels:
//              1st = level 1, 2nd = level 2, 3rd = level 3.
//              Create > Game > Weapons > Shooter
// ============================================================

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShooter", menuName = "Game/Weapons/Shooter")]
public class ShooterData : WeaponData
{
    [Header("Lazer per level (1st = level 1, 2nd = level 2, 3rd = level 3)")]
    public List<LazerData> lazers = new List<LazerData>();

    // The lazer this gun fires at a level
    public LazerData LazerAtLevel(int level)
    {
        if (lazers.Count == 0)
            return null;
        return lazers[Mathf.Clamp(level - 1, 0, lazers.Count - 1)];
    }
}
