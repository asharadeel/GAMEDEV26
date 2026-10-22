// ============================================================
// Title:       ShooterUpgradeUI
// Version:     1.1
// Written by:  Ali
// Description: The shop's shooter upgrade window. One bar per
//              ShooterStat (Lazer, Damage, Fire Rate, Bullet Speed,
//              Reload Time), filling left to right, with a Buy
//              button that opens the shop's Buy pop-up for the next
//              block. Opened by the Upgrade button on a shooter's
//              ShopItemCard. The shop window hides while it's open
//              and comes back as it was (same tab) when it closes.
//              Goes on the window's container (always active).
//              Built by Tools > Build Shooter Upgrades.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

// One bar's parts
[System.Serializable]
public class UpgradeBar
{
    public ShooterStat stat;
    public RawImage[] blocks;   // left to right, extra blocks hide (the lazer bar only has 2)
    public Text value;          // "10 → 13"
    public Button buy;
    public Text buyLabel;       // "Buy" / "MAX"
}

public class ShooterUpgradeUI : MonoBehaviour
{
    // Cards are prefabs and can't point at scene objects, so they use this
    public static ShooterUpgradeUI Instance { get; private set; }

    // Bar names, in ShooterStat order
    public static readonly string[] BarNames = { "Lazer", "Damage", "Fire Rate", "Bullet Speed", "Reload Time" };

    public GameObject window;   // the window's Frame
    public GameObject shop;     // the shop window, hidden while this is open
    public Text title;
    public Text message;        // "You have 120 coins"
    public UpgradeBar[] bars;

    [Header("Block colours")]
    public Color filled = new Color(0.35f, 0.85f, 1f);
    public Color empty = new Color(1f, 1f, 1f, 0.15f);

    private ShooterData shooter;
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

    public void Open(ShooterData gun)
    {
        shooter = gun;
        title.text = "Upgrade " + gun.displayName;
        Refresh();
        window.SetActive(true);
        SetShopVisible(false);
    }

    // After buying, the Buy pop-up closes and this shows the new block
    public void Refresh()
    {
        if (shooter == null)
            return;

        message.text = "You have " + ProfileManager.Profile.Coins + " coins";

        foreach (UpgradeBar bar in bars)
        {
            int level = ProfileManager.GetLevel(shooter, bar.stat);
            int max = shooter.MaxLevel(bar.stat);
            bool maxed = level >= max;

            for (int i = 0; i < bar.blocks.Length; i++)
            {
                bar.blocks[i].gameObject.SetActive(i < max);
                bar.blocks[i].color = i < level ? filled : empty;
            }

            bar.value.text = maxed ? Describe(bar.stat, level) : Describe(bar.stat, level) + "  →  " + Describe(bar.stat, level + 1);
            bar.buyLabel.text = maxed ? "MAX" : "Buy";
            bar.buy.interactable = !maxed;
        }
    }

    // e.g. "Buy UV Lazer" or "Buy Damage (10 → 13)", button "Buy: 40"
    void OpenBuy(UpgradeBar bar)
    {
        int next = ProfileManager.GetLevel(shooter, bar.stat) + 1;
        string name = bar.stat == ShooterStat.Lazer
            ? shooter.LazerAt(next).displayName
            : BarNames[(int)bar.stat] + " (" + Describe(bar.stat, next - 1) + " → " + Describe(bar.stat, next) + ")";
        ShopPopups.Instance.OpenBuy(shooter, bar.stat, name);
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

    // A bar's value at a level, e.g. "4.8/s", "1.7s", "UV Lazer (Bigger shots)"
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
}
