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

    private TextMesh _textMesh;
    private Transform _cameraTransform;

    public void OnDrawGizmos()
    {
        AlignPosition();
    }

    public void Initialize()
    {
        _textMesh = GetComponentInChildren<TextMesh>();
        _textMesh.text = "";
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
        var textTrans = _textMesh.transform;
        textTrans.LookAt(_cameraTransform);
        textTrans.Rotate(Vector3.up, 180);
        textTrans.eulerAngles = RoundRotation(textTrans.eulerAngles);
        
        var profit = CheckGhostTypeInList(ghostType);
        var hasProfit = profit != 0;
        _textMesh.text = (b & hasProfit) ? profit.ToString() : "";

        UnderGhost = b;
    }

    private int CheckGhostTypeInList(string ghostType)
    {
        foreach (var pair in goldForBuildings)
        {
            if (pair.key == ghostType)
            {
                return pair.value;
            }
        }

        return 0;
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