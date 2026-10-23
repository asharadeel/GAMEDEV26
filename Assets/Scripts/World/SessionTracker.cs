// ============================================================
// Title:       SessionTracker
// Version:     1.0
// Written by:  Ashar
// Description: The level's own log for this run: time survived,
//              mobs killed and everything picked up. Mob and Pickup
//              report to it, the death screen reads it.
//              One per level scene (on the DeathScreen container).
// ============================================================

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SessionTracker : MonoBehaviour
{
    public static SessionTracker Instance { get; private set; }

    //session details
    private float startTime;
    private readonly Dictionary<string, int> kills = new Dictionary<string, int>();
    private readonly Dictionary<string, int> pickups = new Dictionary<string, int>();

    //STARTING PROCEDURES
    void Awake()
    {
        Instance = this;
        startTime = Time.time;
    }

    //LOGGING (safe to call when there's no tracker in the scene)
    public static void AddKill(string mobName)
    {
        if (Instance != null)
            Add(Instance.kills, mobName);
    }

    public static void AddPickup(string itemName)
    {
        if (Instance != null)
            Add(Instance.pickups, itemName);
    }

    static void Add(Dictionary<string, int> log, string key)
    {
        if (!log.ContainsKey(key))
            log[key] = 0;
        log[key]++;
    }

    //REPORT
    public float TimeSurvived
    {
        get { return Time.time - startTime; }
    }

    public string Report()
    {
        int seconds = Mathf.FloorToInt(TimeSurvived);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Time survived: " + (seconds / 60) + "m " + (seconds % 60) + "s");
        sb.AppendLine();
        sb.AppendLine("<b>Mobs killed</b>");
        AppendLog(sb, kills, "None");
        sb.AppendLine();
        sb.AppendLine("<b>Picked up</b> (kept in your profile)");
        AppendLog(sb, pickups, "Nothing");
        return sb.ToString().TrimEnd();
    }

    void AppendLog(StringBuilder sb, Dictionary<string, int> log, string empty)
    {
        if (log.Count == 0)
        {
            sb.AppendLine("- " + empty);
            return;
        }
        foreach (KeyValuePair<string, int> entry in log)
            sb.AppendLine("- " + entry.Key + " x" + entry.Value);
    }
}
