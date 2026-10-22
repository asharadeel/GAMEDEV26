// ============================================================
// Title:       ReadWritePlayerProfile
// Version:     1.2
// Written by:  Ashar
// Description: For the Library in the Hub. Reads the saved player
//              profile and achievements and writes them out as text
//              so the player can track their progress: level, tokens,
//              stats, equipped gear, owned items, inventory and
//              achievements. Refreshes every time it's shown.
//              Title = the player's name, body = their stats.
//              If no profile/name is found it shows a "Profile not
//              loaded" message instead.
//              Put it on the Library's Frame (e.g. a GeneralTextWindow)
//              and drag the body Text (Content/Context) into Body Text.
// ============================================================

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ReadWritePlayerProfile : MonoBehaviour
{
    public Text titleText;               // player name goes here, e.g. Content/Title1
    public Text bodyText;                // stats go here, e.g. Content/Context

    [Header("Shown when no profile is found")]
    public string notLoadedTitle = "Profile not loaded";
    [TextArea]
    public string notLoadedBody = "Profile has not generated... something gone wrong :/";

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        string playerName = GetPlayerName();
        bool loaded = ProfileManager.Profile != null && !string.IsNullOrEmpty(playerName);

        if (titleText != null)
            titleText.text = loaded ? playerName : notLoadedTitle;
        if (bodyText != null)
            bodyText.text = loaded ? BuildText() : notLoadedBody;
    }

    // Empty if no profile was picked on the Main Menu, so the "Profile not loaded" message shows
    string GetPlayerName()
    {
        if (!ProfileManager.HasProfile)
            return "";
        return ProfileManager.Profile.PlayerName;
    }

    string BuildText()
    {
        PlayerProfile p = ProfileManager.Profile;
        ItemDatabase db = ItemDatabase.Instance;
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<b>Level</b> " + p.Level);
        sb.AppendLine("<b>Tokens</b> " + p.Tokens);
        sb.AppendLine();

        sb.AppendLine("<b>Stats</b>");
        sb.AppendLine("- Health: " + Mathf.CeilToInt(p.Health) + " / " + PlayerProfile.MaxStat);
        sb.AppendLine("- Stamina: " + Mathf.CeilToInt(p.Stamina) + " / " + PlayerProfile.MaxStat);
        sb.AppendLine("- Hunger: " + Mathf.CeilToInt(p.Hunger) + " / " + PlayerProfile.MaxStat);
        sb.AppendLine("- Thirst: " + Mathf.CeilToInt(p.Thirst) + " / " + PlayerProfile.MaxStat);
        sb.AppendLine();

        sb.AppendLine("<b>Equipped</b>");
        sb.AppendLine("- Shooter: " + NameOf(db, p.EquippedShooter) + " (" + NameOf(db, p.EquippedLazer) + ")");
        sb.AppendLine("- Melee: " + NameOf(db, p.EquippedMelee));
        sb.AppendLine("- Special: " + NameOf(db, p.EquippedSpecial));
        sb.AppendLine();

        sb.AppendLine("<b>Owned Weapons</b>");
        List<string> owned = OwnedWeaponNames(db);
        sb.AppendLine(owned.Count > 0 ? "- " + string.Join("\n- ", owned) : "- None yet");
        sb.AppendLine();

        sb.AppendLine("<b>Inventory</b> (" + p.InventorySlots + " slots)");
        bool anyItem = false;
        for (int i = 0; i < p.InventorySlots; i++)
        {
            string id = p.GetSlot(i);
            if (string.IsNullOrEmpty(id))
                continue;
            string key = i == PlayerProfile.MaxInventorySlots - 1 ? "0" : (i + 4).ToString();
            sb.AppendLine("- [" + key + "] " + NameOf(db, id));
            anyItem = true;
        }
        if (!anyItem)
            sb.AppendLine("- Empty");
        sb.AppendLine();

        AppendAchievements(sb);
        return sb.ToString().TrimEnd();
    }

    void AppendAchievements(StringBuilder sb)
    {
        // Adding 0 makes the manager load the saved progress without changing anything
        AchievementManager.Add(AchievementStat.MobsKilled, 0);

        int unlocked = 0;
        foreach (AchievementData a in AchievementManager.All)
        {
            if (AchievementManager.IsUnlocked(a.id))
                unlocked++;
        }

        sb.AppendLine("<b>Achievements</b> (" + unlocked + " / " + AchievementManager.All.Count + ")");
        foreach (AchievementData a in AchievementManager.All)
        {
            bool done = AchievementManager.IsUnlocked(a.id);
            int count = Mathf.Min(AchievementManager.GetCount(a.stat), a.goal);
            sb.AppendLine((done ? "[x] " : "[ ] ") + a.title + " - " + a.description + " (" + count + "/" + a.goal + ")");
        }
    }

    // Every weapon the player can use (bought or unlocked from the start)
    List<string> OwnedWeaponNames(ItemDatabase db)
    {
        List<string> names = new List<string>();
        if (db == null)
            return names;

        foreach (WeaponData w in db.GetAll<WeaponData>())
        {
            if (ProfileManager.Owns(w))
                names.Add(w.displayName);
        }
        return names;
    }

    string NameOf(ItemDatabase db, string id)
    {
        if (string.IsNullOrEmpty(id))
            return "None";
        if (db == null)
            return id;

        ItemData item = db.Get<ItemData>(id);
        return item != null ? item.displayName : id;
    }
}
