// ============================================================
// Title:       Mob
// Version:     1.2
// Written by:  Ashar
// Description: Base for every mob. Holds health, shows the health
//              bar, dies at 0, and has helpers for finding the
//              player. FollowMob and ShooterMob build on this.
// ============================================================

using UnityEngine;

public class Mob : MonoBehaviour
{
    public MobData data;
    public float health;

    protected Transform player;
    protected float myRadius;            // half the mob's width
    protected float playerRadius;

    private MobHealthBar healthBar;

    public virtual void Init(MobData mobData, GameObject healthBarPrefab)
    {
        data = mobData;
        health = mobData.health;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            Collider pc = p.GetComponent<Collider>();
            playerRadius = pc != null ? pc.bounds.extents.x : 0.5f;
        }

        Bounds bounds = ModelBounds();
        myRadius = Mathf.Max(bounds.extents.x, bounds.extents.z);

        if (healthBarPrefab != null)
        {
            // Sit the bar just above the top of the model
            Vector3 pos = new Vector3(transform.position.x, bounds.max.y + 0.6f, transform.position.z);
            healthBar = Instantiate(healthBarPrefab, pos, Quaternion.identity, transform).GetComponent<MobHealthBar>();
        }

        UpdateBar();
    }

    public float AttackDamage
    {
        get { return data != null ? data.attackDamage : 0f; }
    }

    // Called by projectiles and melee (via Damage.Hit)
    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        UpdateBar();

        if (health <= 0f)
            Destroy(gameObject);
    }

    // ---- Helpers for FollowMob / ShooterMob ----

    // Flat distance (ignores height). Infinity if there's no player.
    protected float DistanceToPlayer()
    {
        if (player == null)
            return Mathf.Infinity;

        Vector3 a = transform.position, b = player.position;
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    // Flat direction towards the player
    protected Vector3 DirectionToPlayer()
    {
        if (player == null)
            return Vector3.zero;

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        return dir.normalized;
    }

    protected void Face(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    Bounds ModelBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);
        return bounds;
    }

    void UpdateBar()
    {
        if (healthBar != null && data != null)
            healthBar.Set(health, data.health);
    }
}
