using System;

public class CardHand
{
    private CardBehaviour[] _cards;
    private int _currentSize = 0;

    public CardHand(int size)
    {
        _cards = new CardBehaviour[size];
    }

    public bool Add(CardBehaviour newCard)
    {
        if (_currentSize == _cards.Length) return false;
        _cards[_currentSize] = newCard;
        _currentSize++;
        return true;
    }

    public void Remove(CardBehaviour card)
    {
        var cardPos = GetCardPos(card);
        for (int i = cardPos + 1; i < _cards.Length; i++)
        {
            _cards[i - 1] = _cards[i];
        }
        _cards[_cards.Length - 1] = null;
        
        _currentSize--;
    }

    private int GetCardPos(CardBehaviour card)
    {
        var cardPos = -1;
        for (int i = 0; i < _cards.Length; i++)
        {
            if (_cards[i] == card)
            {
                _cards[i] = null;
                cardPos = i;
                break;
            }
        }

        if (cardPos == -1)
        {
            throw new Exception("Can't find such card in hand");
        }

        return cardPos;
    }

    public CardBehaviour Get(int pos)
    {
        return _cards[pos];
    }
}