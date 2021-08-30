using System;
using UnityEngine;

public class SoleBehaviour : MonoBehaviour
{
    [SerializeField] private float highlightingRatio = default;
    
    private Material _material = default;
    private Color _basicColor = default;
    private void Start()
    {
        _material = gameObject.GetComponent<Renderer>().material;
        _basicColor = _material.color;
    }

    private void OnMouseEnter()
    {
        _material.color = _basicColor + new Color(highlightingRatio, highlightingRatio, highlightingRatio, 0);
    }
    
    private void OnMouseExit()
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
}
