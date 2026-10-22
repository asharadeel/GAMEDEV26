// ============================================================
// Title:       StreetLight
// Version:     1.0
// Written by:  Ashar
// Description: Turns a street light on at night and off in the
//              day, using the scene's DayNight. On = the bulb
//              glows (emission) and its point light is enabled.
//              Goes on the StreetLight prefab's root.
// ============================================================

using UnityEngine;

public class StreetLight : MonoBehaviour
{
    public Renderer bulb;                  // the sphere at the top
    public Light lamp;                     // point light on the bulb
    public Color glowColour = Color.white;

    private DayNight dayNight;
    private Material bulbMaterial;
    private bool isOn;

    void Start()
    {
        dayNight = FindFirstObjectByType<DayNight>();
        bulbMaterial = bulb.material;      // its own copy, so lights don't change each other
        SetOn(IsNight());
    }

    void Update()
    {
        bool night = IsNight();
        if (night != isOn)
            SetOn(night);
    }

    // Night is before sunrise (0.25) and after sunset (0.75). No DayNight = always off
    bool IsNight()
    {
        return dayNight != null && (dayNight.timeOfDay < 0.25f || dayNight.timeOfDay > 0.75f);
    }

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
