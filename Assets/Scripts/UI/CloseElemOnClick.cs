// ============================================================
// Title:       CloseElemOnClick
// Version:     1.0
// Written by:  Ashar
// Description: Button: turns off every object in the list.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CloseElemOnClick : MonoBehaviour
{
    public GameObject[] elements;

    //LISTENERS
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Close);
    }

    //INTERFACE
    void Close()
    {
        foreach (GameObject element in elements)
        {
            if (element != null)
                element.SetActive(false);
        }
        Debug.Log("Closed an interface.");
    }
}
