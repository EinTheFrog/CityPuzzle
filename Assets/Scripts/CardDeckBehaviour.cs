using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CardDeckBehaviour : MonoBehaviour
{
    [SerializeField] private string tableTag = "Table";
    [SerializeField] private CardBehaviour[] cardDeck;
    [SerializeField] private int cardsPerSpawn = 2;
    [SerializeField] private int handSize = 6;
    [SerializeField] [Range(0, 1.5f)] private float spaceBetweenCards = 0.5f;
    [SerializeField] private float tableBorderOffset = 20f;
    [SerializeField] private float cardSpeed = 1f;

    private RectTransform _table;
    private Vector2 _tableCenter;
    private float _tableWidth;
    private float _cardWidth;
    private CardHand _cardHand;
    private Vector2 _cardSpawnPos;
    private float _spaceBetweenCards;

    private void Start()
    {
        if (cardDeck.Length == 0) throw new Exception("Deck is empty!");
        
        _cardHand = new CardHand(handSize);
        
        GetTableParameters();
        _cardWidth = cardDeck[0].GetComponent<RectTransform>().rect.width;
        SpawnCards();
    }

    private void GetTableParameters()
    {
        _table = GameObject.FindGameObjectWithTag(tableTag).GetComponent<RectTransform>();
        var tableRect = _table.rect;
        _tableCenter = tableRect.center;
        _tableWidth = tableRect.width;
        _cardSpawnPos = new Vector3(tableRect.xMax, tableRect.yMax, 0);
    }

    private void SpawnCards()
    {
        int rnd = 0;
        for (int i = 0; i < cardsPerSpawn; i++)
        {
            rnd = Random.Range(0, cardDeck.Length);
            var card = Instantiate(cardDeck[rnd]);
            
            card.GetDragDest = GetCardDragDest;
            card.Speed = cardSpeed;
            var cardTransform = card.transform;
            cardTransform.parent = _table.transform;
            cardTransform.localPosition = _cardSpawnPos;
            
            _cardHand.Add(card);
        }

        var cardsWidthSum = _cardHand.CurrentSize * _cardWidth;
        var tableAvailableWidth = _tableWidth - 2 * tableBorderOffset;
        _spaceBetweenCards = cardsWidthSum * spaceBetweenCards > tableAvailableWidth
            ? tableAvailableWidth / cardsWidthSum
            : spaceBetweenCards;

        for (int i = 0; i < _cardHand.CurrentSize; i++)
        {
            _cardHand.Get(i).DragTo(CalculateCardDest(i));
        }
    }

    private void AddCardToHand(CardBehaviour card)
    {
        _cardHand.Add(card);
    }

    private void RemoveCardFromHand(CardBehaviour card)
    {
        _cardHand.Remove(card);
    }
    

    private Vector2 GetCardDragDest(CardBehaviour card)
    {
        for (int i = 0; i < handSize; i++)
        {
            if (card != _cardHand.Get(i)) continue;
            var dest = CalculateCardDest(i);
            return dest;
        }

        throw new Exception("Can't find such card");
    }

    private Vector2 CalculateCardDest(int deckPos)
    {
        var centerOffset = (_cardHand.CurrentSize - 1) / 2f;
        var space = _spaceBetweenCards * _cardWidth;
        var res = _tableCenter + Vector2.right * (deckPos - centerOffset) * space;
        return res;
    }
        
}
