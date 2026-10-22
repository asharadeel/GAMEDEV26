// ============================================================
// Title:       Projectile
// Version:     1.4
// Written by:  Ashar
// Updated by:  Ali (v1.4: pierce, shots that go through mobs)
// Description: Moves a fired lazer/special and deals damage on hit.
//              Player shots hit everything except the player.
//              Enemy shots (fromEnemy) hit the player and pass
//              through mobs.
// ============================================================

using System.Collections.Generic;
using UnityEngine;

// Flies straight until it hits something or has travelled its max distance.
// radius 0 = hits one thing, radius > 0 = explodes and hits everything nearby.
public class Projectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private float radius;
    private float maxDistance;
    private float travelled;
    private bool fromEnemy;
    private int pierce;
    private Vector3 direction;
    private readonly HashSet<Mob> hitMobs = new HashSet<Mob>();   // a mob made of several shapes only gets hit once

    // Spawns a model as a projectile. Used by the player and by shooter mobs.
    // pierce = how many mobs it goes through before it stops (player shots only)
    public static Projectile Spawn(GameObject model, Vector3 position, Vector3 direction, float size, Vector3 rotationOffset,
                                   float speed, float damage, float distance, float radius, bool fromEnemy, int pierce = 0)
    {
        if (model == null)
            return null;

        Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset);
        GameObject obj = Instantiate(model, position, rotation);
        obj.transform.localScale *= size;

        // Triggers + kinematic rigidbody so it detects hits without being pushed around
        foreach (Collider c in obj.GetComponentsInChildren<Collider>())
            c.isTrigger = true;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
            rb = obj.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        Projectile p = obj.AddComponent<Projectile>();
        p.direction = direction;
        p.speed = speed;
        p.damage = damage;
        p.maxDistance = distance;
        p.radius = radius;
        p.fromEnemy = fromEnemy;
        p.pierce = pierce;
        return p;
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position += direction * step;

        travelled += step;
        if (travelled >= maxDistance)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Projectile>() != null)
            return;

        if (fromEnemy)
        {
            // Enemy shots fly through mobs and only hurt the player
            if (other.GetComponentInParent<Mob>() != null)
                return;
            if (other.CompareTag("Player"))
                Damage.Hit(other, damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Player"))
            return;

        if (radius > 0f)
        {
            Damage.HitArea(transform.position, radius, damage);
            Destroy(gameObject);
            return;
        }

        Mob mob = other.GetComponentInParent<Mob>();
        if (mob != null && !hitMobs.Add(mob))
            return;   // another part of a mob it already hit

        Damage.Hit(other, damage);

        // Piercing shots keep going through mobs (walls still stop them)
        if (mob != null && pierce > 0)
        {
            pierce--;
            return;
        }
        Destroy(gameObject);
    }
}
