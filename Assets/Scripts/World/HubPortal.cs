using UnityEngine;
using UnityEngine.SceneManagement;

// put this on a portal with a trigger collider
// walk into it and it loads the level
// if its locked it shows a message instead
public class HubPortal : MonoBehaviour
{
    public string sceneName = "Scene 1 Plains";  // scene to load, must be in build settings
    public bool locked = false;

    public GameObject lockedMessage;  // text that says the portal is locked
    public float showMessageFor = 2f; // seconds

    private float hideTime;

    void Update()
    {
        // hide the locked message after a bit
        if (lockedMessage != null && lockedMessage.activeSelf && Time.time > hideTime)
            lockedMessage.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (locked)
        {
            if (lockedMessage != null)
            {
                lockedMessage.SetActive(true);
                hideTime = Time.time + showMessageFor;
            }
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
