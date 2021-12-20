using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class CustomSaveSystem : MonoBehaviour
{
    [SerializeField] private GoldManagerBehaviour goldManagerBehaviour = default;
    [SerializeField] private BuildingManagerBehaviour buildingManagerBehaviour = default;
    
    private const string FileName = "/SavedGame.dat";
    private const int Version = 1;

    public void SaveGame()
    {
        var bf = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + FileName);
        var data = new GameData(
            Version,
            goldManagerBehaviour.Level, 
            goldManagerBehaviour.Gold, 
            buildingManagerBehaviour.Soles,
            buildingManagerBehaviour.BuildingsLength
            );
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }

    public void LoadGame()
    {
        var path = Application.persistentDataPath + FileName;
        if (File.Exists(path))
        {
            var bf = new BinaryFormatter();
            var file = File.Open(path, FileMode.Open);
            var data = (GameData)bf.Deserialize(file);
            file.Close();
            AssignGameData(data);
        }
        else
        {
            Debug.Log("There is no save data!");
        }
    }

    private void AssignGameData(GameData data)
    {
        switch (data.version)
        {
            case 1:
            {
                goldManagerBehaviour.LoadData(data.level, data.gold);
                buildingManagerBehaviour.DestroyAllBuildings();
                foreach (var buildingData in data.buildingsData)
                {
                    buildingManagerBehaviour.LoadBuilding(buildingData.type, buildingData.soleId);
                }
                break;
            }
        }
    }
}
