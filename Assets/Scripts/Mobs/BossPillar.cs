using UnityEngine;

// one of the purple pillars around the boss room
// shoot or hit it to break it, while any pillar is up the boss takes less damage
// a line joins it to the boss so you can see what its doing
// the boss rebuilds it after a while
public class BossPillar : MonoBehaviour
{
    public float maxHealth = 50f;
    public GameObject crystal;        // the part that breaks
    public LineRenderer beam;         // line to the boss
    public GameObject healthBarPrefab; // same health bar the mobs use, shows up once its hit

    [HideInInspector] public BossMob boss;
    [HideInInspector] public bool broken;

    private float health;
    private Renderer crystalRenderer;
    private Color fullColour;
    private Vector3 fullSize;
    private float growTimer = 1f;
    private MobHealthBar bar;

    void Start()
    {
        health = maxHealth;
        crystalRenderer = crystal.GetComponent<Renderer>();
        fullColour = crystalRenderer.material.color;
        fullSize = crystal.transform.localScale;
    }

    void Update()
    {
        // grow back in when rebuilt
        if (growTimer < 1f)
        {
            growTimer += Time.deltaTime / 0.6f;
            crystal.transform.localScale = fullSize * Mathf.Min(growTimer, 1f);
        }

        // keep the beam pointing at the boss
        if (beam != null)
        {
            bool show = !broken && boss != null;
            beam.enabled = show;
            if (show)
            {
                beam.SetPosition(0, crystal.transform.position);
                beam.SetPosition(1, boss.transform.position + Vector3.up * 3f);
            }
        }
    }

    // called by the players shots and melee (same as mobs)
    public void TakeDamage(float amount)
    {
        if (broken)
            return;

        health -= amount;

        // show the health bar above it
        if (bar == null && healthBarPrefab != null)
        {
            Vector3 top = transform.position + Vector3.up * 4.5f;
            bar = Instantiate(healthBarPrefab, top, Quaternion.identity, transform).GetComponent<MobHealthBar>();
        }
        if (bar != null)
        {
            bar.gameObject.SetActive(true);
            bar.Set(health, maxHealth);
        }

        // gets darker the more its hit
        crystalRenderer.material.color = Color.Lerp(Color.black, fullColour, health / maxHealth);

        if (health <= 0f)
        {
            broken = true;
            crystal.SetActive(false);
            if (bar != null)
                bar.gameObject.SetActive(false);
            FloatingText.Show(transform.position + Vector3.up * 4f, "Pillar broken!");
            if (boss != null)
                boss.PillarBroken();
        }
    }

    public void Rebuild()
    {
        broken = false;
        health = maxHealth;
        crystalRenderer.material.color = fullColour;
        crystal.SetActive(true);
        crystal.transform.localScale = Vector3.zero;
        growTimer = 0f;
    }
}
