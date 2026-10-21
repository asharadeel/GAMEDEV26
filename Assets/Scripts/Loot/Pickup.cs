// ============================================================
// Title:       Pickup
// Version:     1.1
// Written by:  Davud Jabiyev
// Description: A consumable lying in the world. The player picks it
//              up by walking over it. If the inventory is full it
//              stays on the ground. Coloured by type, shows a message
//              when picked up. Createable with
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

        // Set a colour
        // blue for drinks, orange for food, white for else
        Renderer rend = cube.GetComponent<Renderer>();
        if (item.thirst > 0) {
            rend.material.color = Color.blue;
        } else if (item.hunger > 0) {
            rend.material.color = new Color(1f, 0.5f, 0f);
        } else {
            rend.material.color = Color.white;
        }

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
            // Show message
            FloatingText.Show(transform.position, "+ " + item.displayName);

            Destroy(gameObject);
        }
    }
}
