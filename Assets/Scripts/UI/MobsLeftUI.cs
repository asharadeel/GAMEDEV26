// ============================================================
// Title:       MobsLeftUI
// Version:     1.1
// Written by:  Ashar
// Description: "Mobs Left" tracker at the top of the screen.
//              Adds up every spawner in the scene and shows how
//              many of each mob are still to be killed,
//              e.g. "Lvl 1 x 120   Lvl 2 x 60   Lvl 3 x 30".
// ============================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobsLeftUI : MonoBehaviour
{
    public Text countsText;

    private MobSpawner[] spawners;

    void Start()
    {
        spawners = FindObjectsByType<MobSpawner>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (countsText == null || spawners == null)
            return;

        // Every mob type across all spawners, in the order they first appear (weakest first)
        List<MobData> types = new List<MobData>();
        foreach (MobSpawner spawner in spawners)
        {
            foreach (MobData mob in spawner.MobTypes())
            {
                if (!types.Contains(mob))
                    types.Add(mob);
            }
        }

        string text = "";
        foreach (MobData mob in types)
        {
            int left = 0;
            foreach (MobSpawner spawner in spawners)
                left += spawner.Remaining(mob);

            string label = string.IsNullOrEmpty(mob.shortName) ? mob.displayName : mob.shortName;
            text += (text == "" ? "" : "     ") + label + " x " + left;
        }

        countsText.text = text;
    }
}
