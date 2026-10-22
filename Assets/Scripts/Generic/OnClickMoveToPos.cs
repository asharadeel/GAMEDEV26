// ============================================================
// Title:       OnClickMoveToPos
// Version:     1.0
// Written by:  Ashar
// Description: Button: moves the player to a transform's position
//              when clicked (e.g. "Enter" on the Enter Building
//              pop-up puts the player at the building's landing).
// ============================================================

using UnityEngine;
using UnityEngine.UI;

// Put on a Button. Drag where to go into Target.
[RequireComponent(typeof(Button))]
public class OnClickMoveToPos : MonoBehaviour
{
    public Transform target;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Move);
    }

    void Move()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null || target == null)
            return;

        // Move the Rigidbody too, otherwise physics can pull the player back
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.position = target.position;
        }
        player.transform.position = target.position;
    }
}
