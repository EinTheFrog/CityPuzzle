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
        transform.position = eventData.position;
        var y = transform.position.y;
        var alpha = Mathf.Clamp(1 - y / 200, 0.2f, 1f);
        _image.color = new Color(1, 1, 1, alpha);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var dragDest = GetDragDest(this);
        DragTo(dragDest);
        _buildingManager.TryToUseCard();
    }

    private void UseCard()
    {
        Destroy(this);
    }
    
    public void DragTo(Vector2 dest)
    {
        _freeToDrag = true;
        _dragDestination = dest;
    }

    private void DragUpdate()
    {
        var pos = new Vector2(transform.localPosition.x, transform.localPosition.y);
        var path = _dragDestination - pos;
        if (path.magnitude < Speed * Time.deltaTime)
        {
            _freeToDrag = false;
            return;
        }

        var direction = path.normalized;
        var d = new Vector3(direction.x, direction.y, 0) * Time.deltaTime * Speed;
        transform.position += d;
    }
}
