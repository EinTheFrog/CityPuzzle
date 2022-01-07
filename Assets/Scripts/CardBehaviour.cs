using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class CardBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private int frequency = 0;

    public BuildingBehaviour building;
    
    private const float InertiaMultiplier = 0.4f; // Handpicked value
    private const float TransparencyDenominator = 100f; // Handpicked value
    private const float ZoomScale = 2f;

    private BuildingManagerBehaviour _buildingManager = default;
    private Image _image = default;
    private bool _freeToDrag = false;
    private Vector2 _dragDestination = default;
    private bool _isZoomed = false;
    private bool _isDragged = false;

    public float Speed { get; set; }
    public int Frequency => frequency;
    public delegate Vector2 DragDestDel(CardBehaviour card);
    public delegate void AffectAllCards();

    public DragDestDel GetTableDest;
    public DragDestDel GetZoomDest;
    public AffectAllCards UnZoomAllCards;
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
        _isDragged = true;
        UnZoomAllCards();
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
        var dragDest = GetTableDest(this);
        var dy = Mathf.Clamp(y - dragDest.y, 0, TransparencyDenominator);
        var alpha = Mathf.Clamp(1 - dy / TransparencyDenominator, 0.2f, 1f);
        _image.color = new Color(1, 1, 1, alpha);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragged = false;
        _image.color = new Color(1, 1, 1, 1);
        var dragDest = GetTableDest(this);
        DragTo(dragDest);
        _buildingManager.TryToUseCard();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isDragged) return;
        Zoom(!_isZoomed);
    }

    public void Zoom(bool zoom)
    {
        if (zoom == _isZoomed) return;
        
        if (zoom)
        {
            transform.localScale *= ZoomScale;
            var screenCenter = GetZoomDest(this);
            transform.SetAsLastSibling();
            DragTo(screenCenter);
            _isZoomed = true;
        }
        else
        {
            var placeOnTable = GetTableDest(this);
            DragTo(placeOnTable);
            transform.localScale /= ZoomScale;
            _isZoomed = false;
        }
    }
    
    public void DragTo(Vector2 dest)
    {
        _freeToDrag = true;
        _dragDestination = dest;
    }

    private void DragUpdate()
    {
        var rectTransform = GetComponent<RectTransform>();
        var localPos = rectTransform.localPosition;
        var pos = new Vector2(localPos.x, localPos.y);
        var path = _dragDestination - pos;
        var pathLength = path.magnitude;
        if (pathLength < Speed * Time.deltaTime)
        {
            _freeToDrag = false;
            return;
        }
        
        var direction = path.normalized;
        var speedMultiplier = Mathf.Clamp(pathLength / (Speed * InertiaMultiplier), 0.1f, 3f);
        var d = direction * Time.deltaTime * Speed * speedMultiplier;
        rectTransform.localPosition += new Vector3(d.x, d.y, 0);
    }
}
