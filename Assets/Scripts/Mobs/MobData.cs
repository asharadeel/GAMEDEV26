// ============================================================
// Title:       MobData
// Version:     1.2
// Written by:  Ashar
// Description: Profile for a mob type: its stats, its model and how
//              it behaves (Follow = chases the player, Shooter =
//              stays still and shoots). Create > Game > Mob
// ============================================================

using UnityEngine;

public enum MobType
{
    Follow,     // FollowMob: wanders, chases the player, hits up close
    Shooter     // ShooterMob: stays still, shoots at the player
}

[CreateAssetMenu(fileName = "NewMob", menuName = "Game/Mob")]
public class MobData : ScriptableObject
{
    [Header("Mob")]
    public string id = "";
    public string displayName = "";
    public string shortName = "";       // used in the Mobs Left tracker, e.g. "Lvl 1"
    public GameObject model;             // prefab that gets spawned
    public MobType type = MobType.Follow;

    [Header("Stats")]
    public float health = 100f;
    public float attackDamage = 10f;     // per hit (Follow) or per shot (Shooter)

    [Header("Follow")]
    public float moveSpeed = 3f;
    public float chaseRange = 25f;       // starts chasing when the player is this close
    public float attackRange = 0.5f;     // gap between mob and player to hit
    public float attackCooldown = 1f;    // seconds between hits

    [Header("Shooter")]
    public float shootRange = 20f;       // shoots when the player is this close
    public float fireRate = 0.5f;        // shots per second
    public GameObject projectileModel;
    public float projectileSpeed = 15f;
    public float projectileSize = 1f;
}
