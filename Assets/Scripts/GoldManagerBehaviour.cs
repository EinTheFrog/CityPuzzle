using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldManagerBehaviour : MonoBehaviour
{
    [SerializeField] private Text goldText = default;
    [SerializeField] private Button btnNextLevel = default;
    [SerializeField] private int goldForNextLevel = 50;
    [SerializeField] private Image loadingScreen = default;
    [SerializeField] private BuildingManagerBehaviour buildingManager = default;
    [SerializeField] private CardDeckBehaviour cardDeck = default;

    private int _gold = 0;
    
    private void Start()
    {
        goldText.text = _gold.ToString();
        btnNextLevel.enabled = false;
        loadingScreen.enabled = false;
    }

    public void EarnGold(int count)
    {
        if (count <= 0)
        {
            throw new Exception("Gold count must be greater than zero");
        }
        _gold += count;
        if (_gold >= goldForNextLevel)
        {
            btnNextLevel.enabled = true;
        }
        goldText.text = _gold.ToString();
    }
    
    public void SpendGold(int count)
    {
        if (count <= 0)
        {
            throw new Exception("Gold count must be greater than zero");
        }

        _gold -= count;
        if (_gold < 0)
        {
            _gold = 0;
        }
        goldText.text = _gold.ToString();
    }

    public void StartNextLevel()
    {
        SpendGold(goldForNextLevel);
        loadingScreen.enabled = true;
        buildingManager.DestroyAllBuildings();
        cardDeck.ClearHand();
        btnNextLevel.enabled = false;
        loadingScreen.enabled = false;
    }
}
