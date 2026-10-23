// ============================================================
// Title:       MobSpawner
// Version:     1.4
// Written by:  Ashar
// Updated by:  cthullu96 (v1.4: max alive at once so the maze doesn't fill up)
// Description: Spawns mobs around this point. Each row in Spawns
//              is a mob + how many to spawn, weakest first.
//              Row 0 starts straight away, every 5s.
//              Each next row starts 30s later and spawns 4s slower
//              (row 1: after 30s, every 9s. row 2: after 60s, every 13s...)
//              Nothing spawns until the player is within Activation Radius.
//              Remaining(mob) = how many of that mob are still to be killed.
//              max alive = most mobs from this spawner alive at once.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public MobData mob;
        public int amount = 10;
    }

    public SpawnEntry[] spawns;

    [Header("UI")]
    public GameObject healthBarPrefab;   //UI/MobHealthBar

    [Header("Timing")]
    public float firstInterval = 5f;
    public float delayPerRow = 30f;
    public float intervalPerRow = 4f;

    [Header("Area")]
    public float spawnRadius = 1.5f;
    public float activationRadius = 20f;
    public int maxAlive = 4;

    //spawner state
    private Transform player;
    private bool started;
    private int[] remaining;             //per row: not spawned yet + alive
    private int alive;

    //STARTING PROCEDURES
    void Awake()
    {
        remaining = new int[spawns.Length];
        for (int i = 0; i < spawns.Length; i++)
            remaining[i] = spawns[i].mob != null ? spawns[i].amount : 0;
    }

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    //ACTIVATION
    void Update()
    {
        if (started || player == null)
            return;

        Vector3 a = transform.position, b = player.position;
        a.y = 0f;
        b.y = 0f;
        if (Vector3.Distance(a, b) > activationRadius)
            return;

        started = true;
        for (int i = 0; i < spawns.Length; i++)
            StartCoroutine(SpawnRow(i));
    }

    //QUERIES
    public int Remaining(MobData mob)
    {
        int total = 0;
        for (int i = 0; i < spawns.Length; i++)
        {
            if (spawns[i].mob == mob)
                total += remaining[i];
        }
        return total;
    }

    public IEnumerable<MobData> MobTypes()
    {
        foreach (SpawnEntry entry in spawns)
        {
            if (entry.mob != null)
                yield return entry.mob;
        }
    }

    //SPAWNING
    IEnumerator SpawnRow(int row)
    {
        SpawnEntry entry = spawns[row];
        if (entry.mob == null || entry.mob.model == null)
            yield break;

        float delay = row * delayPerRow;
        float interval = firstInterval + row * intervalPerRow;

        yield return new WaitForSeconds(delay);

        for (int n = 0; n < entry.amount; n++)
        {
            //wait for one to die
            while (alive >= maxAlive)
                yield return null;

            Spawn(entry.mob, row);
            yield return new WaitForSeconds(interval);
        }
    }

    void Spawn(MobData mobData, int row)
    {
        Vector2 offset = Random.insideUnitCircle * spawnRadius;
        Vector3 pos = transform.position + new Vector3(offset.x, 0f, offset.y);

        GameObject mob = Instantiate(mobData.model, pos, Quaternion.identity);
        mob.name = mobData.displayName;
        StandOnGround(mob, transform.position.y);

        //mob type
        Mob m = mobData.type == MobType.Shooter ? mob.AddComponent<ShooterMob>() : (Mob)mob.AddComponent<FollowMob>();
        m.Init(mobData, healthBarPrefab);
        alive++;
        m.onDied = () => { remaining[row]--; alive--; };
    }

    //bottom of the model sits at groundY
    void StandOnGround(GameObject mob, float groundY)
    {
        Renderer[] renderers = mob.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        mob.transform.position += Vector3.up * (groundY - bounds.min.y);
    }

    //GIZMOS
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}
