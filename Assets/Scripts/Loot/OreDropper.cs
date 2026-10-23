// ============================================================
// Title:       OreDropper
// Version:     1.0
// Written by:  Ashar
// Description: One place for ore drops, so the chances aren't
//              repeated in every mob's loot list. Every mob kill
//              (not the boss) rolls each ore separately, and
//              harder mobs (higher tier in MobData) drop them more.
//              Called from Mob when it dies.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OreDropper
{
    //chances per kill (0 never, 1 always)
    private const float StoneChance = 0.15f;   //15%
    private const float IronChance = 0.05f;    //5%
    private const float SteelChance = 0.02f;   //2%

    //harder mobs: chances x this (tier 1, 2, 3)
    private static readonly float[] TierMultiplier = { 1f, 1.5f, 2f };   //tier 3 = 30% / 10% / 4%

    private const float Scatter = 0.8f;        //how far drops spread from the mob

    //DROP
    public static void Drop(Vector3 position, int tier)
    {
        float m = TierMultiplier[Mathf.Clamp(tier, 1, TierMultiplier.Length) - 1];
        Roll("stone", StoneChance * m, position);
        Roll("iron", IronChance * m, position);
        Roll("steel", SteelChance * m, position);
    }

    static void Roll(string oreId, float chance, Vector3 position)
    {
        if (Random.value > chance)
            return;

        OreData ore = ItemDatabase.Instance.Get<OreData>(oreId);
        if (ore == null)
            return;

        Vector2 offset = Random.insideUnitCircle * Scatter;
        Pickup.Spawn(position + new Vector3(offset.x, 0f, offset.y), ore);
    }
}
