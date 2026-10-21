// ============================================================
// Title:       AchievementPanel
// Version:     1.0
// Written by:  Davud Jabiyev
// Description: A list of all achievements. J opens and closes it.
//              Each row shows [x] when unlocked and the progress,
//              e.g. "[ ] Slayer - Kill 10 mobs (7/10)".
// ============================================================

using UnityEngine;
using UnityEngine.UI;

public class AchievementPanel : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.J;
    public GameObject panel;             // the UI panel to show / hide
    public Text[] rows;                  // UI Text instances, one per achievement, in order
    
    // Future TODO: Event driven instead of hooking into Update()

    void Start()
    {
        // Make sure the list of all achievements is filled
        AchievementManager.Add(AchievementStat.MobsKilled, 0);

        // Start game with panel closed
        if (panel != null) {
            panel.SetActive(false);
        }
    }

    void Update()
    {
        // Open / Close toggle panel with the key
        if (Input.GetKeyDown(toggleKey) && panel != null) {
            panel.SetActive(!panel.activeSelf);
        }

        // Only update the text while it is visible
        if (panel != null && panel.activeSelf) {
            RefreshRows();
        }
    }

    void RefreshRows()
    {
        // Foreach row (UI Text instance)
        for (int i = 0; i < rows.Length; i++)
        {
            // Get achievement data by index
            AchievementData a = AchievementManager.All[i];
            bool done = AchievementManager.IsUnlocked(a.id);

            // Extract progress, but never more than the goal
            int count = AchievementManager.GetCount(a.stat);
            if (count > a.goal) {
                count = a.goal;
            }

            // "Render" Text
            string box = "[-]";
            if (done) {
                box = "[x]";
            }
            rows[i].text = box + " " + a.title + " - " + a.description + " (" + count + "/" + a.goal + ")";
            rows[i].color = done ? Color.yellow : Color.white;
        }
    }
}
