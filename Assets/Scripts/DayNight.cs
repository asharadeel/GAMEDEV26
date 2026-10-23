// ============================================================
// Title:       DayNight
// Version:     1.1
// Written by:  Ashar
// Description: Day/night cycle: rotates and fades the sun. At night
//              the same light becomes a dim blue moon so it never
//              goes fully black.
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

    //moon details
    public float moonIntensity = 0.3f;                          //brightness at midnight
    public Color sunColour = Color.white;
    public Color moonColour = new Color(0.6f, 0.7f, 1f);

    void Update()
    {
        timeOfDay += Time.deltaTime / dayLength;
        if (timeOfDay >= 1f)
            timeOfDay -= 1f;

        float angle = timeOfDay * 360f - 90f;
        float height = Mathf.Sin(timeOfDay * Mathf.PI * 2f - Mathf.PI / 2f);   //1 noon, -1 midnight

        if (height >= 0f)
        {
            //SUN
            sun.transform.rotation = Quaternion.Euler(angle, -30f, 0f);
            sun.intensity = maxIntensity * height;
            sun.color = sunColour;
        }
        else
        {
            //MOON - opposite side of the sky, fades in after sunset so it doesn't pop
            sun.transform.rotation = Quaternion.Euler(angle + 180f, -30f, 0f);
            sun.intensity = moonIntensity * Mathf.Clamp01(-height * 3f);
            sun.color = moonColour;
        }
    }
}
