// ============================================================
// Title:       OpenElemOnClick
// Version:     1.0
// Written by:  Ashar
// Description: Button: turns on every object in the list.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenElemOnClick : MonoBehaviour
{
    public GameObject[] elements;

    //LISTENERS
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Open);
    }

    //INTERFACE
    void Open()
    {
        foreach (GameObject element in elements)
        {
            if (element != null)
                element.SetActive(true);
        }
        Debug.Log("Opened an interface.");
    }
}
