// ============================================================
// Title:       TabSelector
// Version:     1.0
// Written by:  Ashar
// Description: A row of buttons, each with its own frame. Clicking
//              a button shows its frame and hides all the others.
//              The picked button is greyed out so you can see which
//              one is open. Opens on the first tab every time.
//              Goes on the window that holds the buttons and frames.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

// One button and the frame it opens
[System.Serializable]
public class SelectorTab
{
    public Button button;
    public GameObject frame;
}

public class TabSelector : MonoBehaviour
{
    public SelectorTab[] tabs;

    void Awake()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;   // copy, so each button remembers its own tab
            if (tabs[i].button != null)
                tabs[i].button.onClick.AddListener(() => Select(index));
        }
    }

    // Back to the first tab every time the window opens
    void OnEnable()
    {
        Select(0);
    }

    public void Select(int index)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            bool on = i == index;
            if (tabs[i].frame != null)
                tabs[i].frame.SetActive(on);
            if (tabs[i].button != null)
                tabs[i].button.interactable = !on;
        }
    }
}
