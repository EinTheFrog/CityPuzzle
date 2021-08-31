using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CardBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private BuildingManagerBehaviour _buildingManager = default;
    private Image _image = default;
    private void Start()
    {
        _buildingManager = FindObjectOfType<BuildingManagerBehaviour>();
        _image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _buildingManager.SelectCard(this);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        var y = transform.localPosition.y;
        var alpha = Mathf.Clamp(1 - y / 200, 0.2f, 1f);
        _image.color = new Color(1, 1, 1, alpha);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _buildingManager.TryToUseCard();
    }

    private void UseCard()
    {
        Destroy(this);
    }
}
