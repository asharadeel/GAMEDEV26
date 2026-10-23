// ============================================================
// Title:       BackpackBox
// Version:     1.0
// Written by:  Ashar
// Description: The backpack in the Hub inventory: the 6 food/drink
//              slots (keys 4-9) you take into a level. Each slot
//              shows what's in it, pressing it puts that item back
//              into stored food.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackBox : MonoBehaviour
{
    public Button[] slots = new Button[6];    //keys 4-9
    public Text[] labels = new Text[6];

    private static readonly List<BackpackBox> boxes = new List<BackpackBox>();

    //LISTENERS
    void Awake()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int index = i;   //copy, so each button remembers its own slot
            slots[i].onClick.AddListener(() => TakeOut(index));
        }
    }

    void OnEnable()
    {
        boxes.Add(this);
        Refresh();
    }

    void OnDisable()
    {
        boxes.Remove(this);
    }

    public static void RefreshAll()
    {
        foreach (BackpackBox box in boxes)
            box.Refresh();
    }

    //REFRESH
    public void Refresh()
    {
        PlayerProfile p = ProfileManager.Profile;
        for (int i = 0; i < slots.Length; i++)
        {
            ConsumableData item = ItemDatabase.Instance.Get<ConsumableData>(p.GetSlot(i));
            labels[i].text = (i + 4) + "\n" + (item != null ? item.displayName : "Empty");
            slots[i].interactable = item != null;
        }
    }

    //SLOT
    void TakeOut(int index)
    {
        if (ProfileManager.MoveSlotToStorage(index))
            Debug.Log("Put slot " + (index + 4) + " back into stored food.");
        RefreshAll();
        FoodCard.RefreshAll();
    }
}
