// ============================================================
// Title:       StreetLight
// Version:     1.1
// Written by:  Ashar
// Description: Keeps a street light on all the time: the bulb
//              glows (emission) and its point light is enabled.
//              Goes on the StreetLight prefab's root.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLight : MonoBehaviour
{
    //light parts
    public Renderer bulb;
    public Light lamp;
    public Color glowColour = Color.white;

    //STARTING PROCEDURES
    void Start()
    {
        Material bulbMaterial = bulb.material;   //own copy so lights don't change each other
        bulbMaterial.EnableKeyword("_EMISSION");
        bulbMaterial.SetColor("_EmissionColor", glowColour);
        lamp.enabled = true;
    }
}
