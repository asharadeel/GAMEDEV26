using System.Collections;
using UnityEngine;

// the level 1 boss, a big pill that lives in the hex room
// it walks at the player and keeps turning to face them
// moves: jump slam (aoe), throw boulders, swing its club up close
// it only takes a little damage while its pillars are standing
// break all the pillars and it gets weak for a while, then they rebuild
// when it dies the portal piece shows up in the middle of the room
public class BossMob : Mob
{
    [Header("setup")]
    public MobData bossData;
    public GameObject healthBarPrefab;
    public Transform arenaCentre;
    public float arenaRadius = 30f;      // fight starts when the player is this close to the middle
    public BossPillar[] pillars;
    public GameObject portalPiece;       // hidden until the boss dies

    [Header("defence")]
    public float shieldedDamage = 0.2f;  // only takes 20% damage while any pillar is up
    public float weakTime = 12f;         // seconds it stays weak before the pillars come back
    public Renderer body;
    public Color shieldedColour = new Color(0.35f, 0.2f, 0.5f);
    public Color weakColour = new Color(0.9f, 0.35f, 0.2f);
    public GameObject shield;            // see through bubble shown while shielded

    [Header("moves")]
    public float restBetweenMoves = 1.5f;

    [Header("move 1 - jump slam")]
    public float slamDamage = 30f;
    public float slamRadius = 6f;
    public float jumpTime = 1.1f;
    public float jumpHeight = 8f;
    public GameObject slamWarning;       // red ring on the floor where it will land

    [Header("move 2 - boulders")]
    public GameObject boulderModel;
    public Transform hand;
    public GameObject heldBoulder;       // the boulder you see in its hand before it throws
    public int bouldersPerThrow = 3;
    public float boulderDamage = 20f;
    public float boulderSpeed = 18f;

    [Header("move 3 - club")]
    public Transform clubPivot;
    public float clubDamage = 25f;
    public float clubRange = 5f;

    private Rigidbody rb;
    private bool fighting;
    private bool walking;
    private bool lockFacing;
    private bool weak;
    private bool dead;
    private Rigidbody playerRb;
    private Vector3 heldSize;

    void Start()
    {
        Init(bossData, healthBarPrefab);

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (player != null)
            playerRb = player.GetComponent<Rigidbody>();

        foreach (BossPillar p in pillars)
            p.boss = this;

        if (portalPiece != null)
            portalPiece.SetActive(false);
        if (slamWarning != null)
            slamWarning.SetActive(false);
        if (heldBoulder != null)
        {
            heldSize = heldBoulder.transform.localScale;
            heldBoulder.SetActive(false);
        }

        SetShielded(true);
    }

    void Update()
    {
        // wait for the player to walk into the room
        if (!fighting && !dead && player != null)
        {
            Vector3 a = player.position;
            Vector3 b = arenaCentre.position;
            a.y = 0f;
            b.y = 0f;
            if (Vector3.Distance(a, b) <= arenaRadius)
                StartCoroutine(Fight());
        }
    }

    void FixedUpdate()
    {
        if (!fighting || dead || data == null)
            return;

        // keep turning to face the player
        if (!lockFacing)
        {
            Vector3 dir = DirectionToPlayer();
            if (dir != Vector3.zero)
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(dir), 8f * Time.fixedDeltaTime));
        }

        if (rb.isKinematic)
            return;

        // walk at the player, stop when close enough to hit
        Vector3 move = Vector3.zero;
        if (walking && DistanceToPlayer() > clubRange - 1f)
            move = DirectionToPlayer() * data.moveSpeed * SpeedMultiplier;

        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
    }

    // the fight loop

    IEnumerator Fight()
    {
        fighting = true;
        FloatingText.Show(transform.position + Vector3.up * 7f, data.displayName + " wakes up!");
        yield return new WaitForSeconds(1.5f);

        while (!dead)
        {
            // walk for a bit, or until the player is close
            walking = true;
            float until = Time.time + restBetweenMoves;
            while (Time.time < until && DistanceToPlayer() > clubRange)
                yield return null;
            walking = false;

            // pick a move
            float distance = DistanceToPlayer();
            if (distance <= clubRange)
                yield return StartCoroutine(Club());
            else if (Random.value < 0.5f)
                yield return StartCoroutine(Slam());
            else
                yield return StartCoroutine(Boulders());
        }
    }

    // move 1: jump up and slam down

    IEnumerator Slam()
    {
        rb.velocity = Vector3.zero;

        // land where the player is, but stay inside the room
        Vector3 target = FlatPlayerPos();
        Vector3 fromCentre = target - arenaCentre.position;
        fromCentre.y = 0f;
        if (fromCentre.magnitude > arenaRadius - 3f)
            target = arenaCentre.position + fromCentre.normalized * (arenaRadius - 3f);
        target.y = transform.position.y;

        // show the warning ring
        if (slamWarning != null)
        {
            slamWarning.SetActive(true);
            slamWarning.transform.position = new Vector3(target.x, transform.position.y + 0.05f, target.z);
            slamWarning.transform.localScale = new Vector3(slamRadius * 2f, slamRadius * 2f, 1f);
        }

        // crouch before jumping
        yield return StartCoroutine(Squash(0.75f, 0.3f));

        // jump in an arc
        rb.isKinematic = true;
        Vector3 start = transform.position;
        for (float t = 0f; t < 1f; t += Time.fixedDeltaTime / jumpTime)
        {
            Vector3 pos = Vector3.Lerp(start, target, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;
            rb.MovePosition(pos);
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(target);
        yield return new WaitForFixedUpdate();
        rb.isKinematic = false;

        // land and hurt the player if they are inside the ring
        if (slamWarning != null)
            slamWarning.SetActive(false);
        MakeShockwave(target, slamRadius);
        if (FlatDistance(player.position, target) <= slamRadius)
            player.SendMessage("TakeDamage", slamDamage, SendMessageOptions.DontRequireReceiver);

        yield return StartCoroutine(Squash(0.8f, 0.15f));
        yield return new WaitForSeconds(0.8f);   // gap so the player can hit back
    }

    // move 2: throw boulders

    IEnumerator Boulders()
    {
        rb.velocity = Vector3.zero;

        for (int i = 0; i < bouldersPerThrow && !dead; i++)
        {
            // boulder grows in its hand
            heldBoulder.SetActive(true);
            for (float t = 0f; t < 1f; t += Time.deltaTime / 0.5f)
            {
                heldBoulder.transform.localScale = heldSize * t;
                yield return null;
            }

            // aim a little ahead of where the player is running
            Vector3 aim = player.position;
            if (playerRb != null)
            {
                float flightTime = FlatDistance(hand.position, player.position) / boulderSpeed;
                Vector3 v = playerRb.velocity;
                v.y = 0f;
                aim += v * flightTime;
            }
            Vector3 dir = aim - hand.position;
            dir.y = 0f;
            dir.Normalize();

            heldBoulder.SetActive(false);
            Projectile.Spawn(boulderModel, hand.position, dir, 1f, Vector3.zero, boulderSpeed, boulderDamage, 80f, 0f, true);

            yield return new WaitForSeconds(0.35f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    // move 3: swing the club

    IEnumerator Club()
    {
        rb.velocity = Vector3.zero;

        // pull the club back
        yield return StartCoroutine(TurnClub(60f, 130f, 0.45f));

        // swing across the front, stop turning so the player can dodge
        lockFacing = true;
        yield return StartCoroutine(TurnClub(130f, 0f, 0.09f));

        // hit if the player is close and in front
        Vector3 toPlayer = DirectionToPlayer();
        if (DistanceToPlayer() <= clubRange && Vector3.Angle(transform.forward, toPlayer) < 75f)
            player.SendMessage("TakeDamage", clubDamage, SendMessageOptions.DontRequireReceiver);

        yield return StartCoroutine(TurnClub(0f, -60f, 0.09f));
        lockFacing = false;

        // put it back
        yield return StartCoroutine(TurnClub(-60f, 60f, 0.4f));
        yield return new WaitForSeconds(0.4f);
    }

    IEnumerator TurnClub(float from, float to, float time)
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime / time)
        {
            clubPivot.localRotation = Quaternion.Euler(0f, Mathf.Lerp(from, to, t), 0f);
            yield return null;
        }
        clubPivot.localRotation = Quaternion.Euler(0f, to, 0f);
    }

    // taking damage and the pillars

    public override void TakeDamage(float amount)
    {
        if (dead)
            return;

        float damage = weak ? amount : amount * shieldedDamage;

        if (health - damage <= 0f)
            Die();

        base.TakeDamage(damage);   // this destroys the boss when health hits 0

        if (!dead)
            StartCoroutine(Flash());
    }

    // a pillar calls this when it breaks
    public void PillarBroken()
    {
        foreach (BossPillar p in pillars)
        {
            if (!p.broken)
                return;
        }

        // every pillar is down
        SetShielded(false);
        FloatingText.Show(transform.position + Vector3.up * 7f, "Shield down!");
        Invoke("RebuildPillars", weakTime);
    }

    void RebuildPillars()
    {
        if (dead)
            return;

        foreach (BossPillar p in pillars)
            p.Rebuild();

        SetShielded(true);
        FloatingText.Show(transform.position + Vector3.up * 7f, "Shield back up!");
    }

    void SetShielded(bool on)
    {
        weak = !on;
        if (shield != null)
            shield.SetActive(on);
        if (body != null)
            body.material.color = on ? shieldedColour : weakColour;
    }

    void Die()
    {
        dead = true;
        StopAllCoroutines();
        CancelInvoke();

        if (slamWarning != null)
            slamWarning.SetActive(false);

        // every pillar breaks with the boss (added by Ashar)
        foreach (BossPillar p in pillars)
        {
            if (p != null)
                p.Shatter();
        }

        if (portalPiece != null)
        {
            portalPiece.transform.position = arenaCentre.position;
            portalPiece.SetActive(true);
        }

        FloatingText.Show(transform.position + Vector3.up * 5f, data.displayName + " defeated!");
    }

    // little effects

    // body goes white for a moment when hit
    IEnumerator Flash()
    {
        if (body == null)
            yield break;
        body.material.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        body.material.color = weak ? weakColour : shieldedColour;
    }

    // squashes the body down then back up
    IEnumerator Squash(float amount, float time)
    {
        Transform b = body.transform;
        Vector3 normal = b.localScale;
        Vector3 squashed = new Vector3(normal.x * 1.15f, normal.y * amount, normal.z * 1.15f);
        for (float t = 0f; t < 1f; t += Time.deltaTime / time)
        {
            b.localScale = Vector3.Lerp(normal, squashed, Mathf.Sin(t * Mathf.PI));
            yield return null;
        }
        b.localScale = normal;
    }

    // flat ring that spreads out and fades where it lands
    void MakeShockwave(Vector3 centre, float radius)
    {
        GameObject wave = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(wave.GetComponent<Collider>());
        wave.transform.position = centre + Vector3.up * 0.1f;
        Renderer r = wave.GetComponent<Renderer>();
        r.material.color = weakColour;
        wave.AddComponent<Shockwave>().radius = radius;
    }

    Vector3 FlatPlayerPos()
    {
        Vector3 p = player.position;
        p.y = transform.position.y;
        return p;
    }

    float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }
}
