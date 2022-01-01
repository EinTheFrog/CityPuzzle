using System.Collections.Generic;
[System.Serializable]
public class GameData
{
    public static readonly int NoCardConst = -1;
    
    public int version;
    public int level;
    public int gold;
    public BuildingData[] buildingsData;
    public int[] cardTypes;

    public GameData(int version, int level, int gold, SoleBehaviour[] soles, int buildingsLength, CardBehaviour[] cards)
    {
        this.version = version;
        this.level = level;
        this.gold = gold;
        buildingsData = new BuildingData[buildingsLength];
        var k = 0;
        foreach (var sole in soles)
        {
            if (sole.Building == null) continue;
            buildingsData[k] = new BuildingData(sole.Building.BuildingType, sole.transform.position, sole.Id);
            k++;
        }

        cardTypes = new int[cards.Length];
        for (var i = 0; i < cards.Length; i++)
        {
            var card = cards[i];
            if (card == null)
            {
                cardTypes[i] = NoCardConst;
                continue;
            }
            switch (card.building.BuildingType)
            {
                case BuildingType.Barracks:
                    cardTypes[i] = 0; break;
                case BuildingType.Church: 
                    cardTypes[i] = 1; break;
                case BuildingType.Graveyard: 
                    cardTypes[i] = 2; break;
                case BuildingType.House: 
                    cardTypes[i] = 3; break;
                case BuildingType.Smithy: 
                    cardTypes[i] = 4; break;
                case BuildingType.Well: 
                    cardTypes[i] = 5; break;
            }
        }
    }
}