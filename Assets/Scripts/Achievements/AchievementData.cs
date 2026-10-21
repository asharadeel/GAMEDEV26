// ============================================================
// Title:       AchievementData
// Version:     1.0
// Written by:  Davud Jabiyev
// Description: One achievement: a name, a description and a goal.
//              The goal is reached when the matching stat
//              (AchievementStat) gets to goal or higher.
// ============================================================

// A stat that we track
public enum AchievementStat
{
    MobsKilled,         // mobs the player killed
    ItemsPickedUp,      // loot picked up from the ground
    ItemsConsumed,      // anything eaten or drunk
    DrinksConsumed,     // only drinks consumed
    FoodEaten,          // only food consumed
    DropsMissed         // tries to pick up with a full inventory
}

// An achievement entry
public class AchievementData
{
    public string id;                    // unique name
    public string title;                 // user facing, e.g. "First Blood"
    public string description;           // e.g. "Kill 1 mob"
    public AchievementStat stat;         // which stat this watches
    public int goal;                     // count needed to unlock

    public AchievementData(string id, string title, string description, AchievementStat stat, int goal)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.stat = stat;
        this.goal = goal;
    }
}
