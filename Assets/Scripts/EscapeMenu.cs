// ============================================================
// Title:       EscapeMenu
// Version:     1.1
// Written by:  Ashar
// Updated by:  cthullu96 (v1.1: input system package)
//              ai use: input system change written with claude (anthropic), owner cthullu96
// Description: Esc toggles the menu on and off.
// ============================================================

using UnityEngine;

// Put on an object that's always on (e.g. the empty EscMenu), and drag the
// frame into "menu". Esc toggles the frame on/off.
// To start hidden, add InactiveOnStart to the frame.
public class EscapeMenu : MonoBehaviour
{
    public GameObject menu;

    void Update()
    {
        if (GameInput.Menu.WasPressedThisFrame() && menu != null)
        {
            menu.SetActive(!menu.activeSelf);
        }
    }
}
