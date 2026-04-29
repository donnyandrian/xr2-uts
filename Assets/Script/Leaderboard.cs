using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Entry : IComparable<Entry>
{
    public float score;
    public DateTime saveTime;

    public Entry(float score, DateTime saveTime)
    {
        this.score = score;
        this.saveTime = saveTime;
    }

    public int CompareTo(Entry other)
    {
        if (other == null) return 1;

        return other.score.CompareTo(score);
    }

    public override string ToString()
    {
        return $"{score}";
    }
}

[Serializable]
public class SerializableList<T>
{
    public List<T> list;
}

public class Leaderboard : MonoBehaviour
{
    public UnityEvent OnLoad;

    private List<Entry> entries;
    private string path;

    private void Start()
    {
        path = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        if (!Directory.Exists(path))
        {
            string directoryPath = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                // Create the directory if it doesn't exist
                Directory.CreateDirectory(directoryPath);
            }
        }

        if (File.Exists(path))
        {
            var data = File.ReadAllText(path);
            var res = JsonUtility.FromJson<SerializableList<Entry>>(data);
            entries = res.list;
        }
        else
        {
            var now = DateTime.Now;
            entries = new List<Entry> { new(0, now), new(0, now), new(0, now), new(0, now), new(0, now) };
        }

        OnLoad?.Invoke();
        
        Debug.Log("Save location: " + path);
    }

    public List<Entry> Entries() => entries;

    public void AddEntry(Entry item)
    {
        if (item == null) return;

        int index = entries.BinarySearch(item);

        if (index < 0)
        {
            // BinarySearch returns bitwise complement of the insertion index if not found
            index = ~index;
            entries.Insert(index, item);
        }
    }

    public void RemoveEntry(Entry item)
    {
        int index = entries.BinarySearch(item);
        if (index >= 0)
        {
            entries.RemoveAt(index);
        }
    }

    public void SaveData()
    {
        Debug.Log("Entries Count: " + entries.Count.ToString());
        var wrapper = new SerializableList<Entry> { list = entries.GetRange(0, 5) };
        var json = JsonUtility.ToJson(wrapper);

        Debug.Log(json);
        File.WriteAllText(path, json);
    }
}
