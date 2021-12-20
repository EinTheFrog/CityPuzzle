using System.Collections.Generic;
[System.Serializable]
public class GameData
{
    public int version;
    public int level;
    public int gold;
    public BuildingData[] buildingsData;

    public GameData(int version, int level, int gold, SoleBehaviour[] soles, int buildingsLength)
    {
        this.version = version;
        this.level = level;
        this.gold = gold;
        buildingsData = new BuildingData[buildingsLength];
        var i = 0;
        foreach (var sole in soles)
        {
            if (sole.Building == null) continue;
            buildingsData[i] = new BuildingData(sole.Building.BuildingType, sole.transform.position, sole.Id);
            i++;
        }
    }
}