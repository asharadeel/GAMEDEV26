using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

// reloads the open scene when its file changes on disk (git pull, or edited outside unity)
// unity normally keeps showing its own copy (it even restores it from a backup after
// recompiling or leaving play mode), so outside changes don't show until you reopen the scene
// if the scene has unsaved changes it asks first. editor only, nothing to set up
[InitializeOnLoad]
public class ReloadChangedScenes : AssetPostprocessor
{
    const string Key = "ReloadChangedScenes:";   // + scene path, the file's write time when unity last opened/saved it

    static ReloadChangedScenes()
    {
        EditorSceneManager.sceneOpened += (scene, mode) => Remember(scene.path);
        EditorSceneManager.sceneSaved += scene => Remember(scene.path);
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.EnteredEditMode)
                Check();
        };
        EditorApplication.delayCall += Check;   // after a recompile
    }

    static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
    {
        foreach (string path in imported)
        {
            if (path.EndsWith(".unity"))
            {
                EditorApplication.delayCall += Check;
                return;
            }
        }
    }

    static void Check()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || SceneManager.sceneCount != 1)
            return;

        Scene scene = SceneManager.GetActiveScene();
        if (string.IsNullOrEmpty(scene.path) || !File.Exists(scene.path))
            return;

        string known = SessionState.GetString(Key + scene.path, "");
        if (known == "")
        {
            Remember(scene.path);   // first time we've seen it, nothing to compare yet
            return;
        }
        if (known == WriteTime(scene.path))
            return;

        if (scene.isDirty && !EditorUtility.DisplayDialog("Scene changed on disk",
                scene.name + " was changed outside Unity. Reload it? Your unsaved changes in it will be lost.", "Reload", "Keep mine"))
        {
            Remember(scene.path);   // don't ask again for this change
            return;
        }

        EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
    }

    static void Remember(string path)
    {
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
            SessionState.SetString(Key + path, WriteTime(path));
    }

    static string WriteTime(string path)
    {
        return File.GetLastWriteTimeUtc(path).Ticks.ToString();
    }
}
