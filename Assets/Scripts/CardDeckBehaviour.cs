using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeckBehaviour : MonoBehaviour
{
    [SerializeField] private static readonly int cardsCount = 1;
    [SerializeField] private static readonly CardBehaviour[] Cards = new CardBehaviour[cardsCount];
    [SerializeField] private int cardsPerSpawn = 2;

    private void SpawnCards()
    {
        int k = 0;
        for (int i = 0; i < cardsPerSpawn; i++)
        {
            k = Random.Range(0, cardsCount);
            var card = Instantiate(Cards[k]);
        }
    }
}
