// ============================================================
// Title:       ShooterData
// Version:     3.0
// Written by:  Ashar
// Updated by:  Ali (v3.0: separate upgrade bars instead of levels)
// Description: Slot 1 gun (e.g. EM Raygun). Each part of the gun
//              is upgraded on its own bar in the shop:
//              Lazer (which lazer it fires, from the Lazers list),
//              Damage, Fire Rate, Bullet Speed and Reload Time.
//              Fires magazineSize shots, then reloads.
//              Create > Game > Weapons > Shooter
// ============================================================

using System.Collections.Generic;
using UnityEngine;

// The shooter's upgrade bars, in the order the shop shows them
public enum ShooterStat { Lazer, Damage, FireRate, BulletSpeed, ReloadTime }

// One upgradeable stat: value = start + perLevel x upgrades bought
[System.Serializable]
public class ShooterUpgrade
{
    public float start;
    public float perLevel;           // negative to go down (reload time)
    public int firstCost = 20;       // each upgrade costs this much more than the last (20, 40, 60...)
}

[CreateAssetMenu(fileName = "NewShooter", menuName = "Game/Weapons/Shooter")]
public class ShooterData : WeaponData
{
    public const int MaxUpgrades = 5;   // blocks on each stat bar

    [Header("Magazine")]
    public int magazineSize = 20;       // shots before it has to reload

    [Header("Upgrade bars (value with 0 upgrades + change per upgrade)")]
    public ShooterUpgrade damage = new ShooterUpgrade { start = 10f, perLevel = 3f };
    public ShooterUpgrade fireRate = new ShooterUpgrade { start = 4f, perLevel = 0.8f };       // shots per second
    public ShooterUpgrade bulletSpeed = new ShooterUpgrade { start = 20f, perLevel = 4f };
    public ShooterUpgrade reloadTime = new ShooterUpgrade { start = 2f, perLevel = -0.3f };    // seconds

    [Header("Lazer bar (1st = starting lazer, each next one costs its Coin Cost)")]
    public List<LazerData> lazers = new List<LazerData>();

    // How many upgrades a bar has (the lazer bar has one per lazer after the first)
    public int MaxLevel(ShooterStat stat)
    {
        if (stat == ShooterStat.Lazer)
            return Mathf.Max(0, lazers.Count - 1);
        return MaxUpgrades;
    }

    // Price of the upgrade that takes a bar to toLevel
    public int UpgradeCost(ShooterStat stat, int toLevel)
    {
        if (stat == ShooterStat.Lazer)
            return lazers[Mathf.Clamp(toLevel, 0, lazers.Count - 1)].coinCost;
        return Upgrade(stat).firstCost * toLevel;
    }

    // A stat's value after some upgrades (not for the lazer bar)
    public float Value(ShooterStat stat, int level)
    {
        ShooterUpgrade u = Upgrade(stat);
        return u.start + u.perLevel * level;
    }

    // The lazer at a lazer bar level
    public LazerData LazerAt(int level)
    {
        if (lazers.Count == 0)
            return null;
        return lazers[Mathf.Clamp(level, 0, lazers.Count - 1)];
    }

    ShooterUpgrade Upgrade(ShooterStat stat)
    {
        switch (stat)
        {
            case ShooterStat.Damage: return damage;
            case ShooterStat.FireRate: return fireRate;
            case ShooterStat.BulletSpeed: return bulletSpeed;
            default: return reloadTime;
        }
    }
}
