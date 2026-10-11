using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Put on a Button. Loads the chosen scene when clicked.
// The dropdown lists the scenes in File > Build Settings.
[RequireComponent(typeof(Button))]
public class OpenScene : MonoBehaviour
{
    public string sceneName = "Scene 1 Plains";

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Open);
    }

    void Open()
    {
        SceneManager.LoadScene(sceneName);
    }
}

#if UNITY_EDITOR
// Shows sceneName as a dropdown in the Inspector instead of a text box
[CustomEditor(typeof(OpenScene))]
public class OpenSceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty sceneName = serializedObject.FindProperty("sceneName");

        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        string[] names = new string[scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
            names[i] = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);

        if (names.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes in File > Build Settings. Add some to pick from.", MessageType.Warning);
        }
        else
        {
            int index = Mathf.Max(0, System.Array.IndexOf(names, sceneName.stringValue));
            index = EditorGUILayout.Popup("Scene", index, names);
            sceneName.stringValue = names[index];
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
