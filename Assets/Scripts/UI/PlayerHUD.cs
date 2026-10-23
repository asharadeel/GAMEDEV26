// ============================================================
// Title:       PlayerHUD
// Version:     1.4
// Written by:  Ashar
// Updated by:  Ali (v1.2: ammo counter, v1.3: Q/E ability cooldowns)
//              cthullu96 (v1.4: input system package)
//              ai use: input system change written with claude (anthropic), owner cthullu96
// Description: Keeps the health/stamina/hunger/thirst bars, ammo, abilities and objectives up to date.
//              H hides/shows the whole player UI.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Bars")]
    public Slider healthSlider;
    public Slider staminaSlider;
    public Slider hungerSlider;
    public Slider thirstSlider;

    [Header("Ammo")]
    public PlayerCombat combat;
    public Text ammoText;        // "12 / 20" or "Reloading..." while the shooter is selected

    [Header("Abilities")]
    public PlayerAbilities abilities;
    public Text abilityText;     // "Q  Fire  Ready" / "E  Ice  3.2s"

    [Header("Objectives")]
    public Text objectivesText;
    [TextArea]
    public string[] objectives = { "Explore the plains", "Find food and water", "Survive the night" };

    private CanvasGroup group;

    //STARTING PROCEDURES
    void Start()
    {
        //fade instead of disabling, so H still works
        group = GetComponent<CanvasGroup>();
        if (group == null)
            group = gameObject.AddComponent<CanvasGroup>();

        RefreshObjectives();
    }

    void Update()
    {
        if (GameInput.HideUI.WasPressedThisFrame())
            SetVisible(group.alpha < 1f);

        PlayerProfile p = ProfileManager.Profile;

        SetBar(healthSlider, p.Health, PlayerProfile.MaxStat);
        SetBar(staminaSlider, p.Stamina, PlayerProfile.MaxStat);
        SetBar(hungerSlider, p.Hunger, PlayerProfile.MaxStat);
        SetBar(thirstSlider, p.Thirst, PlayerProfile.MaxStat);

        if (ammoText != null && combat != null)
            ammoText.text = combat.AmmoLabel();
        if (abilityText != null && abilities != null)
            abilityText.text = abilities.SlotLabel(0) + "\n" + abilities.SlotLabel(1);
    }

    //INTERFACE
    public void SetVisible(bool visible)
    {
        group.alpha = visible ? 1f : 0f;
        group.interactable = visible;
        group.blocksRaycasts = visible;
    }

    void SetBar(Slider slider, float value, float max)
    {
        if (slider != null)
            slider.value = value / max;
    }

    //OBJECTIVES
    public void RefreshObjectives()
    {
        if (objectivesText == null)
            return;

        objectivesText.text = "";
        foreach (string objective in objectives)
            objectivesText.text += "- " + objective + "\n";
    }
}
