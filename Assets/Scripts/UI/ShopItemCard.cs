// ============================================================
// Title:       ShopItemCard
// Version:     1.1
// Written by:  Ashar
// Updated by:  Ali (v1.1: shooters open ShooterUpgradeUI)
// Description: One item in the shop (the ShopItemCard prefab).
//              Drag the item asset into Item. Shows its name and
//              level, and the buttons that fit:
//              not owned = Buy, owned = Equip + Refund + Upgrade.
//              Refund is hidden for starting items and greyed out
//              while equipped. Upgrade is hidden at max level.
//              Buy/Upgrade/Refund open the pop-ups in ShopPopups.
//              Shooters have no level: their Upgrade button opens
//              the upgrade bars window (ShooterUpgradeUI).
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
        upgradeButton.gameObject.SetActive(shooter ? owned : ProfileManager.CanUpgrade(item));
        refundButton.gameObject.SetActive(owned && !item.unlocked);   // starting items can't be refunded

        equipButton.interactable = !equipped;
        equipLabel.text = equipped ? "Equipped" : "Equip";
        refundButton.interactable = !equipped;                        // unequip (equip something else) first
    }

    void Upgrade()
    {
        if (!(item is ShooterData shooter))
            ShopPopups.Instance.OpenUpgrade(this);
        else if (ShooterUpgradeUI.Instance != null)
            ShooterUpgradeUI.Instance.Open(shooter);
        else
            Debug.LogWarning("No shooter upgrade window in this scene, run Tools > Build Shooter Upgrades");
    }

    void Equip()
    {
        ProfileManager.Equip(item);
        RefreshAll();   // the item this replaced is no longer equipped
    }
}
