// ============================================================
// Title:       Pickup
// Version:     1.5
// Written by:  Davud Jabiyev
// Updated by:  Davud Jabiyev (v1.3: counts pickups for achievements)
//              Ashar (v1.4: ores, they go in the ore pouch so are never left behind,
//              v1.5: uses the item's model when it has one)
// Description: A consumable lying in the world. The player picks it
//              up by walking over it. If the inventory is full it
//              stays on the ground. Coloured by type, spins and bobs
//              up and down, shows a message when picked up. Createable with
//              Pickup.Spawn(position, item).
// ============================================================

using UnityEngine;

public class Pickup : MonoBehaviour
{
    public ItemData item;
    public float lifetime = 30f;         // seconds until it despawns
    public float spinSpeed = 90f;        // degrees per second
    public float bobHeight = 0.15f;      // how far it moves up and down
    public float bobSpeed = 3f;          // how fast it moves up and down

    private Vector3 startPos;            // where it was spawned (for bobbing)

    // Small cube with a trigger collider
    public static void Spawn(Vector3 position, ItemData item)
    {
        // Use the item's model if it has one (Models/Consumables, Models/Ores)
        GameObject model = null;
        if (item is ConsumableData c) {
            model = c.model;
        } else if (item is OreData o) {
            model = o.model;
        }
        if (model != null) {
            GameObject obj = Object.Instantiate(model, position + Vector3.up * 0.5f, Quaternion.identity);
            BoxCollider box = obj.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(0.8f, 0.8f, 0.8f);
            obj.AddComponent<Pickup>().item = item;
            return;
        }

        // Create cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position + Vector3.up * 0.5f;
        cube.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        // Make the collider a trigger so the player can walk through it
        cube.GetComponent<Collider>().isTrigger = true;

        // Set a colour
        // blue for drinks, orange for food, white for else
        Renderer rend = cube.GetComponent<Renderer>();
        ConsumableData food = item as ConsumableData;
        if (item is OreData ore) {
            rend.material.color = ore.colour;
        } else if (food != null && food.thirst > 0) {
            rend.material.color = Color.blue;
        } else if (food != null && food.hunger > 0) {
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
        // Remember start position for bobbing
        startPos = transform.position;

        // De-spawn it after a while
        Destroy(gameObject, lifetime);
    }

    // Runs every frame
    // Used for bobbing and spinning
    void Update()
    {
        // Spin around the up axis
        // Time.deltaTime = seconds since last frame
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);

        // Bob up and down
        // Mathf.Sin alternates smoothly between -1 and 1 :)
        // Time.time = seconds since game started
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPos + Vector3.up * offset;
    }

    void OnTriggerEnter(Collider other)
    {
        // Only player can pick it up
        if (!other.CompareTag("Player")) {
            return;
        }

        // Ores go in the ore pouch (never full), food/drink needs a free slot
        bool added;
        if (item is OreData) {
            ProfileManager.Profile.AddOre(item.id, 1);
            added = true;
        } else {
            added = ProfileManager.Profile.AddToInventory(item.id);
        }
        if (added) {
            // Show message
            FloatingText.Show(transform.position, "+ " + item.displayName);

            AchievementManager.Add(AchievementStat.ItemsPickedUp, 1);
            Destroy(gameObject);
        } else {
            AchievementManager.Add(AchievementStat.DropsMissed, 1);
        }
    }
}
