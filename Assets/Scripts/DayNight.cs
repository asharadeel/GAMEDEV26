// ============================================================
// Title:       DayNight
// Version:     1.0
// Written by:  Ashar
// Description: Day/night cycle: rotates and fades the sun.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    //sun details
    public Light sun;

    public float dayLength = 120f; //seconds

    [Range(0f, 1f)]
    public float timeOfDay = 0.3f; //0 midnight, 0.25 sunrise, 0.5 noon, 0.75 sunset

    public float maxIntensity = 1f;

    void Update()
    {
        timeOfDay += Time.deltaTime / dayLength;
        if (timeOfDay >= 1f)
            timeOfDay -= 1f;

        //ROTATE SUN
        float angle = timeOfDay * 360f - 90f;
        sun.transform.rotation = Quaternion.Euler(angle, -30f, 0f);

        //SUN INTENSITY
        float height = Mathf.Sin(timeOfDay * Mathf.PI * 2f - Mathf.PI / 2f);
        sun.intensity = maxIntensity * Mathf.Clamp01(height);
    }
}
