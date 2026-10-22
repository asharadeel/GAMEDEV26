// ============================================================
// Title:       SkillTreeUI
// Version:     1.0
// Written by:  Ali
// Description: The Hub's Skill Tree window. One branch per ability
//              (Fire, Ice, Lightning): the ability at the bottom,
//              its nodes above, joined by lines. A node can only be
//              bought once the node below it is unlocked, so each
//              branch is climbed path by path. Buying goes through
//              the shop's Buy pop-up (ShopPopups).
//              The circles and lines are made from each ability's
//              Nodes list when the window first opens, so a new
//              node only needs adding to the ability asset.
//              Goes on the window's container (always active).
// ============================================================

using UnityEngine;
using UnityEngine.UI;

// One ability's column in the window
[System.Serializable]
public class SkillBranch
{
    public AbilityData ability;
    public Text header;          // ability name and its current stats
    public RectTransform area;   // the nodes and lines go in here
}

public class SkillTreeUI : MonoBehaviour
{
    public static SkillTreeUI Instance { get; private set; }

    public GameObject window;    // the window's Frame
    public Text message;         // "You have 120 coins" / "Unlock Searing first"
    public SkillBranch[] branches;
    public SkillNodeUI nodePrefab;

    [Header("Layout")]
    public float columnSpacing = 190f;
    public float bottomPadding = 70f;   // room for the ability's own title under it
    public float topPadding = 150f;

    [Header("Colours")]
    public Color available = Color.white;
    public Color locked = new Color(0.25f, 0.25f, 0.25f);
    public Color lineOff = new Color(0.35f, 0.35f, 0.35f);

    private SkillNodeUI[][] nodes;    // [branch][node], node 0 = the ability itself
    private Image[][] lines;          // [branch][node], the line from the node's parent to it
    private bool wasOpen;

    void Awake()
    {
        Instance = this;
    }

    // Refresh every time the window opens (RadiusUI opens the Frame directly)
    void Update()
    {
        bool open = window.activeInHierarchy;
        if (open && !wasOpen)
            Refresh();
        wasOpen = open;
    }

    public void Refresh()
    {
        if (nodes == null)
            Build();

        message.text = "You have " + ProfileManager.Profile.Coins + " coins";

        for (int b = 0; b < branches.Length; b++)
        {
            AbilityData ability = branches[b].ability;
            branches[b].header.text = "<b>" + ability.displayName + "</b>\n<size=22>" + Stats(ability) + "</size>";

            SkillNodeUI root = nodes[b][0];
            root.circle.color = ability.colour;
            root.icon.text = ability.displayName.Substring(0, 1);
            root.title.text = "<b>" + ability.displayName + "</b>\nFree";

            for (int i = 0; i < ability.nodes.Count; i++)
            {
                SkillNode node = ability.nodes[i];
                SkillNodeUI ui = nodes[b][i + 1];
                bool unlocked = ProfileManager.IsUnlocked(ability, i);
                bool canBuy = ProfileManager.CanUnlock(ability, i);

                ui.circle.color = unlocked ? ability.colour : canBuy ? available : locked;
                ui.icon.text = canBuy ? "+" : "";
                ui.title.text = "<b>" + node.title + "</b>\n" + ability.EffectText(node) + "\n" + (unlocked ? "Unlocked" : node.cost + " coins");
                lines[b][i + 1].color = unlocked ? ability.colour : lineOff;
            }
        }
    }

    void Clicked(int branch, int node)
    {
        AbilityData ability = branches[branch].ability;
        SkillNode data = ability.nodes[node];

        if (ProfileManager.IsUnlocked(ability, node))
        {
            message.text = data.title + " is already unlocked";
            return;
        }
        if (!ProfileManager.CanUnlock(ability, node))
        {
            message.text = "Unlock " + ability.nodes[data.parent].title + " first";
            return;
        }
        ShopPopups.Instance.OpenBuy(data.title + " (" + ability.EffectText(data) + ")", data.cost,
                                    () => ProfileManager.TryUnlock(ability, node));
    }

    // "Damage 25 · Blast 3 · Cooldown 6s"
    string Stats(AbilityData ability)
    {
        string text = "Damage " + ProfileManager.GetStat(ability, AbilityStat.Damage).ToString("0.#")
            + "  ·  " + ability.radiusName + " " + ProfileManager.GetStat(ability, AbilityStat.Radius).ToString("0.#")
            + "  ·  Cooldown " + ProfileManager.GetStat(ability, AbilityStat.Cooldown).ToString("0.#") + "s";
        if (ability.extraName != "")
            text += "  ·  " + ability.extraName + " " + ProfileManager.GetStat(ability, AbilityStat.Extra).ToString("0.#");
        return text;
    }

    // ---- Making the circles and lines ----

    void Build()
    {
        nodes = new SkillNodeUI[branches.Length][];
        lines = new Image[branches.Length][];

        for (int b = 0; b < branches.Length; b++)
        {
            AbilityData ability = branches[b].ability;
            RectTransform area = branches[b].area;
            int count = ability.nodes.Count + 1;
            nodes[b] = new SkillNodeUI[count];
            lines[b] = new Image[count];

            int rows = 1;
            foreach (SkillNode n in ability.nodes)
                rows = Mathf.Max(rows, n.row);
            float rowSpacing = (area.rect.height - bottomPadding - topPadding) / rows;

            // lines first so the circles sit on top of them
            RectTransform lineLayer = Layer("Lines", area);
            RectTransform nodeLayer = Layer("Nodes", area);

            nodes[b][0] = MakeNode(nodeLayer, Vector2.up * bottomPadding);
            for (int i = 0; i < ability.nodes.Count; i++)
            {
                SkillNode n = ability.nodes[i];
                nodes[b][i + 1] = MakeNode(nodeLayer, new Vector2(n.column * columnSpacing, bottomPadding + n.row * rowSpacing));

                int branch = b, node = i;
                nodes[b][i + 1].button.onClick.AddListener(() => Clicked(branch, node));
            }
            for (int i = 0; i < ability.nodes.Count; i++)
            {
                Vector2 from = nodes[b][ability.nodes[i].parent + 1].GetComponent<RectTransform>().anchoredPosition;
                Vector2 to = nodes[b][i + 1].GetComponent<RectTransform>().anchoredPosition;
                lines[b][i + 1] = MakeLine(lineLayer, from, to);
            }
        }
    }

    SkillNodeUI MakeNode(RectTransform parent, Vector2 position)
    {
        SkillNodeUI node = Instantiate(nodePrefab, parent);
        RectTransform rt = node.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0f);
        rt.anchoredPosition = position;
        return node;
    }

    Image MakeLine(RectTransform parent, Vector2 from, Vector2 to)
    {
        GameObject obj = new GameObject("Line", typeof(RectTransform), typeof(Image));
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0f);
        Vector2 dir = to - from;
        rt.sizeDelta = new Vector2(dir.magnitude, 8f);
        rt.anchoredPosition = (from + to) * 0.5f;
        rt.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        Image img = obj.GetComponent<Image>();
        img.raycastTarget = false;
        return img;
    }

    RectTransform Layer(string name, RectTransform parent)
    {
        RectTransform rt = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        return rt;
    }
}
