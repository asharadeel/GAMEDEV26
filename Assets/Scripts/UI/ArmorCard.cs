// ============================================================
// Title:       ArmorCard
// Version:     1.0
// Written by:  Ashar
// Description: One armour in the Hub (the ArmorCard prefab). Drag
//              the armour asset into Item and pick a Mode:
//              Craft = Blacksmith, shows the ore recipe, Craft button
//              Buy   = Store, Buy button (coins, Buy pop-up)
//              Equip = Inventory, wear this one (only one at a time)
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmorCard : MonoBehaviour
{
    public enum Mode { Craft, Buy, Equip }

    public ArmorData item;
    public Mode mode = Mode.Craft;

    [Header("Card parts")]
    public Text nameText;        //name, owned count and the recipe
    public Text infoText;        //one short line: shield and replace time
    public Button actionButton;
    public Text actionLabel;

    //every card that's showing, so crafting/buying updates them all
    private static readonly List<ArmorCard> cards = new List<ArmorCard>();

    //LISTENERS
    void Awake()
    {
        actionButton.onClick.AddListener(Act);
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
        foreach (ArmorCard card in cards)
            card.Refresh();
    }

    //REFRESH
    public void Refresh()
    {
        if (item == null)
        {
            nameText.text = "No armor";
            return;
        }

        PlayerProfile p = ProfileManager.Profile;
        int owned = p.GetArmorCount(item.id);
        infoText.text = "Shield " + item.healthBonus + "  |  next piece in " + item.replaceDelay + "s";

        string text = "<b>" + item.displayName + "</b>\nOwned: " + owned;
        if (mode == Mode.Craft)
        {
            foreach (OreCost cost in item.recipe)
            {
                if (cost.ore == null)
                    continue;
                int have = p.GetOreCount(cost.ore.id);
                string colour = have >= cost.amount ? "#7CFC7C" : "#FF6B6B";
                text += "\n<color=" + colour + ">" + cost.ore.displayName + " " + have + " / " + cost.amount + "</color>";
            }
            actionLabel.text = "Craft";
            actionButton.interactable = ProfileManager.CanCraftArmor(item);
        }
        else if (mode == Mode.Buy)
        {
            actionLabel.text = "Buy: " + item.coinCost;
            actionButton.interactable = true;
        }
        else
        {
            bool equipped = ProfileManager.IsArmorEquipped(item) && owned > 0;
            actionLabel.text = owned <= 0 ? "Not owned" : (equipped ? "Unequip" : "Equip");
            actionButton.interactable = owned > 0;
        }
        nameText.text = text;
    }

    //BUTTON
    void Act()
    {
        if (mode == Mode.Craft)
        {
            if (ProfileManager.TryCraftArmor(item))
                Debug.Log("Crafted " + item.displayName);
            RefreshAll();
        }
        else if (mode == Mode.Buy)
        {
            ShopPopups.Instance.OpenBuy(item.displayName, item.coinCost, () => ProfileManager.TryBuyArmor(item));
        }
        else
        {
            if (ProfileManager.IsArmorEquipped(item))
            {
                ProfileManager.UnequipArmor();
                Debug.Log("Took off " + item.displayName);
            }
            else if (ProfileManager.EquipArmor(item))
            {
                Debug.Log("Equipped " + item.displayName);
            }
            RefreshAll();
        }
    }
}
