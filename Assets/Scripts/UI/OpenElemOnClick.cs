// ============================================================
// Title:       OpenElemOnClick
// Version:     1.0
// Written by:  Ashar
// Description: Button: turns on every object in the list.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

// Put on a Button. Turns ON every object in the list when clicked.
[RequireComponent(typeof(Button))]
public class OpenElemOnClick : MonoBehaviour
{
    public GameObject[] elements;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Open);
    }

    void Open()
    {
        foreach (GameObject element in elements)
        {
            if (element != null)
                element.SetActive(true);
        }
    }
}
