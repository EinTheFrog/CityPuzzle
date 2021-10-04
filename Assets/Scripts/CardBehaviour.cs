using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CardBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public BuildingBehaviour building;
    
    private const float InertiaMultiplier = 0.4f; //handpicked value
    private const float TransparencyDenominator = 100f; //handpicked value
    
    private BuildingManagerBehaviour _buildingManager = default;
    private Image _image = default;
    private bool _freeToDrag = false;
    private Vector2 _dragDestination = default;

    public float Speed { get; set; }
    public delegate Vector2 DragDestDel(CardBehaviour card);

    public DragDestDel GetDragDest;
    private void Start()
    {
        _buildingManager = FindObjectOfType<BuildingManagerBehaviour>();
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        if (_freeToDrag)
        {
            DragUpdate();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _buildingManager.SelectCard(this);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _freeToDrag = false;
        transform.position = eventData.position;
        CalculateAlpha();
    }

    private void CalculateAlpha()
    {
        var y = transform.localPosition.y;
        var dragDest = GetDragDest(this);
        var dy = Mathf.Clamp(y - dragDest.y, 0, TransparencyDenominator);
        var alpha = Mathf.Clamp(1 - dy / TransparencyDenominator, 0.2f, 1f);
        _image.color = new Color(1, 1, 1, alpha);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _image.color = new Color(1, 1, 1, 1);
        var dragDest = GetDragDest(this);
        DragTo(dragDest);
        _buildingManager.TryToUseCard();
    }
    
    public void DragTo(Vector2 dest)
    {
        _freeToDrag = true;
        _dragDestination = dest;
    }

    private void DragUpdate()
    {
        var localPas = transform.localPosition;
        var pos = new Vector2(localPas.x, localPas.y);
        var path = _dragDestination - pos;
        var pathLength = path.magnitude;
        if (pathLength < Speed * Time.deltaTime)
        {
            _freeToDrag = false;
            return;
        }
        
        var direction = path.normalized;
        var speedMultiplier = Mathf.Clamp(pathLength / (Speed * InertiaMultiplier), 0.1f, 3f);
        var d = new Vector3(direction.x, direction.y, 0) * Time.deltaTime * Speed * speedMultiplier;
        transform.position += d;
    }
}
