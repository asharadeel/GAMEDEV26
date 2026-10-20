// ============================================================
// Title:       Projectile
// Version:     1.3
// Written by:  Ashar
// Description: Moves a fired lazer/special and deals damage on hit.
//              Player shots hit everything except the player.
//              Enemy shots (fromEnemy) hit the player and pass
//              through mobs.
// ============================================================

using UnityEngine;

// Flies straight until it hits something or has travelled its max distance.
// radius 0 = hits one thing, radius > 0 = explodes and hits everything nearby.
public class Projectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private float radius;
    private float maxDistance;
    private float travelled;
    private bool fromEnemy;
    private Vector3 direction;

    // Spawns a model as a projectile. Used by the player and by shooter mobs.
    public static Projectile Spawn(GameObject model, Vector3 position, Vector3 direction, float size, Vector3 rotationOffset,
                                   float speed, float damage, float distance, float radius, bool fromEnemy)
    {
        if (model == null)
            return null;

        Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset);
        GameObject obj = Instantiate(model, position, rotation);
        obj.transform.localScale *= size;

        // Triggers + kinematic rigidbody so it detects hits without being pushed around
        foreach (Collider c in obj.GetComponentsInChildren<Collider>())
            c.isTrigger = true;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
            rb = obj.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        Projectile p = obj.AddComponent<Projectile>();
        p.direction = direction;
        p.speed = speed;
        p.damage = damage;
        p.maxDistance = distance;
        p.radius = radius;
        p.fromEnemy = fromEnemy;
        return p;
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position += direction * step;

        travelled += step;
        if (travelled >= maxDistance)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Projectile>() != null)
            return;

        if (fromEnemy)
        {
            // Enemy shots fly through mobs and only hurt the player
            if (other.GetComponentInParent<Mob>() != null)
                return;
            if (other.CompareTag("Player"))
                Damage.Hit(other, damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Player"))
            return;

        if (radius > 0f)
            Damage.HitArea(transform.position, radius, damage);
        else
            Damage.Hit(other, damage);

        Destroy(gameObject);
    }
}
