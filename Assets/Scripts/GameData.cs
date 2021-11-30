using System.Collections.Generic;
[System.Serializable]
public class GameData
{
    public int level;
    public int gold;
    public BuildingData[] buildingsData;

    private GameData(int level, int gold, SoleBehaviour[] soles, int buildingsLength)
    {
        this.level = level;
        this.gold = gold;
        buildingsData = new BuildingData[buildingsLength];
        var i = 0;
        foreach (var sole in soles)
        {
            if (sole.Building == null) continue;
            buildingsData[i] = new BuildingData(sole.Building.BuildingType, sole.transform.position);
            i++;
        }
    }
}