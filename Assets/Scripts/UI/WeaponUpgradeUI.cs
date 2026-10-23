// ============================================================
// Title:       WeaponUpgradeUI
// Version:     2.0
// Written by:  Ali
// Description: The shop's upgrade window, opened by the Upgrade
//              button on any weapon's ShopItemCard. Bars fill left
//              to right, each with a Buy button that opens the
//              shop's Buy pop-up for its next block.
//              Shooters: one bar per ShooterStat (Lazer, Damage,
//              Fire Rate, Bullet Speed, Reload Time).
//              Melee/specials: one Level bar (levels 2 and 3).
//              The shop window hides while it's open and comes back
//              as it was (same tab) when it closes.
//              Goes on the window's container (always active).
//              Built by Tools > Build Weapon Upgrades.
// ============================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// One bar's parts
[System.Serializable]
public class UpgradeBar
{
    public ShooterStat stat;    // which shooter bar this is (the 1st bar is the Level bar for melee/specials)
    public GameObject row;      // hidden when the weapon doesn't use this bar
    public Text label;          // "Damage" / "Level"
    public RawImage[] blocks;   // left to right, extra blocks hide (the lazer and level bars only have 2)
    public Text value;          // "10 → 13"
    public Button buy;
    public Text buyLabel;       // "Buy" / "MAX"
}

public class WeaponUpgradeUI : MonoBehaviour
{
    // Cards are prefabs and can't point at scene objects, so they use this
    public static WeaponUpgradeUI Instance { get; private set; }

    // Shooter bar names, in ShooterStat order
    public static readonly string[] BarNames = { "Lazer", "Damage", "Fire Rate", "Bullet Speed", "Reload Time" };

    public GameObject window;   // the window's Frame
    public GameObject shop;     // the shop window, hidden while this is open
    public Text title;
    public Text message;        // "You have 120 coins"
    public UpgradeBar[] bars;

    [Header("Block colours")]
    public Color filled = new Color(0.35f, 0.85f, 1f);
    public Color empty = new Color(1f, 1f, 1f, 0.15f);

    private WeaponData weapon;
    private ShooterData shooter;   // the weapon, if it's a shooter
    private CanvasGroup shopGroup;
    private bool shopHidden;

    void Awake()
    {
        Instance = this;
        foreach (UpgradeBar bar in bars)
        {
            UpgradeBar b = bar;
            b.buy.onClick.AddListener(() => OpenBuy(b));
        }
    }

    // However the window was closed, bring the shop back
    void Update()
    {
        if (shopHidden && !window.activeInHierarchy)
            SetShopVisible(true);
    }

    public void Open(WeaponData item)
    {
        weapon = item;
        shooter = item as ShooterData;
        title.text = "Upgrade " + item.displayName;
        Refresh();
        window.SetActive(true);
        SetShopVisible(false);
    }

    // After buying, the Buy pop-up closes and this shows the new block
    public void Refresh()
    {
        if (weapon == null)
            return;

        message.text = "You have " + ProfileManager.Profile.Coins + " coins";

        for (int i = 0; i < bars.Length; i++)
        {
            UpgradeBar bar = bars[i];
            bool used = shooter != null || i == 0;
            bar.row.SetActive(used);
            if (!used)
                continue;

            int level = Level(bar);
            int max = MaxLevel(bar);
            bool maxed = level >= max;

            bar.label.text = shooter != null ? BarNames[i] : "Level";
            for (int b = 0; b < bar.blocks.Length; b++)
            {
                bar.blocks[b].gameObject.SetActive(b < max);
                bar.blocks[b].color = b < level ? filled : empty;
            }

            bar.value.text = ValueText(bar, level, maxed);
            bar.buyLabel.text = maxed ? "MAX" : "Buy";
            bar.buy.interactable = !maxed;
        }
    }

    // Blocks filled: shooter upgrades bought, or melee/special level - 1
    int Level(UpgradeBar bar)
    {
        return shooter != null ? ProfileManager.GetLevel(shooter, bar.stat) : ProfileManager.GetLevel(weapon) - 1;
    }

    int MaxLevel(UpgradeBar bar)
    {
        return shooter != null ? shooter.MaxLevel(bar.stat) : WeaponData.MaxLevel - 1;
    }

    string ValueText(UpgradeBar bar, int level, bool maxed)
    {
        if (shooter == null)
        {
            int current = level + 1;
            return maxed ? "Level " + current + " (max)" : "Level " + current + " → " + (current + 1) + ": " + LevelChange(current, current + 1);
        }
        return maxed ? Describe(bar.stat, level) : Describe(bar.stat, level) + "  →  " + Describe(bar.stat, level + 1);
    }

    // e.g. "Buy UV Lazer", "Buy Damage (10 → 13)", "Buy Punch Level 2"
    void OpenBuy(UpgradeBar bar)
    {
        int next = Level(bar) + 1;
        string name;
        int cost;
        if (shooter == null)
        {
            name = weapon.displayName + " Level " + (next + 1);
            cost = weapon.UpgradeCost(next + 1);
        }
        else
        {
            name = bar.stat == ShooterStat.Lazer
                ? shooter.LazerAt(next).displayName
                : BarNames[(int)bar.stat] + " (" + Describe(bar.stat, next - 1) + " → " + Describe(bar.stat, next) + ")";
            cost = shooter.UpgradeCost(bar.stat, next);
        }
        WeaponData item = weapon;
        ShooterStat stat = bar.stat;
        ShopPopups.Instance.OpenBuy(name, cost, () => item is ShooterData gun
            ? ProfileManager.TryUpgrade(gun, stat)
            : ProfileManager.TryUpgrade(item));
    }

    // Hidden with a CanvasGroup instead of switched off, so it keeps its tab when it comes back
    void SetShopVisible(bool visible)
    {
        if (shop == null)
            return;
        if (shopGroup == null)
        {
            shopGroup = shop.GetComponent<CanvasGroup>();
            if (shopGroup == null)
                shopGroup = shop.AddComponent<CanvasGroup>();
        }

        shopGroup.alpha = visible ? 1f : 0f;
        shopGroup.interactable = visible;
        shopGroup.blocksRaycasts = visible;
        shopHidden = !visible;
    }

    // A shooter bar's value at a level, e.g. "4.8/s", "1.7s", "UV Lazer (Bigger shots)"
    string Describe(ShooterStat stat, int level)
    {
        if (stat == ShooterStat.Lazer)
        {
            LazerData lazer = shooter.LazerAt(level);
            if (lazer == null)
                return "-";
            string perk = lazer.PerkText;
            return perk == "" ? lazer.displayName : lazer.displayName + " (" + perk + ")";
        }

        float v = shooter.Value(stat, level);
        if (stat == ShooterStat.FireRate)
            return v.ToString("0.#") + "/s";
        if (stat == ShooterStat.ReloadTime)
            return v.ToString("0.0#") + "s";
        return v.ToString("0.#");
    }

    // What changes between two melee/special levels, e.g. "Damage 5 → 7.5"
    string LevelChange(int from, int to)
    {
        List<string> changes = new List<string>();
        if (weapon is MeleeData melee)
        {
            MeleeLevel a = melee.AtLevel(from), b = melee.AtLevel(to);
            AddChange(changes, "Damage", a.damage, b.damage, "");
            AddChange(changes, "Attacks", a.fireRate, b.fireRate, "/s");
            AddChange(changes, "Radius", a.radius, b.radius, "");
        }
        return changes.Count > 0 ? string.Join(", ", changes) : "Better stats";
    }

    void AddChange(List<string> changes, string name, float from, float to, string unit)
    {
        if (!Mathf.Approximately(from, to))
            changes.Add(name + " " + from.ToString("0.#") + unit + " → " + to.ToString("0.#") + unit);
    }
}
