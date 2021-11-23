using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour
{
    [SerializeField] private string buildingName = default;
    [SerializeField] private StringInt[] goldForBuildings = default;

    public string BuildingName => buildingName;
    public StringInt[] GoldForBuildings => goldForBuildings;
    public bool UnderGhost { get; private set; } = false;

    private SpriteRenderer _sprite;
    private Transform _cameraTransform;

    public void OnDrawGizmos()
    {
        AlignPosition();
    }

    public void Initialize()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _sprite.enabled = false;
        GetComponentInChildren<BoxCollider>().enabled = false;
        _cameraTransform = FindObjectOfType<Camera>().transform;
    }

    private void AlignPosition()
    {
        if (!transform.hasChanged) return;
        var localPosition = transform.localPosition;
        localPosition = new Vector3(
            Mathf.Round(localPosition.x),
            localPosition.y,
            Mathf.Round(localPosition.z)
        );
        transform.localPosition = localPosition;
    }

    public void Highlight(bool b, string ghostType)
    {
        var spriteTrans = _sprite.transform;
        spriteTrans.LookAt(_cameraTransform);
        spriteTrans.eulerAngles = RoundRotation(spriteTrans.eulerAngles);
        
        var haveProfit = CheckGhostTypeInList(ghostType);
        _sprite.enabled = b & haveProfit;

        UnderGhost = b;
    }

    private bool CheckGhostTypeInList(string ghostType)
    {
        foreach (var pair in goldForBuildings)
        {
            if (pair.key == ghostType)
            {
                return true;
            }
        }

        return false;
    }

    private Vector3 RoundRotation(Vector3 oldRot)
    {
        var y = oldRot.y;
        var z = oldRot.z;
        var newY = Mathf.Round(y / 90) * 90;
        var newZ = Mathf.Round(z / 90) * 90;
        return new Vector3(oldRot.x, newY, newZ);
    }
}