// ============================================================
// Title:       InactiveOnStart
// Version:     1.0
// Written by:  Ashar
// Description: Turns its object off when the scene starts.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveOnStart : MonoBehaviour
{
    //STARTING PROCEDURES
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
