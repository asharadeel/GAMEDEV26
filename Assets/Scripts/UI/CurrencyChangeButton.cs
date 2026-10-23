// ============================================================
// Title:       CurrencyChangeButton
// Version:     1.0
// Written by:  Ashar
// Description: Button: adds or takes coins from the current
//              profile. Pick Add or Subtract and the amount.
//              Subtract doesn't go through if there aren't enough.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CurrencyChangeButton : MonoBehaviour
{
    public enum Mode { Add, Subtract }

    public Mode mode = Mode.Add;
    public int amount = 10;

    //LISTENERS
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Change);
    }

    //COINS
    void Change()
    {
        PlayerProfile p = ProfileManager.Profile;

        if (mode == Mode.Add)
        {
            p.Coins += amount;
            Debug.Log("Added " + amount + " coins.");
        }
        else if (p.Coins >= amount)
        {
            p.Coins -= amount;
            Debug.Log("Took " + amount + " coins.");
        }
        else
        {
            Debug.Log("Not enough coins.");
        }
    }
}
