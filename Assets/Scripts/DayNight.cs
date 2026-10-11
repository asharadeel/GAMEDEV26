using UnityEngine;

public class DayNight : MonoBehaviour
{
    public Light sun;

    public float dayLength = 120f; // seconds for a full day

    [Range(0f, 1f)]
    public float timeOfDay = 0.3f; // 0 = midnight, 0.25 = sunrise, 0.5 = noon, 0.75 = sunset

    public float maxIntensity = 1f;

    void Update()
    {
        timeOfDay += Time.deltaTime / dayLength;
        if (timeOfDay >= 1f)
            timeOfDay -= 1f;

        // Spin the sun around: 0 at sunrise, 90 overhead at noon, 180 at sunset
        float angle = timeOfDay * 360f - 90f;
        sun.transform.rotation = Quaternion.Euler(angle, -30f, 0f);

        // Bright in the day, off at night
        float height = Mathf.Sin(timeOfDay * Mathf.PI * 2f - Mathf.PI / 2f);
        sun.intensity = maxIntensity * Mathf.Clamp01(height);
    }
}
