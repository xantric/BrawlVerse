using UnityEngine;

public class GrabbableItem : MonoBehaviour
{
    public bool isGrabbedByPlayer = false;

    public void OnGrabbed()
    {
        isGrabbedByPlayer = true;
        transform.parent = null;
        gameObject.tag = "HeldItem";
    }

    public void OnReleased()
    {
        isGrabbedByPlayer = false;
        gameObject.tag = "Item";
    }
}