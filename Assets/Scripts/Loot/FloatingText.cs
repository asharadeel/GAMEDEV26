// ============================================================
// Title:       FloatingText
// Version:     1.0
// Written by:  Davud Jabiyev
// Description: A short message in the world that floats up and
//              disappears, e.g. "+ Beans". Createable with
//              FloatingText.Show(position, message).
// ============================================================

using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float riseSpeed = 1f;
    public float lifetime = 1.5f;       // seconds until it disappears

    public static void Show(Vector3 position, string message)
    {
        // Create game object to hold text
        GameObject textHolder = new GameObject("FloatingText");
        textHolder.transform.position = position + Vector3.up;

        // Create and add text component
        TextMesh text = textHolder.AddComponent<TextMesh>();
        text.text = message;
        text.characterSize = 0.15f;
        text.fontSize = 48;
        text.anchor = TextAnchor.MiddleCenter;
        text.color = Color.white;

        // Add this script to the object
        textHolder.AddComponent<FloatingText>();
    }

    // Runs once, after object created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Runs every frame
    void Update()
    {
        // Move up a little each frame.
        // Time.deltaTime = seconds since the last frame
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
    }
}
