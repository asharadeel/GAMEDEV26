using UnityEngine;
using UnityEngine.UI;

// Put on a Button. Turns OFF every object in the list when clicked.
[RequireComponent(typeof(Button))]
public class CloseElemOnClick : MonoBehaviour
{
    public GameObject[] elements;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Close);
    }

    void Close()
    {
        foreach (GameObject element in elements)
        {
            if (element != null)
                element.SetActive(false);
        }
    }
}
