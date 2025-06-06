using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;

   public void TakeDamage(float amount)
{
    health -= amount;
    Debug.Log(gameObject.name + " Health: " + health);

    if (health <= 0f)
    {
        Die();
    }
}


    private void Die()
    {
        Destroy(gameObject);
    }
    
}
