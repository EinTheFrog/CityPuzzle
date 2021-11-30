using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BuildingManagerBehaviour : MonoBehaviour
{
    [SerializeField] private string buildingParentTag = default;
    [SerializeField] private GoldManagerBehaviour goldManager = default;
    public SoleBehaviour Sole { get; private set; }
    public CardBehaviour Card { get; private set; }

    private static readonly Vector3 FarAway = Vector3.up * 100; //handpicked value
    private static readonly string BuildingPrefabsPath = "Assets/SceneObjects/Buildings";

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
        if (Sole.IsOccupied) return false;

        EarnGoldForBuilding(building);
        
        var buildingInstance = Instantiate(building);
        buildingInstance.Initialize();
        PutBuildingOnSole(buildingInstance.gameObject, sole.gameObject);
        sole.Occupy(buildingInstance);
        _builtBuildings.Add(buildingInstance);
        return true;
    }

    public void LoadBuilding(BuildingType type, Vector2 pos)
    {
        BuildingBehaviour building = null;
        switch (type)
        {
            case BuildingType.Barracks:
                building = AssetDatabase.LoadAssetAtPath<BuildingBehaviour>(BuildingPrefabsPath + "/Barracks");
                break;
            case BuildingType.Church:
                building = AssetDatabase.LoadAssetAtPath<BuildingBehaviour>(BuildingPrefabsPath + "/Barracks");
                break;
            case BuildingType.Graveyard:
                building = AssetDatabase.LoadAssetAtPath<BuildingBehaviour>(BuildingPrefabsPath + "/Barracks");
                break;
            case BuildingType.House:
                building = AssetDatabase.LoadAssetAtPath<BuildingBehaviour>(BuildingPrefabsPath + "/Barracks");
                break;
            case BuildingType.Smithy:
                building = AssetDatabase.LoadAssetAtPath<BuildingBehaviour>(BuildingPrefabsPath + "/Barracks");
                break;
            case BuildingType.Well:
                building = AssetDatabase.LoadAssetAtPath<BuildingBehaviour>(BuildingPrefabsPath + "/Barracks");
                break;
        }

        var sole = FindSole(pos);
        BuildBuilding(building, sole);
    }

    private SoleBehaviour FindSole(Vector2 pos)
    {
        foreach (var sole in _soles)
        {
            var solePos = sole.transform.position;
            var soleCoords = new Vector2(solePos.x, solePos.z);
            if (soleCoords.Equals(pos))
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
        var soles = GetComponentsInChildren<SoleBehaviour>();
        foreach (var sole in soles)
        {
            sole.Free();
        }
    }
}
