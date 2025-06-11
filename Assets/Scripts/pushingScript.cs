using UnityEngine;

public class PlayerPushTrigger : MonoBehaviour
{
    public float pushForce = 5f;
    private Rigidbody objectToPush;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("push"))
        {
            objectToPush = other.attachedRigidbody;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == objectToPush)
        {
            objectToPush = null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && objectToPush != null)
        {
            objectToPush.AddForce(transform.forward * pushForce, ForceMode.Impulse);
            Debug.Log("Pushed " + objectToPush.name);
        }
    }
}

