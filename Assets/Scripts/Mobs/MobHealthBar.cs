// ============================================================
// Title:       MobHealthBar
// Version:     1.0
// Written by:  Ashar
// Description: Small health bar that floats above a mob: a slider
//              that drops as it takes damage and its health as a
//              number. Always faces the camera.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobHealthBar : MonoBehaviour
{
    //bar ui
    public Slider slider;
    public Text healthText;

    private Camera cam;

    //STARTING PROCEDURES
    void Start()
    {
        cam = Camera.main;
    }

    //UPDATE BAR
    public void Set(float current, float max)
    {
        if (slider != null)
            slider.value = Mathf.Clamp01(current / max);
        if (healthText != null)
            healthText.text = Mathf.CeilToInt(Mathf.Max(current, 0f)) + " / " + Mathf.CeilToInt(max);
    }

    //FACE CAMERA
    void LateUpdate()
    {
        if (cam != null)
            transform.rotation = cam.transform.rotation;
    }
}
