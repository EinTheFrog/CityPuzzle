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
    [SerializeField] private string freeSpaceTag = "FreeSpace";
    [SerializeField] private string cardsParent = "CardsParent";
    [SerializeField] private CardBehaviour[] cardDeck;
    [SerializeField] private int cardsPerSpawn = 2;
    [SerializeField] private int handSize = 6;
    [SerializeField] [Range(0, 1.5f)] private float spaceBetweenCards = 0.5f;
    [SerializeField] private float tableBorderOffset = 20f;
    [SerializeField] private float cardSpeed = 1f;
    [SerializeField] private int maxCardsInRowCount = 2;

    private RectTransform _table;
    private Vector2 _tableCenter;
    private float _tableWidth;
    private float _cardWidth;
    private CardHand _cardHand;
    private Vector2 _cardSpawnPos;
    private float _spaceBetweenCards;
    private int _cardsRndMax = 0;
    private CardBehaviour[] _cardsRndRange;
    private BuildingType _lastSpawnedCardName;
    private int _cardsInRow = 0;
    private RectTransform _freeSpace;
    private Vector2 _screenCenter;
    private GameObject _cardsParent;

    private void Start()
    {
        if (cardDeck.Length == 0) throw new Exception("Deck is empty!");
        
        _cardHand = new CardHand(handSize);
        
        Invoke(nameof(GetTableParameters), 0.01f);
        _cardWidth = cardDeck[0].GetComponent<RectTransform>().rect.width;
        Invoke(nameof(GetScreenCenter), 0.015f);
        _cardsParent = GameObject.FindGameObjectWithTag(cardsParent);
        
        foreach (var card in cardDeck)
        {
            _cardsRndMax += card.Frequency;
        }

        _cardsRndRange = new CardBehaviour[_cardsRndMax];
        var k = 0;
        foreach (var card in cardDeck)
        {
            var freq = card.Frequency;
            for (int i = 0; i < freq; i++)
            {
                _cardsRndRange[k] = card;
                k++;
            }
        }
    }

    private void GetTableParameters()
    {
        _table = GameObject.FindGameObjectWithTag(tableTag).GetComponent<RectTransform>();
        var tableRect = _table.rect;
        _tableCenter = _table.localPosition;
        _tableCenter += tableRect.center;
        _tableWidth = tableRect.width;
        _cardSpawnPos = new Vector2(tableRect.xMax + 100, _tableCenter.y);
    }

    private void GetScreenCenter()
    {
        _freeSpace = GameObject.FindGameObjectWithTag(freeSpaceTag).GetComponent<RectTransform>();
        _screenCenter = _freeSpace.rect.center;
    }

    public void SpawnCards()
    {
        UnZoomAllCards();
        var rnd = 0;
        for (int i = 0; i < cardsPerSpawn; i++)
        {
            var k = 0;
            do
            {
                k++;
                rnd = Random.Range(0, _cardsRndMax);
                if (_cardsRndRange[rnd].building.BuildingType == _lastSpawnedCardName)
                {
                    _cardsInRow++;
                }
                else
                {
                    _cardsInRow = 1;
                }
            } while (_cardsInRow > maxCardsInRowCount & k < 20) ;

            var card = Instantiate(_cardsRndRange[rnd]);
            var handIsFull = !_cardHand.Add(card);
            if (handIsFull)
            {
                Destroy(card.gameObject);
                break;
            }
            SetCardParameters(card);
            _lastSpawnedCardName = card.building.BuildingType;
        }
        CalculateSpaceBetweenCards();
        UpdateCardsPos();
    }

    public void LoadCards(int[] cardTypes)
    {
        ClearHand();
        foreach (var type in cardTypes)
        {
            if (type == GameData.NoCardConst) continue;
            var card = Instantiate(cardDeck[type]);
            var handIsFull = !_cardHand.Add(card);
            if (handIsFull)
            {
                Destroy(card.gameObject);
                break;
            }
            SetCardParameters(card);
            _lastSpawnedCardName = card.building.BuildingType;
        }
        CalculateSpaceBetweenCards();
        UpdateCardsPos();
    }

    private void SetCardParameters(CardBehaviour card)
    {
        card.GetTableDest = GetCardDragDest;
        card.GetZoomDest = GetCardZoomDest;
        card.UnZoomAllCards = UnZoomAllCards;
        card.Speed = cardSpeed;
        var cardTransform = card.transform;
        cardTransform.SetParent(_cardsParent.transform);
        cardTransform.localPosition = _cardSpawnPos;
    }

    private void CalculateSpaceBetweenCards()
    {
        var cardsWidthSum = _cardHand.CurrentSize * _cardWidth;
        var tableAvailableWidth = _tableWidth - 2 * tableBorderOffset;
        _spaceBetweenCards = cardsWidthSum * spaceBetweenCards > tableAvailableWidth
            ? tableAvailableWidth / cardsWidthSum
            : spaceBetweenCards;
    }

    public void RemoveCardFromHand(CardBehaviour card)
    {
        _cardHand.Remove(card);
        UpdateCardsPos();
    }

    public void ClearHand()
    {
        for (int i = 0; i < handSize; i++)
        {
            var card = _cardHand.Get(i);
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
        _cardHand.Clear();
    }

    private void UpdateCardsPos()
    {
        for (int i = 0; i < handSize; i++)
        {
            var card = _cardHand.Get(i);
            if (card == null) break;
            card.DragTo(CalculateCardDest(i));
        }
    }

    private Vector2 GetCardDragDest(CardBehaviour card)
    {
        for (int i = 0; i < handSize; i++)
        {
            if (card != _cardHand.Get(i)) continue;
            var dest = CalculateCardDest(i);
            card.transform.SetSiblingIndex(i);
            return dest;
        }

        throw new Exception("Can't find such card");
    }

    private Vector2 GetCardZoomDest(CardBehaviour card) => _screenCenter;

    private Vector2 CalculateCardDest(int deckPos)
    {
        var centerOffset = (_cardHand.CurrentSize - 1) / 2f;
        var space = _spaceBetweenCards * _cardWidth;
        var res = _tableCenter + Vector2.right * (deckPos - centerOffset) * space;
        return res;
    }

    public CardBehaviour[] GetCardsInHand()
    {
        return _cardHand.cards;
    }

    private void UnZoomAllCards()
    {
        for (int i = 0; i < handSize; i++)
        {
            var card = _cardHand.Get(i);
            if (card == null) break;
            card.Zoom(false);
        }
    }
}
