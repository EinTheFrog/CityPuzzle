using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManagerBehaviour : MonoBehaviour
{
    public SoleBehaviour Sole { get; private set; }
    public CardBehaviour Card { get; private set; }

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
        Destroy(Card.gameObject);
        CancelCardSelection();
        CancelSoleSelection();
    }
}
