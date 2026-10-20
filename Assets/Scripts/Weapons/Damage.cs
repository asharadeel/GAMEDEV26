// ============================================================
// Title:       Damage
// Version:     1.0
// Written by:  Ashar
// Description: Shared helpers for dealing damage. Sends
//              TakeDamage(float) up to whatever owns the collider
//              (e.g. a Mob on the model's root).
// ============================================================

using System.Collections.Generic;
using UnityEngine;

public static class Damage
{
    // Damage one thing
    public static void Hit(Collider target, float amount)
    {
        target.SendMessageUpwards("TakeDamage", amount, SendMessageOptions.DontRequireReceiver);
    }

    // Damage everything in a radius once each (a mob made of several shapes only gets hit once)
    public static void HitArea(Vector3 centre, float radius, float amount)
    {
        HashSet<Object> done = new HashSet<Object>();

        foreach (Collider hit in Physics.OverlapSphere(centre, radius))
        {
            if (hit.CompareTag("Player"))
                continue;

            Mob mob = hit.GetComponentInParent<Mob>();
            Object key = mob != null ? (Object)mob : hit;
            if (done.Add(key))
                Hit(hit, amount);
        }
    }
}
