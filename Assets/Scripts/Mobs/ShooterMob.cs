// ============================================================
// Title:       ShooterMob
// Version:     1.0
// Written by:  Ashar
// Description: Mob that stays still, turns to face the player when
//              they're in range and shoots at them.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterMob : Mob
{
    private float nextShotTime;

    //AIM AND FIRE
    void Update()
    {
        if (data == null || player == null)
            return;

        if (DistanceToPlayer() > data.shootRange)
            return;

        Vector3 dir = DirectionToPlayer();
        Face(dir);

        if (Time.time >= nextShotTime)
        {
            nextShotTime = Time.time + 1f / data.fireRate;
            Shoot(dir);
        }
    }

    //SHOOTING
    void Shoot(Vector3 dir)
    {
        //player's height so it flies flat into them
        Vector3 pos = transform.position + dir * (myRadius + 0.5f);
        pos.y = player.position.y;

        Projectile.Spawn(data.projectileModel, pos, dir, data.projectileSize, new Vector3(90f, 0f, 0f),
                         data.projectileSpeed, data.attackDamage, data.shootRange + 10f, 0f, true);
    }
}
