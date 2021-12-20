using System;
using UnityEngine;

public class SoleBehaviour : MonoBehaviour
{
    [SerializeField] private float highlightingRatio = default;

    public bool IsOccupied => _building != null;
    public bool UnderGhost { get; set; } = false;
    public BuildingBehaviour Building => _building;
    
    public int Id { get; private set; }
    
    private Material _material = default;
    private Color _basicColor = default;
    private BuildingManagerBehaviour _buildingManager = default;
    private bool _underMouse = false;
    private BuildingBehaviour _building = null;
    private BuildingType _ghostType = BuildingType.EMPTY;
    private void Start()
    {
        _material = gameObject.GetComponent<Renderer>().material;
        _basicColor = _material.color;
        _buildingManager = FindObjectOfType<BuildingManagerBehaviour>();
        var pos = transform.localPosition;
        Id = ((int)pos.x) * 10 + (int)pos.z;
    }

    private void OnMouseEnter()
    {
        _underMouse = true;
        RestoreColor();
        _buildingManager.SelectSole(this);
    }

    private void OnMouseExit()
    {
        _underMouse = false;
        if (UnderGhost)
        {
            Highlight();
        }
    }
    
    private void Highlight()
    {
        if (_underMouse) return;
        _material.color = _basicColor + new Color(highlightingRatio, highlightingRatio, highlightingRatio, 0);
        if (IsOccupied)
        {
            _building.Highlight(true, _ghostType);
        }
    }

    public void RestoreColor()
    {
        if (IsOccupied)
        {
            _building.Highlight(false, BuildingType.EMPTY);
        }
        _material.color = _basicColor;
    }

    public void OnDrawGizmos()
    {
        AlignPosition();
    }
    
    private void AlignPosition()
    {
        if (!transform.hasChanged) return;
        var localPosition = transform.localPosition;
        localPosition = new Vector3(
            Mathf.Round(localPosition.x),
            Mathf.Round(localPosition.y),
            Mathf.Round(localPosition.z)
        );
        transform.localPosition = localPosition;
    }

    public void Occupy(BuildingBehaviour building)
    {
        _building = building;
    }

    public void Free()
    {
        _building = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        var building = other.GetComponentInParent<BuildingBehaviour>();
        if (building == null) return;
        UnderGhost = true;
        _ghostType = building.BuildingType;
        Highlight();
    }

    private void OnTriggerExit(Collider other)
    {
        UnderGhost = false;
        _ghostType = BuildingType.EMPTY;
        RestoreColor();
    }
}
