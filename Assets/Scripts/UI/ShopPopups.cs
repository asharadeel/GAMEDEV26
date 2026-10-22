// ============================================================
// Title:       ShopPopups
// Version:     1.2
// Written by:  Ashar
// Description: The shop's Buy, Upgrade and Refund pop-ups, shared
//              by every ShopItemCard (weapons) and ConsumableCard
//              (food/drink: Buy and Refund). Each pop-up is a
//              GeneralPopUpWindow copy:
//              Buy:     "Buy <item>",               button "Buy: <cost>"
//              Upgrade: "Upgrade to level <n>",     button "Cost: <cost>"
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
    public Text message;        // "You have 120 tokens" / "Not enough tokens"
}

public class ShopPopups : MonoBehaviour
{
    // Cards are prefabs and can't point at scene objects, so they use this
    public static ShopPopups Instance { get; private set; }

    public ShopPopup buy;
    public ShopPopup upgrade;
    public ShopPopup refund;

    private ShopItemCard card;           // the weapon card that opened the pop-up
    private ConsumableCard consumable;   // or the food/drink card

    void Awake()
    {
        Instance = this;
        buy.action.onClick.AddListener(Buy);
        upgrade.action.onClick.AddListener(Upgrade);
        refund.action.onClick.AddListener(Refund);
    }

    // ---- Opening ----

    public void OpenBuy(ShopItemCard from)
    {
        card = from;
        consumable = null;
        Open(buy, "Buy " + card.item.displayName, "Buy: " + card.item.tokenCost);
    }

    public void OpenBuy(ConsumableCard from)
    {
        consumable = from;
        card = null;
        Open(buy, "Buy " + consumable.item.displayName, "Buy: " + consumable.item.tokenCost);
    }

    public void OpenUpgrade(ShopItemCard from)
    {
        card = from;
        int next = ProfileManager.GetLevel(card.item) + 1;
        Open(upgrade, "Upgrade to level " + next, "Cost: " + card.item.UpgradeCost(next));
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
        popup.message.text = "You have " + ProfileManager.Profile.Tokens + " tokens";
        popup.window.SetActive(true);
    }

    // ---- Action buttons ----

    void Buy()
    {
        ItemData item = consumable != null ? (ItemData)consumable.item : card.item;
        if (ProfileManager.Profile.Level < item.levelRequired)
        {
            buy.message.text = "Needs player level " + item.levelRequired;
            return;
        }
        if (ProfileManager.Profile.Tokens < item.tokenCost)
        {
            buy.message.text = "Not enough tokens";
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
            buy.message.text = "You have " + ProfileManager.Profile.Tokens + " tokens";
            ConsumableCard.RefreshAll();
            return;
        }
        Done(buy);
    }

    void Upgrade()
    {
        if (!ProfileManager.TryUpgrade(card.item))
        {
            upgrade.message.text = "Not enough tokens";
            return;
        }
        Done(upgrade);
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
                refund.message.text = "You have " + ProfileManager.Profile.Tokens + " tokens";
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
    }
}
