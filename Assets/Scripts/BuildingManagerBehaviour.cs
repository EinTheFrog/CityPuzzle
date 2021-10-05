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

    private CardDeckBehaviour _cardDeckBehaviour;
    private Transform _buildingParentTransform;

    private void Start()
    {
        _cardDeckBehaviour = FindObjectOfType<CardDeckBehaviour>();
        _buildingParentTransform = GameObject.FindWithTag(buildingParentTag).transform;
    }

    public void SelectSole(SoleBehaviour sole)
    {
        if (sole == null) return;
        Sole = sole;
    }

    public void CancelSoleSelection()
    {
        Sole = null;
    }
    
    public void SelectCard(CardBehaviour card)
    {
        if (card == null) return;
        Card = card;
    }
    
    public void CancelCardSelection()
    {
        Card = null;
    }

    public void TryToUseCard()
    {
        if (Card == null || Sole == null) return;
        var buildingWasBuilt = BuildBuilding(Card.building);
        if (buildingWasBuilt)
        {
            _cardDeckBehaviour.RemoveCardFromHand(Card);
            Destroy(Card.gameObject);
        }
        CancelCardSelection();
        CancelSoleSelection();
    }

    private bool BuildBuilding(BuildingBehaviour building)
    {
        if (Sole.IsOccupied) return false;
        
        var buildingGameObj = Instantiate(building);
        PutBuildingOnSole(buildingGameObj, Sole);
        
        Sole.Occupy();
        return true;
    }

    private void PutBuildingOnSole(BuildingBehaviour building, SoleBehaviour sole)
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
}
