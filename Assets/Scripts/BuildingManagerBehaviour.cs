using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class BuildingManagerBehaviour : MonoBehaviour
{
    [SerializeField] private string buildingParentTag; 
    public SoleBehaviour Sole { get; private set; }
    public CardBehaviour Card { get; private set; }

    private static readonly Vector3 FarAway = Vector3.up * 100; //handpicked value

    private CardDeckBehaviour _cardDeckBehaviour;
    private Transform _buildingParentTransform;
    private BuildingBehaviour _ghost;
    private SoleBehaviour[] _soles;
    private List<BuildingBehaviour> _builtBuildings;
    private List<BuildingBehaviour> _buildingsForEarning;

    private void Start()
    {
        _cardDeckBehaviour = FindObjectOfType<CardDeckBehaviour>();
        _buildingParentTransform = GameObject.FindWithTag(buildingParentTag).transform;
        _soles = FindObjectsOfType<SoleBehaviour>();
        _builtBuildings = new List<BuildingBehaviour>();
        _buildingsForEarning = new List<BuildingBehaviour>();
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
            var buildingWasBuilt = BuildBuilding(Card.building);
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
        _ghost.gameObject.SetActive(false);
    }

    private void DestroyGhost()
    {
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
            _soles[i].UnderHouse = false;
        }
    }

    private bool BuildBuilding(BuildingBehaviour building)
    {
        if (Sole.IsOccupied) return false;
        
        var buildingInstance = Instantiate(building);
        buildingInstance.GetComponentInChildren<BoxCollider>().enabled = false;
        PutBuildingOnSole(buildingInstance.gameObject, Sole.gameObject);

        Sole.Occupy();
        // Adding callback
        buildingInstance.underBuildingEvent = AddBuildingForEarning;
        _builtBuildings.Add(buildingInstance);
        return true;
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

    public void AddBuildingForEarning(BuildingBehaviour building)
    {
        _buildingsForEarning.Add(building);
    }
}
