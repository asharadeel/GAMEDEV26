// ============================================================
// Title:       PlayerCombat
// Version:     2.1
// Written by:  Ashar
// Updated by:  cthullu96 (v1.7: capsule fist shows while punch is selected)
//              Ali (v1.9: shooter magazine + reload, shooter upgrade bars,
//              v2.0: special slot removed (abilities on Q/E), aim shared with PlayerAbilities)
//              cthullu96 (v2.1: input system package)
//              ai use: input system change written with claude (anthropic), owner cthullu96
// Description: Aiming, weapon slots and attacking. Each weapon
//              uses the stats of its current upgrade level.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//goes on the Player. slot 1 shooter, slot 2 melee
[RequireComponent(typeof(PlayerStats))]
public class PlayerCombat : MonoBehaviour
{
    public Camera cam;

    [Header("Aim Arrow")]
    public Transform aimArrow;          //optional
    public float arrowDistance = 1f;
    public float arrowHeight = -0.45f;  //negative = near the floor
    public float arrowSpin = 0f;        //extra turn if the image doesn't point up

    [Header("Punch")]
    public Color fistColour = new Color(0.85f, 0.7f, 0.55f);

    //combat state
    private PlayerStats stats;
    private GameObject fist;
    private float punchTime = -10f;
    private Vector3 aimDirection = Vector3.forward;
    private float nextAttackTime;

    //magazine
    private ShooterData loadedGun;
    private int shotsLeft;
    private float reloadDoneTime;

    public bool Reloading
    {
        get { return Time.time < reloadDoneTime; }
    }

    //STARTING PROCEDURES
    void Start()
    {
        stats = GetComponent<PlayerStats>();
        if (cam == null)
            cam = Camera.main;

        MakeFist();
    }

    //INPUT
    void Update()
    {
        aimDirection = GetAimDirection();
        UpdateArrow();
        UpdateFist();

        //no switching or attacking while a menu is open
        if (PlayerController.uiEnabled)
            return;

        if (GameInput.Weapon1.WasPressedThisFrame()) stats.Profile.SelectedWeapon = 0;
        if (GameInput.Weapon2.WasPressedThisFrame()) stats.Profile.SelectedWeapon = 1;

        if (GameInput.Reload.WasPressedThisFrame() && stats.Profile.SelectedWeapon == 0)
        {
            ShooterData gun = ItemDatabase.Instance.Get<ShooterData>(stats.Profile.EquippedShooter);
            if (GetLazer(gun) != null)
            {
                CheckMagazine(gun);
                Reload(gun);
            }
        }

        //clicking on UI shouldn't also shoot
        bool clickOnUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (GameInput.Attack.IsPressed() && !clickOnUI)
            Attack();
    }

    //ATTACK
    public void Attack()
    {
        if (Time.time < nextAttackTime)
            return;

        ItemDatabase db = ItemDatabase.Instance;
        PlayerProfile p = stats.Profile;

        if (p.SelectedWeapon == 0)
        {
            //shooter
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
            //melee
            MeleeData melee = db.Get<MeleeData>(p.EquippedMelee);
            if (!ProfileManager.Owns(melee))
                return;

            MeleeLevel m = melee.AtLevel(ProfileManager.GetLevel(melee));
            nextAttackTime = Time.time + 1f / m.fireRate;
            Swing(m);
        }
    }

    //AIM / WEAPON INFO
    public Vector3 AimDirection
    {
        get { return aimDirection; }
    }

    public Vector3 ShotOrigin()
    {
        return transform.position + aimDirection * (PlayerRadius() + 0.3f);
    }

    //null if the gun isn't owned
    public static LazerData GetLazer(ShooterData shooter)
    {
        if (!ProfileManager.Owns(shooter))
            return null;
        return shooter.LazerAt(ProfileManager.GetLevel(shooter, ShooterStat.Lazer));
    }

    //HUD ammo text
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

    //MAGAZINE
    //different gun starts with a full magazine
    void CheckMagazine(ShooterData gun)
    {
        if (gun == loadedGun)
            return;
        loadedGun = gun;
        shotsLeft = gun.magazineSize;
        reloadDoneTime = 0f;
    }

    void Reload(ShooterData gun)
    {
        if (Reloading || shotsLeft >= gun.magazineSize)
            return;
        reloadDoneTime = Time.time + Mathf.Max(0f, ProfileManager.GetStat(gun, ShooterStat.ReloadTime));
        shotsLeft = gun.magazineSize;
    }

    //FIRE / SWING
    void Fire(GameObject model, float size, Vector3 rotationOffset, float speed, float damage, float distance, float radius, int pierce)
    {
        Projectile.Spawn(model, ShotOrigin(), aimDirection, size, rotationOffset, speed, damage, distance, radius, false, pierce);
    }

    void Swing(MeleeLevel melee)
    {
        Vector3 centre = transform.position + aimDirection * (PlayerRadius() + melee.radius * 0.5f);

        Damage.HitArea(centre, melee.radius, melee.damage);

        punchTime = Time.time;   //fist jab

        //punch has no model
        if (melee.model != null)
            Destroy(Instantiate(melee.model, centre, Quaternion.LookRotation(aimDirection)), 0.15f);
    }

    float PlayerRadius()
    {
        Collider col = GetComponent<Collider>();
        return col != null ? col.bounds.extents.x : 0.5f;
    }

    //FIST
    void MakeFist()
    {
        fist = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        fist.name = "Fist";
        Destroy(fist.GetComponent<Collider>());
        fist.GetComponent<Renderer>().material.color = fistColour;
        fist.transform.SetParent(transform, false);

        //same size at any player scale
        float s = transform.lossyScale.x;
        fist.transform.localScale = new Vector3(0.35f, 0.4f, 0.35f) / s;
        fist.SetActive(false);
    }

    void UpdateFist()
    {
        if (fist == null || stats == null)
            return;

        PlayerProfile p = stats.Profile;
        bool show = p.SelectedWeapon == 1 && p.EquippedMelee == "punch";
        fist.SetActive(show);
        if (!show)
            return;

        float jab = Time.time - punchTime < 0.12f ? 0.6f : 0f;
        fist.transform.position = transform.position + aimDirection * (PlayerRadius() + 0.3f + jab);
        fist.transform.rotation = Quaternion.LookRotation(aimDirection) * Quaternion.Euler(90f, 0f, 0f);
    }

    //AIMING
    void UpdateArrow()
    {
        if (aimArrow == null)
            return;

        aimArrow.position = transform.position + aimDirection * arrowDistance + Vector3.up * arrowHeight;

        //flat on the ground, top pointing along the aim
        aimArrow.rotation = Quaternion.LookRotation(aimDirection) * Quaternion.Euler(90f, 0f, arrowSpin);
    }

    //mouse on a flat plane at the player's height
    Vector3 GetAimDirection()
    {
        Ray ray = cam.ScreenPointToRay(GameInput.PointerPosition);
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

    //TESTING
    [ContextMenu("Unlock All Weapons (testing)")]
    void UnlockAll()
    {
        foreach (WeaponData weapon in ItemDatabase.Instance.GetAll<WeaponData>())
            ProfileManager.Profile.AddOwned(weapon.id);
        Debug.Log("Unlocked all weapons.");
    }
}
