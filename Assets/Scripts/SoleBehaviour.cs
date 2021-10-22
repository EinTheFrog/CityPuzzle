using System;
using UnityEngine;

public class SoleBehaviour : MonoBehaviour
{
    [SerializeField] private float highlightingRatio = default;

    public bool IsOccupied { get; private set; } = false;
    public bool UnderHouse { get; set; } = false;
    
    private Material _material = default;
    private Color _basicColor = default;
    private BuildingManagerBehaviour _buildingManager = default;
    private bool _underMouse = false;
    private void Start()
    {
        _material = gameObject.GetComponent<Renderer>().material;
        _basicColor = _material.color;
        _buildingManager = FindObjectOfType<BuildingManagerBehaviour>();
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
        if (UnderHouse)
        {
            Highlight();
        }
    }
    
    private void Highlight()
    {
        if (_underMouse) return;
        _material.color = _basicColor + new Color(highlightingRatio, highlightingRatio, highlightingRatio, 0);
    }

    public void RestoreColor()
    {
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

    public void Occupy()
    {
        IsOccupied = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        UnderHouse = true;
        Highlight();
    }

    private void OnTriggerExit(Collider other)
    {
        UnderHouse = false;
        RestoreColor();
    }
}
