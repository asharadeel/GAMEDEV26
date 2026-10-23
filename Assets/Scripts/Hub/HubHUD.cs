// ============================================================
// Title:       HubHUD
// Version:     1.0
// Written by:  Ashar
// Description: The Hub's always-on player info. Top left: the
//              player's name, their level and an xp bar towards
//              the next level. Top right: their coins. Updates by
//              itself when anything changes (buying, upgrading...).
//              Goes on the PlayerInfo container under the Canvas.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HubHUD : MonoBehaviour
{
    //top left
    public Text nameText;
    public Text levelText;
    public Text xpText;
    public RectTransform xpFill;    //anchored left, its width follows the xp

    //top right
    public Text coinsText;

    //what's showing now, so the text only changes when the profile does
    private string shownName;
    private int shownLevel = -1;
    private int shownXp = -1;
    private int shownCoins = -1;

    //LISTENERS
    void Update()
    {
        PlayerProfile p = ProfileManager.Profile;

        if (p.PlayerName != shownName)
        {
            shownName = p.PlayerName;
            nameText.text = string.IsNullOrEmpty(shownName) ? "Player" : shownName;
        }

        if (p.Level != shownLevel || p.Xp != shownXp)
        {
            shownLevel = p.Level;
            shownXp = p.Xp;
            levelText.text = "LEVEL " + shownLevel;
            xpText.text = shownXp + " / " + p.XpToNextLevel + " XP";
            xpFill.anchorMax = new Vector2(Mathf.Clamp01((float)shownXp / p.XpToNextLevel), 1f);
        }

        if (p.Coins != shownCoins)
        {
            shownCoins = p.Coins;
            coinsText.text = shownCoins.ToString("N0");   //1,250
        }
    }
}
