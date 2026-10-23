// ============================================================
// Title:       Mob
// Version:     1.7
// Written by:  Ashar
// Updated by:  Davud Jabiyev (v1.4: drops loot on death, v1.5: counts kills for achievements)
//              Ali (v1.6: can be slowed, used by the Ice ability)
//              Ashar (v1.7: ores drop through OreDropper)
// Description: Base for every mob. Holds health, shows the health
//              bar, dies at 0 (dropping its loot), and has helpers
//              for finding the player. FollowMob and ShooterMob
//              build on this.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    //key details
    public MobData data;
    public float health;

    public System.Action onDied; //spawner counts kills with this

    //player details
    protected Transform player;
    protected float myRadius; //half the mob's width
    protected float playerRadius;

    private MobHealthBar healthBar;
    private float slowMultiplier = 1f;
    private float slowUntil;

    //STARTING PROCEDURES
    public virtual void Init(MobData mobData, GameObject healthBarPrefab)
    {
        data = mobData;
        health = mobData.health;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            Collider pc = p.GetComponent<Collider>();
            playerRadius = pc != null ? pc.bounds.extents.x : 0.5f;
        }

        Bounds bounds = ModelBounds();
        myRadius = Mathf.Max(bounds.extents.x, bounds.extents.z);

        if (healthBarPrefab != null)
        {
            //bar sits just above the model
            Vector3 pos = new Vector3(transform.position.x, bounds.max.y + 0.6f, transform.position.z);
            healthBar = Instantiate(healthBarPrefab, pos, Quaternion.identity, transform).GetComponent<MobHealthBar>();
        }

        UpdateBar();
    }

    public float AttackDamage
    {
        get { return data != null ? data.attackDamage : 0f; }
    }

    //DAMAGE
    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        UpdateBar();

        if (health <= 0f && onDied != null)
        {
            onDied();
            onDied = null; //only count once
        }
        if (health <= 0f)
        {
            //DROP LOOT
            Vector3 feet = transform.position;
            feet.y = ModelBounds().min.y;
            LootDropper.Drop(data, feet);
            if (!(this is BossMob))
                OreDropper.Drop(feet, data.tier);   //ores, more from harder mobs
            AchievementManager.Add(AchievementStat.MobsKilled, 1);
            SessionTracker.AddKill(data.displayName);
            Destroy(gameObject);
        }
    }

    //SLOW
    public void Slow(float multiplier, float seconds)
    {
        slowMultiplier = multiplier;
        slowUntil = Time.time + seconds;
    }

    //HELPERS
    protected float SpeedMultiplier
    {
        get { return Time.time < slowUntil ? slowMultiplier : 1f; }
    }

    //ignores height
    protected float DistanceToPlayer()
    {
        if (player == null)
            return Mathf.Infinity;

        Vector3 a = transform.position, b = player.position;
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    protected Vector3 DirectionToPlayer()
    {
        if (player == null)
            return Vector3.zero;

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        return dir.normalized;
    }

    protected void Face(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    Bounds ModelBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);
        return bounds;
    }

    void UpdateBar()
    {
        if (healthBar != null && data != null)
            healthBar.Set(health, data.health);
    }
}
