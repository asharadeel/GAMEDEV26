// ============================================================
// Title:       ShooterData
// Version:     2.0
// Written by:  Ashar
// Description: Slot 1 gun (e.g. EM Raygun). The gun itself doesn't
//              shoot anything - the equipped lazer decides what
//              comes out. List the lazers this gun can use.
//              Create > Game > Weapons > Shooter
// ============================================================

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShooter", menuName = "Game/Weapons/Shooter")]
public class ShooterData : WeaponData
{
    [Header("Lazers this gun can fire")]
    public List<LazerData> lazers = new List<LazerData>();
}
