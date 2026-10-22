// ============================================================
// Title:       PlayerCombat
// Version:     2.0
// Written by:  Ashar
// Updated by:  cthullu96 (v1.7: capsule fist shows while punch is selected)
//              Ali (v1.9: shooter magazine + reload, shooter upgrade bars,
//              v2.0: special slot removed (abilities on Q/E), aim shared with PlayerAbilities)
// Description: Aiming, weapon slots and attacking. Each weapon
//              uses the stats of its current upgrade level.
// ============================================================

using UnityEngine;
using UnityEngine.EventSystems;

// Goes on the Player. Two weapon slots: 1 shooter, 2 melee (abilities are on Q/E, see PlayerAbilities).
// 1/2 pick a slot. Left click or X attacks with the selected weapon.
// The shooter fires its magazine then reloads (R reloads early).
// Aims at the mouse, 360 degrees around the player.
[RequireComponent(typeof(PlayerStats))]
public class PlayerCombat : MonoBehaviour
{
    public Camera cam;
    public KeyCode attackKey = KeyCode.X;
    public KeyCode reloadKey = KeyCode.R;

    [Header("Aim Arrow")]
    public Transform aimArrow;          // optional sprite that circles the player, pointing at the mouse
    public float arrowDistance = 1f;    // how far from the player's centre
    public float arrowHeight = -0.45f;  // up/down from the player's centre (negative = near the floor)
    public float arrowSpin = 0f;        // extra turn if the image doesn't point up

    [Header("Punch")]
    public Color fistColour = new Color(0.85f, 0.7f, 0.55f);

    private PlayerStats stats;
    private GameObject fist;            // capsule in front of the player while punch is selected
    private float punchTime = -10f;
    private Vector3 aimDirection = Vector3.forward;
    private float nextAttackTime;

    private ShooterData loadedGun;      // the gun the magazine belongs to
    private int shotsLeft;
    private float reloadDoneTime;       // can't shoot until then

    public bool Reloading
    {
        get { return Time.time < reloadDoneTime; }
    }

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        if (cam == null)
            cam = Camera.main;

        MakeFist();
    }

    void Update()
    {
        aimDirection = GetAimDirection();
        UpdateArrow();
        UpdateFist();

        // No switching or attacking while a menu is open
        if (PlayerController.uiEnabled)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) stats.Profile.SelectedWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) stats.Profile.SelectedWeapon = 1;

        if (Input.GetKeyDown(reloadKey) && stats.Profile.SelectedWeapon == 0)
        {
            ShooterData gun = ItemDatabase.Instance.Get<ShooterData>(stats.Profile.EquippedShooter);
            if (GetLazer(gun) != null)
            {
                CheckMagazine(gun);
                Reload(gun);
            }
        }

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
            // The gun's upgrade bars decide the shot, its lazer decides the look and perks
            ShooterData gun = db.Get<ShooterData>(p.EquippedShooter);
            LazerData lazer = GetLazer(gun);
            if (lazer == null)
                return;

            CheckMagazine(gun);
            if (Reloading)
                return;

            shotsLeft--;
            nextAttackTime = Time.time + 1f / ProfileManager.GetStat(gun, ShooterStat.FireRate);
            Fire(lazer.model, lazer.size, lazer.rotationOffset, ProfileManager.GetStat(gun, ShooterStat.BulletSpeed),
                 ProfileManager.GetStat(gun, ShooterStat.Damage), lazer.distance, 0f, lazer.pierce);

            if (shotsLeft <= 0)
                Reload(gun);
        }
        else
        {
            MeleeData melee = db.Get<MeleeData>(p.EquippedMelee);
            if (!ProfileManager.Owns(melee))
                return;

            MeleeLevel m = melee.AtLevel(ProfileManager.GetLevel(melee));
            nextAttackTime = Time.time + 1f / m.fireRate;
            Swing(m);
        }
    }

    // Where the player is aiming (flat, towards the mouse)
    public Vector3 AimDirection
    {
        get { return aimDirection; }
    }

    // Just outside the player in the aim direction, where shots start
    public Vector3 ShotOrigin()
    {
        return transform.position + aimDirection * (PlayerRadius() + 0.3f);
    }

    // The lazer on the gun's lazer bar (null if the gun isn't owned)
    public static LazerData GetLazer(ShooterData shooter)
    {
        if (!ProfileManager.Owns(shooter))
            return null;
        return shooter.LazerAt(ProfileManager.GetLevel(shooter, ShooterStat.Lazer));
    }

    // "12 / 20", "Reloading..." or "" when the shooter isn't selected (for the HUD)
    public string AmmoLabel()
    {
        if (stats == null || stats.Profile.SelectedWeapon != 0)
            return "";

        ShooterData gun = ItemDatabase.Instance.Get<ShooterData>(stats.Profile.EquippedShooter);
        if (GetLazer(gun) == null)
            return "";

        CheckMagazine(gun);
        return Reloading ? "Reloading..." : shotsLeft + " / " + gun.magazineSize;
    }

    // A different gun (or the first shot) starts with a full magazine
    void CheckMagazine(ShooterData gun)
    {
        if (gun == loadedGun)
            return;
        loadedGun = gun;
        shotsLeft = gun.magazineSize;
        reloadDoneTime = 0f;
    }

    // Refills the magazine, the gun can't fire until its reload time has passed
    void Reload(ShooterData gun)
    {
        if (Reloading || shotsLeft >= gun.magazineSize)
            return;
        reloadDoneTime = Time.time + Mathf.Max(0f, ProfileManager.GetStat(gun, ShooterStat.ReloadTime));
        shotsLeft = gun.magazineSize;
    }

    void Fire(GameObject model, float size, Vector3 rotationOffset, float speed, float damage, float distance, float radius, int pierce)
    {
        Projectile.Spawn(model, ShotOrigin(), aimDirection, size, rotationOffset, speed, damage, distance, radius, false, pierce);
    }

    void Swing(MeleeLevel melee)
    {
        Vector3 centre = transform.position + aimDirection * (PlayerRadius() + melee.radius * 0.5f);

        Damage.HitArea(centre, melee.radius, melee.damage);

        punchTime = Time.time;   // makes the fist jab forward

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

    // a capsule so you can see punch is the selected weapon
    void MakeFist()
    {
        fist = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        fist.name = "Fist";
        Destroy(fist.GetComponent<Collider>());
        fist.GetComponent<Renderer>().material.color = fistColour;
        fist.transform.SetParent(transform, false);

        // same size whatever the player's scale is
        float s = transform.lossyScale.x;
        fist.transform.localScale = new Vector3(0.35f, 0.4f, 0.35f) / s;
        fist.SetActive(false);
    }

    // only shows while the melee slot is picked and the melee is punch
    void UpdateFist()
    {
        if (fist == null || stats == null)
            return;

        PlayerProfile p = stats.Profile;
        bool show = p.SelectedWeapon == 1 && p.EquippedMelee == "punch";
        fist.SetActive(show);
        if (!show)
            return;

        // sits just in front of the player pointing where they aim, jabs out for a moment when punching
        float jab = Time.time - punchTime < 0.12f ? 0.6f : 0f;
        fist.transform.position = transform.position + aimDirection * (PlayerRadius() + 0.3f + jab);
        fist.transform.rotation = Quaternion.LookRotation(aimDirection) * Quaternion.Euler(90f, 0f, 0f);
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
