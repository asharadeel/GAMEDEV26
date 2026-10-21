// ============================================================
// Title:       Pickup
// Version:     1.0
// Written by:  Davud Jabiyev
// Description: A consumable lying in the world. The player picks it
//              up by walking over it. If the inventory is full it
//              stays on the ground. Createable with
//              Pickup.Spawn(position, item).
// ============================================================

using UnityEngine;

public class Pickup : MonoBehaviour
{
    public ConsumableData item;
    public float lifetime = 30f;         // seconds until it despawns

    // Small cube with a trigger collider
    public static void Spawn(Vector3 position, ConsumableData item)
    {
        // Create cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position + Vector3.up * 0.5f;
        cube.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        // Make the collider a trigger so the player can walk through it
        cube.GetComponent<Collider>().isTrigger = true;

        // Attach this script to cube and set item to give
        Pickup pickup = cube.AddComponent<Pickup>();
        pickup.item = item;
    }

    void Start()
    {
        // De-spawn it after a while
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Only player can pick it up
        if (!other.CompareTag("Player")) {
            return;
        }

        // If inventory is full, don't pick up
        bool added = ProfileManager.Profile.AddToInventory(item.id);
        if (added) {
            Destroy(gameObject);
        }
    }
}
