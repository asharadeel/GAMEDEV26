using UnityEngine;

// flat circle that grows out to the radius and then disappears
// the boss makes one when it lands from a jump
public class Shockwave : MonoBehaviour
{
    public float radius = 6f;
    public float time = 0.35f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / time;

        // grow from nothing to the full width, very thin
        float size = radius * 2f * t;
        transform.localScale = new Vector3(size, 0.02f, size);

        if (t >= 1f)
            Destroy(gameObject);
    }
}
