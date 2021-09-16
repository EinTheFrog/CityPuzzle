using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CardDeckBehaviour : MonoBehaviour
{
    [SerializeField] private string tableTag = "Table";
    [SerializeField] private CardBehaviour[] cardsDeck;
    [SerializeField] private int cardsPerSpawn = 2;
    [SerializeField] private int handSize = 6;
    [SerializeField] private float spaceBetweenCards = 1.5f;
    [SerializeField] private float cardSpeed = 1f;
    
    private Vector2 _tableCenter = default;
    private CardHand _cardsHand;

    private void Start()
    {
        _cardsHand = new CardHand(handSize);
        
        FindTableCenter();
        SpawnCards();
    }

    private void FindTableCenter()
    {
        var table = GameObject.FindGameObjectWithTag(tableTag).GetComponent<RectTransform>();
        _tableCenter = table.rect.center;
    }

    private void SpawnCards()
    {
        int rnd = 0;
        for (int i = 0; i < cardsPerSpawn; i++)
        {
            rnd = Random.Range(0, cardsDeck.Length);
            var card = Instantiate(cardsDeck[rnd]);
        }
    }

    private void AddCardToHand(CardBehaviour card)
    {
        _cardsHand.Add(card);
    }

    private void RemoveCardFromHand(CardBehaviour card)
    {
        _cardsHand.Remove(card);
    }
    

    private void DragCardsToTable()
    {
        for (int i = 0; i < handSize; i++)
        {
            var dest = _tableCenter + Vector2.right * i * spaceBetweenCards;
            _cardsHand.Get(i)?.DragTo(dest, cardSpeed);
        }
    }
}
