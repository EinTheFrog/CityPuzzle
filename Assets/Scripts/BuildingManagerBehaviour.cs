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
        _cardDeckBehaviour.RemoveCardFromHand(Card);
        BuildBuilding(Card.building);
        Destroy(Card.gameObject);
        CancelCardSelection();
        CancelSoleSelection();
    }

    private void BuildBuilding(BuildingBehaviour building)
    {
        var buildingGameObj = Instantiate(building);
        var buildingTransform = buildingGameObj.transform;
        buildingTransform.SetParent(_buildingParentTransform);
        var soleTransform = Sole.transform;
        var soleScale = soleTransform.localScale;
        var soleBounds = Sole.GetComponent<Renderer>().bounds;
        var soleHeight = soleBounds.max.y * soleScale.y;
        var soleWidth = (soleBounds.max.x - soleBounds.min.x) * soleScale.x;
        buildingTransform.localPosition = soleTransform.localPosition + soleHeight * Vector3.up - soleWidth * Vector3.forward;
        buildingTransform.localRotation = new Quaternion(0, 0, 0, 0);
    }
}
