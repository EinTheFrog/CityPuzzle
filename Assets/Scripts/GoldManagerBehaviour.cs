using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldManagerBehaviour : MonoBehaviour
{
    [SerializeField] private Text goldText = default;

    private int _gold = 0;
    private void Start()
    {
        goldText.text = _gold.ToString();
    }

    public void EarnGold(int count)
    {
        if (count <= 0)
        {
            throw new Exception("Gold count must be greater than zero");
        }
        _gold += count;
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
    }
}
