// ============================================================
// Title:       MobData
// Version:     1.3
// Written by:  Ashar
// Updated by:  Davud Jabiyev (v1.3: added loot drops)
// Description: Profile for a mob type: its stats, its model and how
//              it behaves (Follow = chases the player, Shooter =
//              stays still and shoots). Also lists what it can drop
//              when killed. Create > Game > Mob
// ============================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MobType
{
    Follow, //chases and hits up close
    Shooter //stays still and shoots
}

[Serializable]
public class LootEntry
{
    public ConsumableData item;
    [Range(0f, 1f)]
    public float chance = 0.25f; //0 never, 1 always
}

[CreateAssetMenu(fileName = "NewMob", menuName = "Game/Mob")]
public class MobData : ScriptableObject
{
    [Header("Mob")]
    public string id = "";
    public string displayName = "";
    public string shortName = ""; //Mobs Left tracker, e.g. "Lvl 1"
    public GameObject model;
    public MobType type = MobType.Follow;

    [Header("Stats")]
    public float health = 100f;
    public float attackDamage = 10f; //per hit or per shot

    [Header("Follow")]
    public float moveSpeed = 3f;
    public float chaseRange = 25f;
    public float attackRange = 0.5f; //gap between mob and player
    public float attackCooldown = 1f; //seconds

    [Header("Shooter")]
    public float shootRange = 20f;
    public float fireRate = 0.5f; //shots per second
    public GameObject projectileModel;
    public float projectileSpeed = 15f;
    public float projectileSize = 1f;

    [Header("Loot")]
    public LootEntry[] drops; //each rolled separately
}
