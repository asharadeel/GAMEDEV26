// ============================================================
// Title:       ReadWritePlayerProfile
// Version:     1.4
// Written by:  Ashar
// Updated by:  Ashar (v1.4: armour and ores, slot keys 4-9)
// Description: For the Library in the Hub. Reads the saved player
//              profile and achievements and writes them out as text
//              so the player can track their progress: level, coins,
//              stats, equipped gear, owned items, inventory and
//              achievements. Refreshes every time it's shown.
//              Title = the player's name, body = their stats.
//              If no profile/name is found it shows a "Profile not
//              loaded" message instead.
//              Put it on the Library's Frame (e.g. a GeneralTextWindow)
//              and drag the body Text (Content/Context) into Body Text.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ReadWritePlayerProfile : MonoBehaviour
{
    //interface
    public Text titleText; //player name
    public Text bodyText; //stats

    [Header("Shown when no profile is found")]
    public string notLoadedTitle = "Profile not loaded";
    [TextArea]
    public string notLoadedBody = "Profile has not generated... something gone wrong :/";

    //STARTING PROCEDURES
    void OnEnable()
    {
        Refresh();
        Debug.Log("Opened player profile.");
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

    //empty name = shows not loaded message
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

        //LEVEL & COINS
        sb.AppendLine("<b>Level</b> " + p.Level);
        sb.AppendLine("<b>Coins</b> " + p.Coins);
        sb.AppendLine();

        //STATS
        sb.AppendLine("<b>Stats</b>");
        sb.AppendLine("- Health: " + Mathf.CeilToInt(p.Health) + " / " + PlayerProfile.MaxStat);
        sb.AppendLine("- Stamina: " + Mathf.CeilToInt(p.Stamina) + " / " + PlayerProfile.MaxStat);
        sb.AppendLine("- Hunger: " + Mathf.CeilToInt(p.Hunger) + " / " + PlayerProfile.MaxStat);
        sb.AppendLine("- Thirst: " + Mathf.CeilToInt(p.Thirst) + " / " + PlayerProfile.MaxStat);
        sb.AppendLine();

        //EQUIPPED
        sb.AppendLine("<b>Equipped</b>");
        ShooterData shooter = db != null ? db.Get<ShooterData>(p.EquippedShooter) : null;
        LazerData lazer = shooter != null ? PlayerCombat.GetLazer(shooter) : null;
        sb.AppendLine("- Shooter: " + NameOf(db, p.EquippedShooter) + LevelOf(shooter) + (lazer != null ? " (" + lazer.displayName + ")" : ""));
        sb.AppendLine("- Melee: " + NameOf(db, p.EquippedMelee) + LevelOf(db != null ? db.Get<MeleeData>(p.EquippedMelee) : null));
        sb.AppendLine();

        //OWNED WEAPONS
        sb.AppendLine("<b>Owned Weapons</b>");
        List<string> owned = OwnedWeaponNames(db);
        sb.AppendLine(owned.Count > 0 ? "- " + string.Join("\n- ", owned) : "- None yet");
        sb.AppendLine();

        //INVENTORY
        sb.AppendLine("<b>Inventory</b> (" + p.InventorySlots + " slots)");
        bool anyItem = false;
        for (int i = 0; i < p.InventorySlots; i++)
        {
            string id = p.GetSlot(i);
            if (string.IsNullOrEmpty(id))
                continue;
            sb.AppendLine("- [" + (i + 4) + "] " + NameOf(db, id));
            anyItem = true;
        }
        if (!anyItem)
            sb.AppendLine("- Empty");
        sb.AppendLine();

        //ARMOUR AND ORES
        sb.AppendLine("<b>Armor</b>");
        bool anyArmor = false;
        if (db != null)
        {
            foreach (ArmorData armor in db.armor)
            {
                int count = armor != null ? p.GetArmorCount(armor.id) : 0;
                if (count <= 0)
                    continue;
                string worn = ProfileManager.IsArmorEquipped(armor) ? " (worn, shield " + Mathf.CeilToInt(p.ArmorShield) + ")" : "";
                sb.AppendLine("- " + armor.displayName + " x" + count + worn);
                anyArmor = true;
            }
        }
        if (!anyArmor)
            sb.AppendLine("- None");

        sb.Append("<b>Ores</b>: ");
        List<string> ores = new List<string>();
        if (db != null)
        {
            foreach (OreData ore in db.ores)
            {
                if (ore != null)
                    ores.Add(ore.displayName + " " + p.GetOreCount(ore.id));
            }
        }
        sb.AppendLine(ores.Count > 0 ? string.Join(", ", ores) : "none");
        sb.AppendLine();

        //ACHIEVEMENTS
        AppendAchievements(sb);
        return sb.ToString().TrimEnd();
    }

    void AppendAchievements(StringBuilder sb)
    {
        //adding 0 forces the saved progress to load
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

    //HELPERS
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

    //shooters use upgrade bars, not levels
    string LevelOf(WeaponData weapon)
    {
        return weapon != null && !(weapon is ShooterData) ? " - Level " + ProfileManager.GetLevel(weapon) : "";
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
