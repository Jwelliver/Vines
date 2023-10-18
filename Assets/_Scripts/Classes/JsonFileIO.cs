using System;
using System.IO;
using UnityEngine;

public static class JsonFileIO
{
    public static bool Save<T>(T objectToSave, string fileName)
    {
        try
        {
            string json = JsonUtility.ToJson(objectToSave);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }

            File.WriteAllText(fileName, json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("JsonFileIO.Save() Error: " + e);
            return false;
        }

    }

    public static T Load<T>(string fileName) where T : new()
    {
        if (File.Exists(fileName))
        {
            string json = File.ReadAllText(fileName);
            T objectToLoad = JsonUtility.FromJson<T>(json);
            return objectToLoad;
        }
        else
        {
            Debug.Log($"Cannot load file at {fileName}. File does not exist");
            return new T();
        }
    }
}