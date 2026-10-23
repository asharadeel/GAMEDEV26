// ============================================================
// Title:       FollowMob
// Version:     1.0
// Written by:  Ashar
// Description: Mob that wanders around until the player comes within
//              chase range, then walks at them and hits them when
//              close. Uses a Rigidbody so walls block it.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMob : Mob
{
    [Header("Wander")]
    public float wanderSpeedMultiplier = 0.4f;
    public float wanderChangeEvery = 3f; //seconds

    //movement details
    private Rigidbody rb;
    private Vector3 wanderDirection;
    private float nextWanderChange;
    private float nextAttackTime;

    //STARTING PROCEDURES
    public override void Init(MobData mobData, GameObject healthBarPrefab)
    {
        base.Init(mobData, healthBarPrefab);

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.freezeRotation = true; //Face() handles turning
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    //MOVEMENT
    void FixedUpdate()
    {
        if (data == null || rb == null)
            return;

        float distance = DistanceToPlayer();
        Vector3 move;
        float speed = data.moveSpeed * SpeedMultiplier;

        if (distance <= data.chaseRange)
        {
            move = DirectionToPlayer();
            Face(move);

            //in range, stop and attack
            if (distance <= myRadius + playerRadius + data.attackRange)
            {
                move = Vector3.zero;
                TryAttack();
            }
        }
        else
        {
            move = Wander();
            speed *= wanderSpeedMultiplier;
        }

        rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);
    }

    //WANDER
    Vector3 Wander()
    {
        if (Time.time >= nextWanderChange)
        {
            nextWanderChange = Time.time + wanderChangeEvery;
            Vector2 r = Random.insideUnitCircle;
            wanderDirection = Random.value < 0.3f ? Vector3.zero : new Vector3(r.x, 0f, r.y).normalized;
            Face(wanderDirection);
        }
        return wanderDirection;
    }

    //ATTACK
    void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + data.attackCooldown;
        player.SendMessage("TakeDamage", data.attackDamage, SendMessageOptions.DontRequireReceiver);
    }
}
