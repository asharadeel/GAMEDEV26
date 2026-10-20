// ============================================================
// Title:       LazerData
// Version:     1.0
// Written by:  Ashar
// Description: A bullet/upgrade for a shooter. Holds everything
//              about the shot: damage, fire rate, speed, range.
//              Add it to a shooter's Lazers list to use it.
//              Create > Game > Weapons > Lazer
// ============================================================

using UnityEngine;

[CreateAssetMenu(fileName = "NewLazer", menuName = "Game/Weapons/Lazer")]
public class LazerData : WeaponData
{
    [Header("Lazer")]
    public float damage = 10f;
    public float fireRate = 4f;      // shots per second
    public float speed = 20f;
    public float distance = 50f;     // how far it travels before it disappears
    public float size = 1f;          // scale multiplier on the model
    public Vector3 rotationOffset = new Vector3(90f, 0f, 0f); // tilts the model to face travel direction
}
