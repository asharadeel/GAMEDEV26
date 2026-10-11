using UnityEngine;

// Put on any object you want turned off when the scene starts.
public class InactiveOnStart : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
