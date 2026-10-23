// ============================================================
// Title:       StreetLight
// Version:     1.0
// Written by:  Ashar
// Description: Turns a street light on at night and off in the
//              day, using the scene's DayNight. On = the bulb
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

    private DayNight dayNight;
    private Material bulbMaterial;
    private bool isOn;

    //STARTING PROCEDURES
    void Start()
    {
        dayNight = FindFirstObjectByType<DayNight>();
        bulbMaterial = bulb.material;      //own copy so lights don't change each other
        SetOn(IsNight());
    }

    //DAY/NIGHT CHECK
    void Update()
    {
        bool night = IsNight();
        if (night != isOn)
            SetOn(night);
    }

    //no DayNight = always off
    bool IsNight()
    {
        return dayNight != null && (dayNight.timeOfDay < 0.25f || dayNight.timeOfDay > 0.75f);
    }

    //SWITCH LIGHT
    void SetOn(bool on)
    {
        isOn = on;
        lamp.enabled = on;

        if (on)
        {
            bulbMaterial.EnableKeyword("_EMISSION");
            bulbMaterial.SetColor("_EmissionColor", glowColour);
        }
        else
        {
            bulbMaterial.DisableKeyword("_EMISSION");
        }
    }
}
