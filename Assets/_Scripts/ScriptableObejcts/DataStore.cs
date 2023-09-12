
// using System.Collections.Generic;
// using UnityEngine;

// [CreateAssetMenu]
// public class DataStore : ScriptableObject
// {
//     [System.Serializable]
//     public struct Entry
//     {
//         public string key;
//         public string value;
//         public bool keepOnDisk;
//     }

//     public List<Entry> entries = new List<Entry>();

//     private Dictionary<string, string> data;

//     void OnEnable()
//     {
//         data = new Dictionary<string, string>();
//         LoadData();
//     }

//     void OnDisable()
//     {
//         SaveData();
//     }

//     private void SaveData()
//     {
//         foreach (var entry in entries)
//         {
//             if (entry.keepOnDisk)
//             {
//                 PlayerPrefs.SetString(entry.key, entry.value);
//             }
//         }
//         PlayerPrefs.Save();
//     }

//     private void LoadData()
//     {
//         foreach (var entry in entries)
//         {
//             if (entry.keepOnDisk)
//             {
//                 if (PlayerPrefs.HasKey(entry.key))
//                 {
//                     string savedValue = PlayerPrefs.GetString(entry.key);
//                     SetValue(entry.key, savedValue);
//                 }
//             }
//         }
//     }

//     public string GetValue(string key)
//     {
//         if (!data.ContainsKey(key)) return null;

//         return data[key];
//     }

//     public void SetValue(string key, string value, bool keepOnDisk = false)
//     {
//         // If key exist, update the data, else add a new entry
//         if (data.ContainsKey(key))
//         {
//             data[key] = value;
//             for (int i = 0; i < entries.Count; i++)
//             {
//                 if (entries[i].key == key)
//                 {
//                     entries[i].value = value;
//                     entries[i].keepOnDisk = keepOnDisk;
//                     break;
//                 }
//             }
//         }
//         else
//         {
//             data.Add(key, value);
//             entries.Add(new Entry() { key = key, value = value, keepOnDisk = keepOnDisk });
//         }
//     }

//     public bool ContainsKey(string key)
//     {
//         return data.ContainsKey(key);
//     }

//     public void RemoveKey(string key)
//     {
//         if (data.ContainsKey(key))
//         {
//             data.Remove(key);
//             // If the data is saved into PlayerPrefs, deleting it.
//             if (PlayerPrefs.HasKey(key))
//             {
//                 PlayerPrefs.DeleteKey(key);
//             }

//             // Also remove key from entries list
//             for (int i = 0; i < entries.Count; i++)
//             {
//                 if (entries[i].key == key)
//                 {
//                     entries.RemoveAt(i);
//                     break;
//                 }
//             }
//         }
//     }

//     public void SaveChanges()
//     {
//         SaveData();
//     }
// }


//=============================================== PRE SAVE/LOAD
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/DataStore")]
public class DataStore : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public string key;
        public string value;
    }

    public List<Entry> entries = new List<Entry>();

    private Dictionary<string, string> data;

    void OnEnable()
    {
        data = new Dictionary<string, string>();
        foreach (var entry in entries)
        {
            data[entry.key] = entry.value;
        }
    }

    public string GetValue(string key)
    {
        if (!data.ContainsKey(key)) return null;

        return data[key];
    }

    public void SetValue(string key, string value)
    {
        // If key exist, update the data, else add a new entry
        if (data.ContainsKey(key))
        {
            data[key] = value;
        }
        else
        {
            data.Add(key, value);
            entries.Add(new Entry() { key = key, value = value });
        }
    }

    public bool ContainsKey(string key)
    {
        return data.ContainsKey(key);
    }

    public void RemoveKey(string key)
    {
        if (data.ContainsKey(key))
        {
            data.Remove(key);

            // Also remove key from entries list
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].key == key)
                {
                    entries.RemoveAt(i);
                    break;
                }
            }
        }
    }

}