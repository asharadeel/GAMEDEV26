// ============================================================
// Title:       LazerData
// Version:     2.0
// Written by:  Ashar
// Updated by:  Ali (v2.0: lazers are a look + perk, the gun's
//              upgrade bars decide damage, fire rate and speed)
// Description: What a shooter fires: the model and its perks
//              (bigger shots, passing through enemies).
//              Add it to a shooter's Lazers list to use it.
//              Create > Game > Weapons > Lazer
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLazer", menuName = "Game/Weapons/Lazer")]
public class LazerData : WeaponData
{
    [Header("Lazer")]
    public float distance = 50f;     //travel distance before it disappears
    public float size = 1f;          //model scale multiplier
    public int pierce = 0;           //enemies it goes through
    public Vector3 rotationOffset = new Vector3(90f, 0f, 0f); //tilts the model to face travel direction

    //SHOP TEXT
    public string PerkText
    {
        get
        {
            string text = "";
            if (size > 1f)
                text = "Bigger shots";
            if (pierce > 0)
                text += (text == "" ? "Goes" : ", goes") + " through " + pierce + (pierce == 1 ? " enemy" : " enemies");
            return text;
        }
    }
}
