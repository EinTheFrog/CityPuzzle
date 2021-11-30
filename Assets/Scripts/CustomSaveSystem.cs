using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class CustomSaveSystem : MonoBehaviour
{
    private const string FileName = "savedGame";
    
    public void SaveGame(GameData gameData)
    {
        var bf = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath)
    }
}
