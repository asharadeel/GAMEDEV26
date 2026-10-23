// ============================================================
// Title:       EscapeMenu
// Version:     1.1
// Written by:  Ashar
// Updated by:  cthullu96 (v1.1: input system package)
//              ai use: input system change written with claude (anthropic), owner cthullu96
// Description: Esc toggles the menu on and off.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//must sit on an always-on object, not the menu frame itself
public class EscapeMenu : MonoBehaviour
{
    public GameObject menu;

    //INPUT
    void Update()
    {
        if (GameInput.Menu.WasPressedThisFrame() && menu != null)
        {
            menu.SetActive(!menu.activeSelf);
        }
    }
}
