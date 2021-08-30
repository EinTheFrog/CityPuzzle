using UnityEngine;

public class HouseBehaviour : MonoBehaviour
{
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
}
