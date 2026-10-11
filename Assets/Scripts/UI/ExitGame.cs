using UnityEngine;
using UnityEngine.UI;

// Put on a Button. Closes the game when clicked.
[RequireComponent(typeof(Button))]
public class ExitGame : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Exit);
    }

    void Exit()
    {
#if UNITY_EDITOR
        // Application.Quit does nothing in the editor, so stop Play mode instead
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
