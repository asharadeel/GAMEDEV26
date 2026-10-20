// ============================================================
// Title:       Projectile
// Version:     1.1
// Written by:  Ashar
// Description: Moves a fired lazer/special and deals damage on hit.
// ============================================================

using UnityEngine;

// Added to a bullet/rocket when it's fired. Flies straight until it hits something
// or has travelled its max distance.
// radius 0 = hits one thing, radius > 0 = explodes and hits everything nearby.
public class Projectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private float radius;
    private float maxDistance;
    private float travelled;
    private Vector3 direction;

    public void Launch(Vector3 dir, float projectileSpeed, float projectileDamage, float distance, float explosionRadius)
    {
        direction = dir;
        speed = projectileSpeed;
        damage = projectileDamage;
        radius = explosionRadius;
        maxDistance = distance;
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
        if (other.CompareTag("Player") || other.GetComponentInParent<Projectile>() != null)
            return;

        if (radius > 0f)
        {
            foreach (Collider hit in Physics.OverlapSphere(transform.position, radius))
            {
                if (!hit.CompareTag("Player"))
                    DealDamage(hit);
            }
        }
        else
        {
            DealDamage(other);
        }

        Destroy(gameObject);
    }

    // Anything with a TakeDamage(float) method receives the damage (enemies later)
    void DealDamage(Collider target)
    {
        target.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
    }
}
