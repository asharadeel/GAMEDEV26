// ============================================================
// Title:       LootDropper
// Version:     1.0
// Written by:  Davud Jabiyev
// Description: Rolls a mob's loot table and spawns a Pickup for
//              every entry that succeeds. Called from Mob when it
//              dies (event driven).
// ============================================================

using UnityEngine;

public static class LootDropper
{
    const float Scatter = 0.8f;          // how far drops spread from the mob

    public static void Drop(MobData data, Vector3 position)
    {
        // Check data isn't null
        if (data == null || data.drops == null) {
            return;
        }

        foreach (LootEntry entry in data.drops)
        {
            // Check LootEntry isn't null
            if (entry == null || entry.item == null) {
                continue;
            }
            
            // Roll chance
            if (Random.value > entry.chance) {
                continue;
            }

            // Spawn pickup
            Vector2 offset = Random.insideUnitCircle * Scatter;
            Pickup.Spawn(position + new Vector3(offset.x, 0f, offset.y), entry.item);
        }
    }
}
