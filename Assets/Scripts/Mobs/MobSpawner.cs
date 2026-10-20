// ============================================================
// Title:       MobSpawner
// Version:     1.2
// Written by:  Ashar
// Description: Spawns mobs around this point. Each row in Spawns
//              is a mob + how many to spawn, weakest first.
//              Row 0 starts straight away, every 5s.
//              Each next row starts 30s later and spawns 4s slower
//              (row 1: after 30s, every 9s. row 2: after 60s, every 13s...)
// ============================================================

using System.Collections;
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
    public GameObject healthBarPrefab;   // floats above each mob (UI/MobHealthBar)

    [Header("Timing")]
    public float firstInterval = 5f;     // row 0 spawns this often
    public float delayPerRow = 30f;      // each row starts this much later than the one before
    public float intervalPerRow = 4f;    // each row spawns this much slower than the one before

    [Header("Area")]
    public float spawnRadius = 1.5f;     // random spread around the spawner

    void Start()
    {
        for (int i = 0; i < spawns.Length; i++)
            StartCoroutine(SpawnRow(i));
    }

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
            Spawn(entry.mob);
            yield return new WaitForSeconds(interval);
        }
    }

    void Spawn(MobData mobData)
    {
        Vector2 offset = Random.insideUnitCircle * spawnRadius;
        Vector3 pos = transform.position + new Vector3(offset.x, 0f, offset.y);

        GameObject mob = Instantiate(mobData.model, pos, Quaternion.identity);
        mob.name = mobData.displayName;
        StandOnGround(mob, transform.position.y);

        // The profile's type decides which kind of mob it becomes
        Mob m = mobData.type == MobType.Shooter ? mob.AddComponent<ShooterMob>() : (Mob)mob.AddComponent<FollowMob>();
        m.Init(mobData, healthBarPrefab);
    }

    // Moves the mob up/down so the bottom of its model sits at groundY
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

    // Shows the spawn area in the Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
