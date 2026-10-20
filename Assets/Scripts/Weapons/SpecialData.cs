// ============================================================
// Title:       SpecialData
// Version:     1.1
// Written by:  Ashar
// Description: Slot 3. Fires a projectile that explodes on hit,
//              damaging everything in its radius.
//              Create > Game > Weapons > Special
// ============================================================

using UnityEngine;

[CreateAssetMenu(fileName = "NewSpecial", menuName = "Game/Weapons/Special")]
public class SpecialData : WeaponData
{
    [Header("Special")]
    public float damage = 50f;
    public float fireRate = 0.5f;    // shots per second
    public float speed = 10f;
    public float radius = 3f;        // explosion radius
    public float distance = 50f;     // how far it travels before it disappears
    public float size = 1f;
    public Vector3 rotationOffset = new Vector3(90f, 0f, 0f);
}
