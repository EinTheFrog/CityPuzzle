
using UnityEngine;

[System.Serializable]
public class BuildingData
{
    public BuildingType type;
    public Vector2 pos;

    public BuildingData(BuildingType type, Vector2 pos)
    {
        this.type = type;
        this.pos = pos;
    }
}