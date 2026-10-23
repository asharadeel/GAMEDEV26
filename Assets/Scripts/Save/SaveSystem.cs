// ============================================================
// Title:       SaveSystem
// Version:     1.1
// Written by:  Ashar
// Description: Saves/loads any serializable class as JSON.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//files live in Application.persistentDataPath (Windows: C:\Users\<you>\AppData\LocalLow\DefaultCompany\GAMEDEV26)
public static class SaveSystem
{
    //FILE PATHS
    static string GetPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName + ".json");
    }

    //SAVE / LOAD
    public static void Save<T>(string fileName, T data)
    {
        File.WriteAllText(GetPath(fileName), JsonUtility.ToJson(data, true));
    }

    //fallback if there's no save yet
    public static T Load<T>(string fileName, T fallback)
    {
        string path = GetPath(fileName);
        if (!File.Exists(path))
            return fallback;

        return JsonUtility.FromJson<T>(File.ReadAllText(path));
    }

    //CHECK / DELETE
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
