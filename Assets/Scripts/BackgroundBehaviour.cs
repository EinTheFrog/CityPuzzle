using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBehaviour : MonoBehaviour
{
    private BuildingManagerBehaviour _buildingManager;
    void Start()
    {
        _buildingManager = FindObjectOfType<BuildingManagerBehaviour>();
    }

    private void OnMouseEnter()
    {
        _buildingManager.CancelSoleSelection();
    }
}
