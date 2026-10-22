// ============================================================
// Title:       TilePager
// Version:     1.0
// Written by:  Ashar
// Description: Shows one box (page) at a time instead of a scroll
//              view. Each page has an order and a UI object (a box
//              or a Frame with content). Left/Right buttons flip
//              between them, and each is only pressable if there's
//              a page that way.
//              Goes on the window that holds the pages.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

// One box in the pager. Pages are shown lowest order first.
[System.Serializable]
public class TilePage
{
    public int order;
    public GameObject content;
}

public class TilePager : MonoBehaviour
{
    public TilePage[] pages;
    public Button leftButton;
    public Button rightButton;
    public Text pageLabel;      // optional, e.g. "2 / 5"

    private int current;        // index into pages (after sorting)

    void Awake()
    {
        // Put the pages in order (lowest first)
        System.Array.Sort(pages, (a, b) => a.order.CompareTo(b.order));

        leftButton.onClick.AddListener(Previous);
        rightButton.onClick.AddListener(Next);
    }

    // Back to the first page every time the window opens
    void OnEnable()
    {
        ShowPage(0);
    }

    public void Next()
    {
        ShowPage(current + 1);
    }

    public void Previous()
    {
        ShowPage(current - 1);
    }

    public void ShowPage(int index)
    {
        if (pages.Length == 0)
            return;

        current = Mathf.Clamp(index, 0, pages.Length - 1);

        // Only the current page is switched on
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i].content != null)
                pages[i].content.SetActive(i == current);
        }

        leftButton.interactable = current > 0;
        rightButton.interactable = current < pages.Length - 1;

        if (pageLabel != null)
            pageLabel.text = (current + 1) + " / " + pages.Length;
    }
}
