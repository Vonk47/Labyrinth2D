using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    /// <summary>
    /// Save any serializable data type to disk as JSON.
    /// </summary>
    /// <typeparam name="T">The type of data you're saving.</typeparam>
    /// <param name="data">The object to save.</param>
    /// <param name="fileName">File name (e.g., "maze_settings.json").</param>
    public static void Save<T>(T data, string fileName)
    {
        string fullPath = GetFilePath(fileName);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(fullPath, json);
    }

    /// <summary>
    /// Load saved data from disk. Returns default if file doesn't exist.
    /// </summary>
    /// <typeparam name="T">The type to deserialize into.</typeparam>
    /// <param name="fileName">File name (e.g., "maze_settings.json").</param>
    /// <returns>Loaded object or new instance.</returns>
    public static T Load<T>(string fileName) where T : new()
    {
        string fullPath = GetFilePath(fileName);
        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            return JsonUtility.FromJson<T>(json);
        }
        else
        {
            Debug.LogWarning($"[SaveSystem] File not found: {fullPath}. Returning default.");
            return new T(); // fallback if file doesn't exist
        }
    }

    /// <summary>
    /// Delete a save file by name.
    /// </summary>
    public static void Delete(string fileName)
    {
        string fullPath = GetFilePath(fileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private static string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}
