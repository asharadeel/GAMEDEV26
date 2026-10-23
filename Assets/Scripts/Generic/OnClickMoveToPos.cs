// ============================================================
// Title:       OnClickMoveToPos
// Version:     1.0
// Written by:  Ashar
// Description: Button: moves the player to a transform's position
//              when clicked (e.g. "Enter" on the Enter Building
//              pop-up puts the player at the building's landing).
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OnClickMoveToPos : MonoBehaviour
{
    public Transform target;

    //LISTENERS
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Move);
    }

    //TELEPORT
    void Move()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null || target == null)
            return;

        //move rb too or physics pulls the player back
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.position = target.position;
        }
        player.transform.position = target.position;
        Debug.Log("Moved player to target.");
    }
}
