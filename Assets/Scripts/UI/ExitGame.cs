// ============================================================
// Title:       ExitGame
// Version:     1.0
// Written by:  Ashar
// Description: Button: closes the game.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ExitGame : MonoBehaviour
{
    //LISTENERS
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Exit);
    }

    //QUIT
    void Exit()
    {
        Debug.Log("Quitting game.");
#if UNITY_EDITOR
        // Application.Quit does nothing in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
