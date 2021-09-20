using System;

public class CardHand
{
    private CardBehaviour[] _cards;

    public int CurrentSize { private set; get; }

    public CardHand(int size)
    {
        _cards = new CardBehaviour[size];
    }

    public bool Add(CardBehaviour newCard)
    {
        if (CurrentSize == _cards.Length) return false;
        _cards[CurrentSize] = newCard;
        CurrentSize++;
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
        
        CurrentSize--;
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