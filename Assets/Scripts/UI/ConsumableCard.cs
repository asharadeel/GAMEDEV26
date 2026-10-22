// ============================================================
// Title:       ConsumableCard
// Version:     1.1
// Written by:  Ashar
// Description: One food or drink in the shop (the ConsumableCard
//              prefab). Drag the item asset into Item. Shows its
//              name, a Buy button and how many the player has in
//              their inventory. Buy opens the Buy pop-up in
//              ShopPopups (one bought = one inventory slot).
//              Refund sells one back for half the price, greyed
//              out when the player has none.
// ============================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableCard : MonoBehaviour
{
    public ConsumableData item;

    [Header("Card parts")]
    public Text nameText;
    public Button buyButton;
    public Button refundButton;
    public Text quantityText;

    // Every card that's showing, so a purchase can update them all
    private static readonly List<ConsumableCard> cards = new List<ConsumableCard>();

    void Awake()
    {
        buyButton.onClick.AddListener(() => ShopPopups.Instance.OpenBuy(this));
        refundButton.onClick.AddListener(() => ShopPopups.Instance.OpenRefund(this));
    }

    void OnEnable()
    {
        cards.Add(this);
        Refresh();
    }

    void OnDisable()
    {
        cards.Remove(this);
    }

    public static void RefreshAll()
    {
        foreach (ConsumableCard card in cards)
            card.Refresh();
    }

    public void Refresh()
    {
        if (item == null)
        {
            nameText.text = "No item";
            return;
        }

        int owned = ProfileManager.Profile.CountInInventory(item.id);
        nameText.text = item.displayName;
        quantityText.text = "Amount owned: " + owned;
        refundButton.interactable = owned > 0;
    }
}
