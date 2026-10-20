// ============================================================
// Title:       PlayerCombat
// Version:     1.5
// Written by:  Ashar
// Description: Aiming, weapon slots and attacking.
// ============================================================

using UnityEngine;
using UnityEngine.EventSystems;

// Goes on the Player. Three weapon slots: 1 shooter, 2 melee, 3 special.
// 1/2/3 pick a slot, Q cycles. Left click or X attacks with the selected weapon.
// Aims at the mouse, 360 degrees around the player.
[RequireComponent(typeof(PlayerStats))]
public class PlayerCombat : MonoBehaviour
{
    public Camera cam;
    public KeyCode attackKey = KeyCode.X;

    [Header("Aim Arrow")]
    public Transform aimArrow;          // optional sprite that circles the player, pointing at the mouse
    public float arrowDistance = 1f;    // how far from the player's centre
    public float arrowHeight = -0.45f;  // up/down from the player's centre (negative = near the floor)
    public float arrowSpin = 0f;        // extra turn if the image doesn't point up

    private PlayerStats stats;
    private Vector3 aimDirection = Vector3.forward;
    private float nextAttackTime;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        aimDirection = GetAimDirection();
        UpdateArrow();

        if (Input.GetKeyDown(KeyCode.Alpha1)) stats.Profile.SelectedWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) stats.Profile.SelectedWeapon = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) stats.Profile.SelectedWeapon = 2;
        if (Input.GetKeyDown(KeyCode.Q)) stats.Profile.SelectedWeapon = (stats.Profile.SelectedWeapon + 1) % 3;

        // Clicking on UI (buttons, door prompt) shouldn't also shoot
        bool clickOnUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if ((Input.GetMouseButton(0) && !clickOnUI) || Input.GetKey(attackKey))
            Attack();
    }

    public void Attack()
    {
        if (Time.time < nextAttackTime)
            return;

        ItemDatabase db = ItemDatabase.Instance;
        PlayerProfile p = stats.Profile;

        if (p.SelectedWeapon == 0)
        {
            // The gun decides which lazers are allowed, the lazer decides the shot
            LazerData lazer = GetLazer(db.Get<ShooterData>(p.EquippedShooter));
            if (lazer == null)
                return;

            nextAttackTime = Time.time + 1f / lazer.fireRate;
            Fire(lazer.model, lazer.size, lazer.rotationOffset, lazer.speed, lazer.damage, lazer.distance, 0f);
        }
        else if (p.SelectedWeapon == 1)
        {
            MeleeData melee = db.Get<MeleeData>(p.EquippedMelee);
            if (!ProfileManager.Owns(melee))
                return;

            nextAttackTime = Time.time + 1f / melee.fireRate;
            Swing(melee);
        }
        else
        {
            SpecialData special = db.Get<SpecialData>(p.EquippedSpecial);
            if (!ProfileManager.Owns(special))
                return;

            nextAttackTime = Time.time + 1f / special.fireRate;
            Fire(special.model, special.size, special.rotationOffset, special.speed, special.damage, special.distance, special.radius);
        }
    }

    // The equipped lazer if this gun can use it and it's owned, otherwise the first owned lazer the gun has
    public LazerData GetLazer(ShooterData shooter)
    {
        if (!ProfileManager.Owns(shooter))
            return null;

        LazerData equipped = ItemDatabase.Instance.Get<LazerData>(stats.Profile.EquippedLazer);
        if (shooter.lazers.Contains(equipped) && ProfileManager.Owns(equipped))
            return equipped;

        foreach (LazerData lazer in shooter.lazers)
        {
            if (ProfileManager.Owns(lazer))
                return lazer;
        }
        return null;
    }

    void Fire(GameObject model, float size, Vector3 rotationOffset, float speed, float damage, float distance, float radius)
    {
        Vector3 spawnPos = transform.position + aimDirection * (PlayerRadius() + 0.3f); // just outside the player
        Projectile.Spawn(model, spawnPos, aimDirection, size, rotationOffset, speed, damage, distance, radius, false);
    }

    void Swing(MeleeData melee)
    {
        Vector3 centre = transform.position + aimDirection * (PlayerRadius() + melee.radius * 0.5f);

        Damage.HitArea(centre, melee.radius, melee.damage);

        // Flash the model briefly if it has one (punch has none)
        if (melee.model != null)
            Destroy(Instantiate(melee.model, centre, Quaternion.LookRotation(aimDirection)), 0.15f);
    }

    // Half the player's width, so it works at any scale
    float PlayerRadius()
    {
        Collider col = GetComponent<Collider>();
        return col != null ? col.bounds.extents.x : 0.5f;
    }

    void UpdateArrow()
    {
        if (aimArrow == null)
            return;

        aimArrow.position = transform.position + aimDirection * arrowDistance + Vector3.up * arrowHeight;

        // Lay the sprite flat on the ground with its top pointing along the aim
        aimArrow.rotation = Quaternion.LookRotation(aimDirection) * Quaternion.Euler(90f, 0f, arrowSpin);
    }

    // Where the mouse points on a flat plane at the player's height
    Vector3 GetAimDirection()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 dir = ray.GetPoint(distance) - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                return dir.normalized;
        }
        return aimDirection;
    }

    [ContextMenu("Unlock All Weapons (testing)")]
    void UnlockAll()
    {
        foreach (WeaponData weapon in ItemDatabase.Instance.GetAll<WeaponData>())
            ProfileManager.Profile.AddOwned(weapon.id);
    }
}
