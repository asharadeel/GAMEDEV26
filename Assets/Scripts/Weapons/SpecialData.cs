// ============================================================
// Title:       SpecialData
// Version:     2.0
// Written by:  Ashar
// Description: Slot 3. Fires a projectile that explodes on hit,
//              damaging everything in its radius. Has 3 levels
//              (1 = as bought, 2 and 3 = upgrades), each with its
//              own model and stats.
//              Create > Game > Weapons > Special
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SPECIAL LEVEL
[System.Serializable]
public class SpecialLevel
{
    public GameObject model;         //the projectile
    public float damage = 50f;
    public float fireRate = 0.5f;    //shots per second
    public float speed = 10f;
    public float radius = 3f;        //explosion radius
    public float distance = 50f;     //travel distance before it disappears
    public float size = 1f;
    public Vector3 rotationOffset = new Vector3(90f, 0f, 0f);
}

[CreateAssetMenu(fileName = "NewSpecial", menuName = "Game/Weapons/Special")]
public class SpecialData : WeaponData
{
    [Header("Levels (1st = level 1, 2nd = level 2, 3rd = level 3)")]
    public SpecialLevel[] levels = { new SpecialLevel(), new SpecialLevel(), new SpecialLevel() };

    public SpecialLevel AtLevel(int level)
    {
        return levels[Mathf.Clamp(level - 1, 0, levels.Length - 1)];
    }
}
