// ============================================================
// Title:       InactiveOnStart
// Version:     1.0
// Written by:  Ashar
// Description: Turns its object off when the scene starts.
// ============================================================

using UnityEngine;

// Put on any object you want turned off when the scene starts.
public class InactiveOnStart : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
