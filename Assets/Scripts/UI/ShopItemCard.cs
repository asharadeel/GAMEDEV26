// ============================================================
// Title:       ShopItemCard
// Version:     1.2
// Written by:  Ashar
// Updated by:  Ali (v1.1: shooters open the upgrade bars window,
//              v1.2: every weapon does)
// Description: One item in the shop (the ShopItemCard prefab).
//              Drag the item asset into Item. Shows its name and
//              level, and the buttons that fit:
//              not owned = Buy, owned = Equip + Refund + Upgrade.
//              Refund is hidden for starting items and greyed out
//              while equipped. Buy/Refund open the pop-ups in
//              ShopPopups, Upgrade opens WeaponUpgradeUI.
//              Shooters have no level, just upgrade bars.
// ============================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemCard : MonoBehaviour
{
    public WeaponData item;

    [Header("Card parts")]
    public Text nameText;
    public Button buyButton;
    public Button upgradeButton;
    public Button refundButton;
    public Button equipButton;
    public Text equipLabel;

    // Every card that's showing, so one purchase can update them all
    private static readonly List<ShopItemCard> cards = new List<ShopItemCard>();

    void Awake()
    {
        buyButton.onClick.AddListener(() => ShopPopups.Instance.OpenBuy(this));
        upgradeButton.onClick.AddListener(Upgrade);
        refundButton.onClick.AddListener(() => ShopPopups.Instance.OpenRefund(this));
        equipButton.onClick.AddListener(Equip);
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
        foreach (ShopItemCard card in cards)
            card.Refresh();
    }

    public void Refresh()
    {
        if (item == null)
        {
            nameText.text = "No item";
            return;
        }

        bool owned = ProfileManager.Owns(item);
        bool equipped = ProfileManager.IsEquipped(item);

        bool shooter = item is ShooterData;

        nameText.text = shooter ? item.displayName : item.displayName + "\nLevel " + ProfileManager.GetLevel(item);

        buyButton.gameObject.SetActive(!owned);
        equipButton.gameObject.SetActive(owned);
        upgradeButton.gameObject.SetActive(owned);
        refundButton.gameObject.SetActive(owned && !item.unlocked);   // starting items can't be refunded

        equipButton.interactable = !equipped;
        equipLabel.text = equipped ? "Equipped" : "Equip";
        refundButton.interactable = !equipped;                        // unequip (equip something else) first
    }

    void Upgrade()
    {
        if (WeaponUpgradeUI.Instance != null)
            WeaponUpgradeUI.Instance.Open(item);
        else
            Debug.LogWarning("No upgrade window in this scene, run Tools > Build Weapon Upgrades");
    }

    void Equip()
    {
        ProfileManager.Equip(item);
        RefreshAll();   // the item this replaced is no longer equipped
    }
}
