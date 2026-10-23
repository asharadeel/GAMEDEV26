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
