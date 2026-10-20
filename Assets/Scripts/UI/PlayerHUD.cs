// ============================================================
// Title:       PlayerHUD
// Version:     1.1
// Written by:  Ashar
// Description: Keeps the health/stamina/hunger/thirst bars and objectives up to date.
//              H hides/shows the whole player UI.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

// Goes on PlayerUI. Keeps the bottom bars in sync with the player profile
// and fills in the objectives list.
public class PlayerHUD : MonoBehaviour
{
    [Header("Bars")]
    public Slider healthSlider;
    public Slider staminaSlider;
    public Slider hungerSlider;
    public Slider thirstSlider;

    [Header("Hide UI")]
    public KeyCode hideKey = KeyCode.H;

    [Header("Objectives")]
    public Text objectivesText;
    [TextArea]
    public string[] objectives = { "Explore the plains", "Find food and water", "Survive the night" };

    private CanvasGroup group;

    void Start()
    {
        // Fade the UI instead of turning it off, so this script keeps listening for H
        group = GetComponent<CanvasGroup>();
        if (group == null)
            group = gameObject.AddComponent<CanvasGroup>();

        RefreshObjectives();
    }

    void Update()
    {
        if (Input.GetKeyDown(hideKey))
            SetVisible(group.alpha < 1f);

        PlayerProfile p = ProfileManager.Profile;

        SetBar(healthSlider, p.Health, PlayerProfile.MaxStat);
        SetBar(staminaSlider, p.Stamina, PlayerProfile.MaxStat);
        SetBar(hungerSlider, p.Hunger, PlayerProfile.MaxStat);
        SetBar(thirstSlider, p.Thirst, PlayerProfile.MaxStat);
    }

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

    // Call this after changing the objectives array from code
    public void RefreshObjectives()
    {
        if (objectivesText == null)
            return;

        objectivesText.text = "";
        foreach (string objective in objectives)
            objectivesText.text += "- " + objective + "\n";
    }
}
