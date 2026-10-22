// ============================================================
// Title:       SaveSystem
// Version:     1.1
// Written by:  Ashar
// Description: Saves/loads any serializable class as JSON.
// ============================================================

using System.IO;
using UnityEngine;

// Saves any [System.Serializable] class as a JSON file.
// Files live in Application.persistentDataPath
// (Windows: C:\Users\<you>\AppData\LocalLow\DefaultCompany\GAMEDEV26)
public static class SaveSystem
{
    static string GetPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName + ".json");
    }

    public static void Save<T>(string fileName, T data)
    {
        File.WriteAllText(GetPath(fileName), JsonUtility.ToJson(data, true));
    }

    // Returns the saved data, or the fallback if there's no save yet
    public static T Load<T>(string fileName, T fallback)
    {
        string path = GetPath(fileName);
        if (!File.Exists(path))
            return fallback;

        return JsonUtility.FromJson<T>(File.ReadAllText(path));
    }

    public static bool Exists(string fileName)
    {
        return File.Exists(GetPath(fileName));
    }

    public static void Delete(string fileName)
    {
        string path = GetPath(fileName);
        if (File.Exists(path))
            File.Delete(path);
    }
}
