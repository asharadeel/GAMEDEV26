// ============================================================
// Title:       FoodCard
// Version:     1.1
// Written by:  Ashar
// Description: One food/drink in the Hub inventory's Backpack tab
//              (the FoodCard prefab). Shows how many are stored and
//              what it restores. Pressing it moves one into the
//              backpack (slots 4-9), if there's room.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodCard : MonoBehaviour
{
    public ConsumableData item;

    [Header("Card parts")]
    public Text nameText;        //name, stored count and what it restores
    public Text infoText;        //one short line
    public Button actionButton;
    public Text actionLabel;

    //every card that's showing, so the backpack and cards stay in sync
    private static readonly List<FoodCard> cards = new List<FoodCard>();

    //LISTENERS
    void Awake()
    {
        actionButton.onClick.AddListener(AddToBackpack);
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
        foreach (FoodCard card in cards)
            card.Refresh();
    }

    //REFRESH
    public void Refresh()
    {
        if (item == null)
        {
            nameText.text = "No item";
            return;
        }

        int stored = ProfileManager.Profile.GetStoredFood(item.id);
        string text = "<b>" + item.displayName + "</b>\nStored: " + stored + "\nIn backpack: " + InBackpack();
        if (item.hunger > 0) text += "\nHunger +" + item.hunger;
        if (item.thirst > 0) text += "\nThirst +" + item.thirst;
        if (item.stamina > 0) text += "\nStamina +" + item.stamina;
        if (item.health > 0) text += "\nHealth +" + item.health;
        nameText.text = text;

        infoText.text = "Press to put one in your backpack";
        actionLabel.text = stored > 0 ? "Add 1 (" + stored + " to add)" : "0 to add";
        actionButton.interactable = stored > 0;
    }

    //how many of this item are in slots 4-9
    int InBackpack()
    {
        PlayerProfile p = ProfileManager.Profile;
        int count = 0;
        for (int i = 0; i < p.InventorySlots; i++)
        {
            if (p.GetSlot(i) == item.id)
                count++;
        }
        return count;
    }

    //BUTTON
    void AddToBackpack()
    {
        if (ProfileManager.MoveFoodToBackpack(item))
            Debug.Log("Put " + item.displayName + " in the backpack.");
        else
            Debug.Log("Backpack full or none stored.");
        RefreshAll();
        BackpackBox.RefreshAll();
    }
}
