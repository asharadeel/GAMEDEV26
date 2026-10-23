// ============================================================
// Title:       DeathScreen
// Version:     1.0
// Written by:  Ashar
// Description: Full screen "YOU DIED!" window with the level's
//              SessionTracker log (time survived, kills, pickups).
//              Return to Hub or Restart Level (reloads the scene).
//              The game is paused while it's open. Everything
//              picked up stays in the profile.
//              Goes on the DeathScreen container (always active).
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    public static DeathScreen Instance { get; private set; }

    public string hubScene = "Hub";

    //interface
    public GameObject window;      //the Frame
    public Text titleText;
    public Text logText;
    public Button hubButton;
    public Button restartButton;

    //LISTENERS
    void Awake()
    {
        Instance = this;
        hubButton.onClick.AddListener(() => Leave(hubScene));
        restartButton.onClick.AddListener(() => Leave(SceneManager.GetActiveScene().name));
    }

    //SHOW
    public void Show()
    {
        titleText.text = "YOU DIED!";
        logText.text = SessionTracker.Instance != null ? SessionTracker.Instance.Report() : "";
        window.SetActive(true);
        Time.timeScale = 0f;   //everything stops behind the window
        Debug.Log("Player died.");
    }

    //BUTTONS
    void Leave(string scene)
    {
        Time.timeScale = 1f;
        Debug.Log("Loading scene: " + scene);
        SceneManager.LoadScene(scene);
    }
}
