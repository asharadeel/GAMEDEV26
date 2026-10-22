// ============================================================
// Title:       AbilityData
// Version:     1.0
// Written by:  Ali
// Description: An ability (Fire, Ice, Lightning) and its branch of
//              the skill tree. Every player has every ability at
//              its base stats for free; the skill tree nodes add to
//              them. Equipped on Q or E in the Inventory.
//              Create > Game > Ability
// ============================================================

using System.Collections.Generic;
using UnityEngine;

public enum AbilityType { Fire, Ice, Lightning }

// What a skill node improves
public enum AbilityStat { Damage, Radius, Cooldown, Extra }

// One node of the ability's branch. The ability itself is the root at the bottom (row 0),
// nodes sit above it: row 1-3, column -1 (left path), 0 (middle) or 1 (right path)
[System.Serializable]
public class SkillNode
{
    public string title = "";
    public AbilityStat stat;
    public float amount;             // added to the stat (negative for cooldown)
    public int cost = 50;            // coins
    public int parent = -1;          // index of the node it grows from, -1 = the ability itself
    public int column;
    public int row = 1;
}

[CreateAssetMenu(fileName = "NewAbility", menuName = "Game/Ability")]
public class AbilityData : ItemData
{
    [Header("Ability")]
    public AbilityType type;
    public Color colour = Color.white;   // nodes, lines and card highlight
    public GameObject model;             // Fire: the fireball, Ice: the nova, Lightning: one bolt

    [Header("Base stats (with no nodes unlocked)")]
    public float damage = 20f;
    public float radius = 3f;            // Fire: blast, Ice: nova size, Lightning: range
    public float cooldown = 6f;          // seconds
    public float extra = 0f;             // Ice: slow seconds, Lightning: chains
    public string radiusName = "Radius";
    public string extraName = "";        // empty = not used

    [Header("Skill tree")]
    public List<SkillNode> nodes = new List<SkillNode>();

    // Node ids in the save look like "fire.2"
    public string NodeId(int index)
    {
        return id + "." + index;
    }

    public float BaseValue(AbilityStat stat)
    {
        switch (stat)
        {
            case AbilityStat.Damage: return damage;
            case AbilityStat.Radius: return radius;
            case AbilityStat.Cooldown: return cooldown;
            default: return extra;
        }
    }

    public string StatName(AbilityStat stat)
    {
        switch (stat)
        {
            case AbilityStat.Damage: return "Damage";
            case AbilityStat.Radius: return radiusName;
            case AbilityStat.Cooldown: return "Cooldown";
            default: return extraName;
        }
    }

    // e.g. "+10 Damage", "-1s Cooldown"
    public string EffectText(SkillNode node)
    {
        string sign = node.amount >= 0f ? "+" : "-";
        string unit = node.stat == AbilityStat.Cooldown || (node.stat == AbilityStat.Extra && type == AbilityType.Ice) ? "s" : "";
        return sign + Mathf.Abs(node.amount).ToString("0.#") + unit + " " + StatName(node.stat);
    }
}
