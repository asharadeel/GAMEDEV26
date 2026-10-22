// ============================================================
// Title:       PlayerAbilities
// Version:     1.1
// Written by:  Ali
// Updated by:  cthullu96 (v1.1: input system package)
//              ai use: input system change written with claude (anthropic), owner cthullu96
// Description: Uses the abilities equipped on Q and E (picked in
//              the Hub's Inventory), each with its own cooldown.
//              Stats come from the ability plus its unlocked skill
//              tree nodes. Near a door, E opens the door instead.
//              Fire:      a fireball that explodes where it lands
//              Ice:       a frost nova that hurts and slows mobs around you
//              Lightning: hits the nearest mob and chains to others
// ============================================================

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCombat))]
public class PlayerAbilities : MonoBehaviour
{
    [Header("Fire")]
    public float fireballSpeed = 18f;
    public float fireballDistance = 30f;

    [Header("Ice")]
    [Range(0f, 1f)] public float iceSlow = 0.5f;   // mobs move at this x speed while slowed

    private PlayerCombat combat;
    private readonly Dictionary<string, float> readyAt = new Dictionary<string, float>();   // ability id -> time it can be used again

    void Start()
    {
        combat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        if (PlayerController.uiEnabled)
            return;

        if (GameInput.AbilityQ.WasPressedThisFrame())
            Use(0);
        if (GameInput.AbilityE.WasPressedThisFrame() && !DoorInteract.PlayerNearDoor)
            Use(1);
    }

    // 0 = Q, 1 = E
    public void Use(int slot)
    {
        AbilityData ability = ProfileManager.GetAbility(slot);
        if (ability == null || CooldownLeft(ability) > 0f)
            return;

        float damage = ProfileManager.GetStat(ability, AbilityStat.Damage);
        float radius = ProfileManager.GetStat(ability, AbilityStat.Radius);
        float extra = ProfileManager.GetStat(ability, AbilityStat.Extra);

        bool used;
        if (ability.type == AbilityType.Fire)
            used = Fireball(ability, damage, radius);
        else if (ability.type == AbilityType.Ice)
            used = FrostNova(ability, damage, radius, extra);
        else
            used = ChainLightning(ability, damage, radius, Mathf.RoundToInt(extra));

        if (used)
            readyAt[ability.id] = Time.time + Mathf.Max(0.5f, ProfileManager.GetStat(ability, AbilityStat.Cooldown));
    }

    public float CooldownLeft(AbilityData ability)
    {
        return readyAt.TryGetValue(ability.id, out float t) ? Mathf.Max(0f, t - Time.time) : 0f;
    }

    // "Q  Fire  Ready" / "E  Ice  3.2s" for the HUD, "" if nothing is on that key
    public string SlotLabel(int slot)
    {
        AbilityData ability = ProfileManager.GetAbility(slot);
        if (ability == null)
            return "";
        float left = CooldownLeft(ability);
        return (slot == 0 ? "Q" : "E") + "  " + ability.displayName + "  " + (left > 0f ? left.ToString("0.0") + "s" : "Ready");
    }

    bool Fireball(AbilityData ability, float damage, float radius)
    {
        Projectile.Spawn(ability.model, combat.ShotOrigin(), combat.AimDirection, 1f, Vector3.zero,
                         fireballSpeed, damage, fireballDistance, radius, false);
        return true;
    }

    bool FrostNova(AbilityData ability, float damage, float radius, float slowSeconds)
    {
        Vector3 centre = transform.position;
        Damage.HitArea(centre, radius, damage);
        foreach (Mob mob in MobsWithin(centre, radius))
            mob.Slow(iceSlow, slowSeconds);

        if (ability.model != null)
        {
            GameObject nova = Instantiate(ability.model, centre, Quaternion.identity);
            nova.transform.localScale = Vector3.one * radius * 2f;
            Destroy(nova, 0.3f);
        }
        return true;
    }

    // Nothing in range = nothing happens and the cooldown doesn't start
    bool ChainLightning(AbilityData ability, float damage, float range, int chains)
    {
        List<Mob> hit = new List<Mob>();
        Vector3 from = transform.position;
        Mob target = Nearest(from, range, hit);
        if (target == null)
            return false;

        // each jump reaches half as far as the first strike
        for (int i = 0; i <= chains && target != null; i++)
        {
            Vector3 to = target.transform.position;
            Bolt(ability.model, from, to);
            hit.Add(target);
            target.TakeDamage(damage);
            from = to;
            target = Nearest(from, range * 0.5f, hit);
        }
        return true;
    }

    void Bolt(GameObject model, Vector3 from, Vector3 to)
    {
        if (model == null)
            return;
        from.y = to.y = transform.position.y;
        Vector3 dir = to - from;
        if (dir.sqrMagnitude < 0.01f)
            return;

        GameObject bolt = Instantiate(model, (from + to) * 0.5f, Quaternion.LookRotation(dir));
        Vector3 s = bolt.transform.localScale;
        bolt.transform.localScale = new Vector3(s.x, s.y, dir.magnitude);
        Destroy(bolt, 0.15f);
    }

    Mob Nearest(Vector3 from, float range, List<Mob> skip)
    {
        Mob best = null;
        float bestDistance = range;
        foreach (Mob mob in MobsWithin(from, range))
        {
            if (skip.Contains(mob))
                continue;
            float d = Vector3.Distance(from, mob.transform.position);
            if (d <= bestDistance)
            {
                best = mob;
                bestDistance = d;
            }
        }
        return best;
    }

    // Each mob once, even if it's made of several colliders
    List<Mob> MobsWithin(Vector3 centre, float radius)
    {
        List<Mob> mobs = new List<Mob>();
        foreach (Collider c in Physics.OverlapSphere(centre, radius))
        {
            Mob mob = c.GetComponentInParent<Mob>();
            if (mob != null && !mobs.Contains(mob))
                mobs.Add(mob);
        }
        return mobs;
    }
}
