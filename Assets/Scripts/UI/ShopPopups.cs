// ============================================================
// Title:       ShopPopups
// Version:     1.5
// Written by:  Ashar
// Updated by:  Ali (v1.3: Buy pop-up also buys shooter upgrades,
//              v1.4: and every other weapon upgrade, Upgrade pop-up removed,
//              v1.5: OpenBuy(name, cost, buy) for anything, e.g. skill tree nodes)
// Description: The shop's Buy and Refund pop-ups, shared by every
//              ShopItemCard (weapons), ConsumableCard (food/drink),
//              the bars in WeaponUpgradeUI and the SkillTreeUI nodes
//              (Buy). Each pop-up
//              is a GeneralPopUpWindow copy:
//              Buy:     "Buy <item>",               button "Buy: <cost>"
//              Refund:  "Refund <item>: <refund>",  button "Refund"
//              Goes on the shop's container (always active).
// ============================================================

using UnityEngine;
using UnityEngine.UI;

// One pop-up's parts
[System.Serializable]
public class ShopPopup
{
    public GameObject window;   // the pop-up's Frame
    public Text title;
    public Button action;
    public Text actionLabel;
    public Text message;        // "You have 120 coins" / "Not enough coins"
}

public class ShopPopups : MonoBehaviour
{
    // Cards are prefabs and can't point at scene objects, so they use this
    public static ShopPopups Instance { get; private set; }

    public ShopPopup buy;
    public ShopPopup refund;

    private ShopItemCard card;           // the weapon card that opened the pop-up
    private ConsumableCard consumable;   // or the food/drink card
    private System.Func<bool> buyAction; // or anything else (an upgrade, a skill node): true if it was bought

    void Awake()
    {
        Instance = this;
        buy.action.onClick.AddListener(Buy);
        refund.action.onClick.AddListener(Refund);
    }

    // ---- Opening ----

    public void OpenBuy(ShopItemCard from)
    {
        card = from;
        consumable = null;
        buyAction = null;
        Open(buy, "Buy " + card.item.displayName, "Buy: " + card.item.coinCost);
    }

    public void OpenBuy(ConsumableCard from)
    {
        consumable = from;
        card = null;
        buyAction = null;
        Open(buy, "Buy " + consumable.item.displayName, "Buy: " + consumable.item.coinCost);
    }

    // Anything else that costs coins, e.g. "Buy Damage (10 → 13)" or "Buy Searing".
    // tryBuy spends the coins and returns false if there weren't enough
    public void OpenBuy(string name, int cost, System.Func<bool> tryBuy)
    {
        buyAction = tryBuy;
        card = null;
        consumable = null;
        Open(buy, "Buy " + name, "Buy: " + cost);
    }

    public void OpenRefund(ConsumableCard from)
    {
        consumable = from;
        card = null;
        Open(refund, "Refund " + consumable.item.displayName + ": " + consumable.item.RefundValue, "Refund");
    }

    public void OpenRefund(ShopItemCard from)
    {
        card = from;
        consumable = null;
        Open(refund, "Refund " + card.item.displayName + ": " + card.item.RefundValue, "Refund");
    }

    void Open(ShopPopup popup, string title, string action)
    {
        popup.title.text = title;
        popup.actionLabel.text = action;
        popup.message.text = "You have " + ProfileManager.Profile.Coins + " coins";
        popup.window.SetActive(true);
    }

    // ---- Action buttons ----

    void Buy()
    {
        if (buyAction != null)
        {
            if (!buyAction())
            {
                buy.message.text = "Not enough coins";
                return;
            }
            Done(buy);
            return;
        }

        ItemData item = consumable != null ? (ItemData)consumable.item : card.item;
        if (ProfileManager.Profile.Level < item.levelRequired)
        {
            buy.message.text = "Needs player level " + item.levelRequired;
            return;
        }
        if (ProfileManager.Profile.Coins < item.coinCost)
        {
            buy.message.text = "Not enough coins";
            return;
        }

        bool bought = consumable != null
            ? ProfileManager.TryBuyConsumable(consumable.item)
            : ProfileManager.TryBuyWeapon(card.item);
        if (!bought)
        {
            buy.message.text = consumable != null ? "Inventory full" : "Already owned";
            return;
        }

        // Food/drink: stay open to buy more, just update the count
        if (consumable != null)
        {
            buy.message.text = "You have " + ProfileManager.Profile.Coins + " coins";
            ConsumableCard.RefreshAll();
            return;
        }
        Done(buy);
    }

    void Refund()
    {
        // Food/drink: sell one back, stay open until there's none left
        if (consumable != null)
        {
            if (!ProfileManager.TryRefundConsumable(consumable.item))
            {
                refund.message.text = "You don't have any";
                return;
            }
            ConsumableCard.RefreshAll();
            if (ProfileManager.Profile.CountInInventory(consumable.item.id) == 0)
                Done(refund);
            else
                refund.message.text = "You have " + ProfileManager.Profile.Coins + " coins";
            return;
        }

        if (!ProfileManager.TryRefund(card.item))
        {
            refund.message.text = "Can't refund an equipped item";
            return;
        }
        Done(refund);
    }

    void Done(ShopPopup popup)
    {
        popup.window.SetActive(false);
        ShopItemCard.RefreshAll();
        ConsumableCard.RefreshAll();
        if (WeaponUpgradeUI.Instance != null)
            WeaponUpgradeUI.Instance.Refresh();
        if (SkillTreeUI.Instance != null)
            SkillTreeUI.Instance.Refresh();
    }
}
