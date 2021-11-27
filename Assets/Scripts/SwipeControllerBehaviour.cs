using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeControllerBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private EarthBehaviour earth = default;
    
    private float _xBegin, _xEnd;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _xBegin = eventData.position.x;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _xEnd = eventData.position.x;
        RotateEarth();
    }
    private void RotateEarth()
    {
        if (_xBegin > _xEnd)
        {
            earth.RotateRight();
        }
        else
        {
            earth.RotateLeft();
        }
    }
}
