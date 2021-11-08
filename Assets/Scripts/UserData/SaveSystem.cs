using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem
{

    #region Singleton
    private static SaveSystem instance = new SaveSystem();
    public static SaveSystem Instance => instance;
    #endregion
    private SaveSystem() { Load(); }
    private bool webgl = true;
    private const string key = "userData";

    public string Path => Application.dataPath + "/data.json";
    public UserData UserData { get; private set; }


    public void ChangeDeckData(SortedCardList deckData)
    {
        var deck = deckData.GetData();
        UserData.Deck = new List<UserData_Card>();
        foreach (var item in deck)
        {
            UserData.Deck.Add(new UserData_Card(item.Key,item.Value));
        }
    }

    public void Save()
    {
        string jsonData = JsonUtility.ToJson(UserData);

        if (webgl)
        {
            PlayerPrefs.SetString(key, jsonData);
            PlayerPrefs.Save();
        }
        else
        {
            StreamWriter writer = new StreamWriter(Path, false);
            writer.WriteLine(jsonData);
            writer.Flush();
            writer.Close();
        }
    }

    public void Load()
    {
        if (webgl)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                UserData = new UserData();
                Save();
                return;
            }
            else
            {
                UserData = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString(key));
            }
        }
        else
        {
            if (!File.Exists(Path))
            {
                UserData = new UserData();
                Save();
                return;
            }

            StreamReader reader = new StreamReader(Path);
            string jsonData = reader.ReadToEnd();
            UserData = JsonUtility.FromJson<UserData>(jsonData);
            reader.Close();
        }
    }
}
