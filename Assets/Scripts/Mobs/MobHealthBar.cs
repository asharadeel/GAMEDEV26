// ============================================================
// Title:       MobHealthBar
// Version:     1.0
// Written by:  Ashar
// Description: Small health bar that floats above a mob: a slider
//              that drops as it takes damage and its health as a
//              number. Always faces the camera.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

public class MobHealthBar : MonoBehaviour
{
    public Slider slider;
    public Text healthText;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    public void Set(float current, float max)
    {
        if (slider != null)
            slider.value = Mathf.Clamp01(current / max);
        if (healthText != null)
            healthText.text = Mathf.CeilToInt(Mathf.Max(current, 0f)) + " / " + Mathf.CeilToInt(max);
    }

    void LateUpdate()
    {
        // Face the same way as the camera so it's always readable
        if (cam != null)
            transform.rotation = cam.transform.rotation;
    }
}
