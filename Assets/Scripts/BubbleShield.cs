using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public GameObject shieldObject;   // Drag your shield bubble here
    public float shieldDuration = 5f; // Duration shield stays active
    public float cooldownTime = 10f;  // Cooldown before you can activate again

    private bool isShieldActive = false;
    private bool isOnCooldown = false;

    public bool IsShieldActive
    {
        get { return isShieldActive; }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isShieldActive && !isOnCooldown)
        {
            ActivateShield();
        }
    }

    void ActivateShield()
    {
        shieldObject.SetActive(true);
        isShieldActive = true;
        isOnCooldown = true;
         Debug.Log("Shield activated!");
        Invoke("DeactivateShield", shieldDuration);      // Turn off shield after duration
        Invoke("ResetCooldown", cooldownTime);           // Reset cooldown after cooldownTime
    }



    void DeactivateShield()
    {
        shieldObject.SetActive(false);
        isShieldActive = false;
    }

    void ResetCooldown()
    {
        isOnCooldown = false;
    }
}
