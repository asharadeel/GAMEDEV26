// ============================================================
// Title:       OverlayUI
// Version:     1.0
// Written by:  Ashar
// Description: Put on a menu's Frame. While the Frame is showing,
//              PlayerController.uiEnabled is true and the player
//              can't move, jump, attack, eat, rotate or zoom the
//              camera (so scrolling a menu doesn't zoom the camera).
//              Counts open overlays, so two menus open at once works.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayUI : MonoBehaviour
{
    private static int openCount;   // how many overlays are showing right now

    //OVERLAY COUNT
    void OnEnable()
    {
        openCount++;
        PlayerController.uiEnabled = true;
    }

    void OnDisable()
    {
        openCount = Mathf.Max(0, openCount - 1);
        PlayerController.uiEnabled = openCount > 0;
    }
}
