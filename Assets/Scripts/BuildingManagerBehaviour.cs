using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BuildingManagerBehaviour : MonoBehaviour
{
    [SerializeField] private string buildingParentTag = default;
    [SerializeField] private GoldManagerBehaviour goldManager = default;
    public SoleBehaviour Sole { get; private set; }
    public CardBehaviour Card { get; private set; }
    public SoleBehaviour[] Soles => _soles;
    public int BuildingsLength => _builtBuildings.Count;

    private static readonly Vector3 FarAway = Vector3.up * 100; //handpicked value
    private const string BuildingPrefabsPath = "Assets/Prefabs/SceneObjects/Buildings";

    private CardDeckBehaviour _cardDeckBehaviour;
    private Transform _buildingParentTransform;
    private BuildingBehaviour _ghost;
    private SoleBehaviour[] _soles;
    private List<BuildingBehaviour> _builtBuildings;

    private void Start()
    {
        _cardDeckBehaviour = FindObjectOfType<CardDeckBehaviour>();
        _buildingParentTransform = GameObject.FindWithTag(buildingParentTag).transform;
        _soles = FindObjectsOfType<SoleBehaviour>();
        _builtBuildings = new List<BuildingBehaviour>();
    }

    public void SelectSole(SoleBehaviour sole)
    {
        if (sole == null) return;
        Sole = sole;
        
        if (Card == null) return;
        ShowGhost();
    }

    public void CancelSoleSelection()
    {
        Sole = null;
        if (Card != null)
        {
            HideGhost();
        }
    }
    
    public void SelectCard(CardBehaviour card)
    {
        if (card == null) return;
        Card = card;
        SpawnGhost(Card.building);
    }
    
    public void CancelCardSelection()
    {
        Card = null;
        DestroyGhost();
    }

    public void TryToUseCard()
    {
        if (Card != null && Sole != null)
        {
            var buildingWasBuilt = BuildBuilding(Card.building, Sole);
            if (buildingWasBuilt)
            {
                _cardDeckBehaviour.RemoveCardFromHand(Card);
                Destroy(Card.gameObject);
            }
        }
        CancelCardSelection();
        CancelSoleSelection();
    }

    private void SpawnGhost(BuildingBehaviour building)
    {
        _ghost = Instantiate(building);
        _ghost.GetComponentInChildren<MeshRenderer>().enabled = false;
        _ghost.GetComponentInChildren<TextMesh>().text = "";
        _ghost.gameObject.SetActive(false);
    }

    private void DestroyGhost()
    {
        if (_ghost == null) return;
        Destroy(_ghost.gameObject);
        _ghost = null;
        RestoreSolesColor();
    }
    private void ShowGhost()
    {
        var ghostGameObj = _ghost.gameObject;
        ghostGameObj.SetActive(true);
        PutBuildingOnSole(ghostGameObj, Sole.gameObject);
    }

    private void HideGhost()
    {
        var ghostGameObj = _ghost.gameObject;
        ghostGameObj.transform.localPosition = FarAway;
        ghostGameObj.SetActive(false);
        RestoreSolesColor();
    }

    private void RestoreSolesColor()
    {
        for (int i = 0; i < _soles.Length; i++)
        {
            _soles[i].RestoreColor();
            _soles[i].UnderGhost = false;
        }
    }

    private bool BuildBuilding(BuildingBehaviour building, SoleBehaviour sole)
    {
        if (sole.IsOccupied) return false;

        EarnGoldForBuilding(building);
        
        var buildingInstance = Instantiate(building);
        buildingInstance.Initialize();
        PutBuildingOnSole(buildingInstance.gameObject, sole.gameObject);
        sole.Occupy(buildingInstance);
        _builtBuildings.Add(buildingInstance);
        return true;
    }

    public async void LoadBuilding(BuildingType type, int soleId)
    {
        string buildingPath = "";
        switch (type)
        {
            case BuildingType.Barracks:
                buildingPath = BuildingPrefabsPath + "/Barracks.prefab";
                break;
            case BuildingType.Church:
                buildingPath = BuildingPrefabsPath + "/Church.prefab";
                break;
            case BuildingType.Graveyard:
                buildingPath = BuildingPrefabsPath + "/Graveyard.prefab";
                break;
            case BuildingType.House:
                buildingPath = BuildingPrefabsPath + "/House.prefab";
                break;
            case BuildingType.Smithy:
                buildingPath = BuildingPrefabsPath + "/Smithy.prefab";
                break;
            case BuildingType.Well:
                buildingPath = BuildingPrefabsPath + "/Well.prefab";
                break;
        }

        var op = Addressables.LoadAssetAsync<BuildingBehaviour>(buildingPath);
        var task = await op.Task;
        var building = op.Result;
        var sole = FindSole(soleId);
        BuildBuilding(building, sole);
    }

    private SoleBehaviour FindSole(int id)
    {
        foreach (var sole in _soles)
        {
            if (sole.Id == id)
            {
                return sole;
            }
        }

        throw new Exception("Couldn't find sole");
    }

    private void PutBuildingOnSole(GameObject building, GameObject sole)
    {
        var buildingTransform = building.transform;
        buildingTransform.SetParent(_buildingParentTransform);
        var soleTransform = sole.transform;
        var soleScale = soleTransform.localScale;
        var soleBounds = sole.GetComponent<Renderer>().bounds;
        var soleHeight = soleBounds.max.y * soleScale.y;
        var posCorrection = soleHeight * Vector3.up;
        buildingTransform.localPosition = soleTransform.localPosition + posCorrection;
        buildingTransform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    private void EarnGoldForBuilding(BuildingBehaviour earnBuilding)
    {
        var gold = 0;
        foreach (var building in _builtBuildings)
        {
            if (building.UnderGhost)
            {
                gold += FindValueInSIArray(building.GoldForBuildings, earnBuilding.BuildingType);
            }
        }

        if (gold > 0)
        {
            goldManager.EarnGold(gold);
        }
        if (gold < 0)
        {
            goldManager.SpendGold(Mathf.Abs(gold));
        }
    }

    private int FindValueInSIArray(StringInt[] stringInts, BuildingType key)
    {
        foreach (var pair in stringInts)
        {
            if (pair.key == key)
            {
                return pair.value;
            }
        }

        return 0;
    }

    public void DestroyAllBuildings()
    {
        foreach (var building in _builtBuildings)
        {
            Destroy(building.gameObject);
        }
        _builtBuildings.Clear();
        DestroyGhost();
        FreeSoles();
    }
    
    private void FreeSoles()
    {
        foreach (var sole in _soles)
        {
            sole.Free();
        }
    }
}
