using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;


public class SaveDataManager : MonoBehaviour
{
    private string filePath;
    // Start is called before the first frame update
    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "GameData.json");
    }

    public void SaveData(GameData gameData)
    {
        string jsonData = JsonConvert.SerializeObject(gameData, Formatting.Indented);
        File.WriteAllText(filePath, jsonData);
    }
}
