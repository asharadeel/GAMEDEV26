// ============================================================
// Title:       Flashlight
// Version:     1.0
// Written by:  Ashar
// Description: F turns a flashlight on/off. It's a spot light
//              that points where the player is aiming (the mouse).
//              Goes on the Player.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    //beam details
    public float range = 25f;
    public float spotAngle = 50f;
    public float intensity = 3f;
    public Color colour = new Color(1f, 0.95f, 0.8f);
    public float tilt = 10f;            //degrees down towards the floor

    private Light beam;
    private PlayerCombat combat;

    //STARTING PROCEDURES
    void Start()
    {
        combat = GetComponent<PlayerCombat>();

        GameObject go = new GameObject("FlashlightBeam");
        go.transform.SetParent(transform, false);
        beam = go.AddComponent<Light>();
        beam.type = LightType.Spot;
        beam.range = range;
        beam.spotAngle = spotAngle;
        beam.intensity = intensity;
        beam.color = colour;
        beam.shadows = LightShadows.Soft;
        beam.enabled = false;
    }

    //INPUT
    void Update()
    {
        if (PlayerController.uiEnabled)
            return;

        if (GameInput.Flashlight.WasPressedThisFrame())
        {
            beam.enabled = !beam.enabled;
            Debug.Log(beam.enabled ? "Flashlight on." : "Flashlight off.");
        }
    }

    //AIM
    void LateUpdate()
    {
        if (!beam.enabled)
            return;

        Vector3 aim = combat != null ? combat.AimDirection : MouseAim();
        beam.transform.position = transform.position;
        beam.transform.rotation = Quaternion.LookRotation(aim) * Quaternion.Euler(tilt, 0f, 0f);
    }

    //no PlayerCombat (e.g. the Hub): point at the mouse on the floor
    Vector3 MouseAim()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return transform.forward;

        Ray ray = cam.ScreenPointToRay(GameInput.PointerPosition);
        Plane ground = new Plane(Vector3.up, transform.position);
        if (ground.Raycast(ray, out float distance))
        {
            Vector3 dir = ray.GetPoint(distance) - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                return dir.normalized;
        }
        return transform.forward;
    }
}
