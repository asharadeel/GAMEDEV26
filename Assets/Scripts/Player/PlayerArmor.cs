// ============================================================
// Title:       PlayerArmor
// Version:     1.0
// Written by:  Ashar
//              ai use: claude (anthropic) used for debugging and assistance, owner Ashar
// Description: The worn armour's shield. Damage hits the shield
//              first, whatever is left over goes to health. When
//              the shield breaks one piece is used up, and if there
//              are spare pieces the next goes on after the armour's
//              Replace Delay (health takes the hits until then).
//              PlayerStats adds this to the player by itself.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmor : MonoBehaviour
{
    //replace state
    private bool replacing;
    private float replaceTimeLeft;

    public bool IsReplacing
    {
        get { return replacing; }
    }

    public float ReplaceTimeLeft
    {
        get { return replaceTimeLeft; }
    }

    //STARTING PROCEDURES
    void Start()
    {
        //left the last scene mid-replace: start the wait again
        ArmorData armor = ProfileManager.GetEquippedArmor();
        PlayerProfile p = ProfileManager.Profile;
        if (armor != null && p.ArmorShield <= 0f && p.GetArmorCount(armor.id) > 0)
            StartCoroutine(Replace(armor));
    }

    //DAMAGE
    //returns the damage the shield couldn't take
    public float Absorb(float damage)
    {
        ArmorData armor = ProfileManager.GetEquippedArmor();
        PlayerProfile p = ProfileManager.Profile;
        if (replacing || armor == null || p.ArmorShield <= 0f)
            return damage;

        float shield = p.ArmorShield;
        if (damage < shield)
        {
            p.ArmorShield = shield - damage;
            return 0f;
        }

        Break(armor);
        return damage - shield;
    }

    //BREAK AND REPLACE
    void Break(ArmorData armor)
    {
        PlayerProfile p = ProfileManager.Profile;
        p.ArmorShield = 0f;
        p.AddArmor(armor.id, -1);
        Debug.Log(armor.displayName + " broke, " + p.GetArmorCount(armor.id) + " left.");

        if (p.GetArmorCount(armor.id) > 0)
            StartCoroutine(Replace(armor));
    }

    IEnumerator Replace(ArmorData armor)
    {
        replacing = true;
        replaceTimeLeft = armor.replaceDelay;
        while (replaceTimeLeft > 0f)
        {
            replaceTimeLeft -= Time.deltaTime;
            yield return null;
        }

        replacing = false;
        replaceTimeLeft = 0f;
        ProfileManager.EquipArmor(armor);
        Debug.Log("Put on a new " + armor.displayName + ".");
    }
}
