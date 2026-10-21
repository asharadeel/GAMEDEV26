// ============================================================
// Title:       AchievementPopup
// Version:     1.2
// Written by:  Davud Jabiyev
// Description: A small banner that shows "Achievement unlocked: X"
//              for a few seconds when an achievement is unlocked.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

public class AchievementPopup : MonoBehaviour
{
    public Text bannerText;              // the UI Text instance that shows the message
    public float showTime = 3f;          // seconds the banner stays visible

    private float hideAt = 0f;           // game time when the banner hides

    void Start()
    {
        // Hidden at start
        if (bannerText != null) { // skip if not loaded
            bannerText.gameObject.SetActive(false);
        }
    }

    // Start / Stop listening when this component turns on / off
    void OnEnable()
    {
        AchievementManager.OnUnlocked += ShowBanner;
    }

    void OnDisable()
    {
        AchievementManager.OnUnlocked -= ShowBanner;
    }

    void ShowBanner(AchievementData a)
    {
        if (bannerText == null) { // skip if not loaded
            return;
        }

        bannerText.text = "Achievement unlocked: " + a.title;
        bannerText.gameObject.SetActive(true);
        hideAt = Time.time + showTime;
    }

    void Update()
    {
        // Hide the banner when time is up
        if (bannerText != null && bannerText.gameObject.activeSelf && Time.time >= hideAt) {
            bannerText.gameObject.SetActive(false);
        }
    }
}
