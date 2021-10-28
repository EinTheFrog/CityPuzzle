using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour
{
    [SerializeField] private string name = default;
    [SerializeField] private StringInt[] goldForBuildings = default;
    public delegate void WorkForManager(BuildingBehaviour building);

    public WorkForManager underBuildingEvent;
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
            localPosition.y,
            Mathf.Round(localPosition.z)
        );
        transform.localPosition = localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<BuildingBehaviour>() != null)
        {
            underBuildingEvent(this);
        }
    }
}