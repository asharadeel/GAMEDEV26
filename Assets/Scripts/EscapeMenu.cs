// ============================================================
// Title:       EscapeMenu
// Version:     1.0
// Written by:  Ashar
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
        if (Input.GetKeyDown(KeyCode.Escape) && menu != null)
        {
            menu.SetActive(!menu.activeSelf);
        }
    }
}
