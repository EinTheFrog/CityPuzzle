using System;

public class CardHand
{
    public CardBehaviour[] cards;

    public int CurrentSize { private set; get; }

    public CardHand(int size)
    {
        cards = new CardBehaviour[size];
    }

    public bool Add(CardBehaviour newCard)
    {
        if (CurrentSize == cards.Length) return false;
        cards[CurrentSize] = newCard;
        CurrentSize++;
        return true;
    }

    public void Remove(CardBehaviour card)
    {
        var cardPos = GetCardPos(card);
        for (int i = cardPos + 1; i < cards.Length; i++)
        {
            cards[i - 1] = cards[i];
        }
        cards[cards.Length - 1] = null;
        
        CurrentSize--;
    }

    private int GetCardPos(CardBehaviour card)
    {
        var cardPos = -1;
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == card)
            {
                cards[i] = null;
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
        return cards[pos];
    }

    public void Clear()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = null;
        }

        CurrentSize = 0;
    }
}