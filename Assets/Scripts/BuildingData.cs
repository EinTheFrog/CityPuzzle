
using UnityEngine;

[System.Serializable]
public class BuildingData
{
    public BuildingType type;
    public int soleId;

    public BuildingData(BuildingType type, Vector2 pos, int soleId)
    {
        this.type = type;
        this.soleId = soleId;
    }
}