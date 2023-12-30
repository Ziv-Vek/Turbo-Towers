using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int baseHealth = 1;
    [SerializeField] private int currentHealth = 1;

    public event Action<int> onDamageTaken;
    public event Action<int> onHealthGained;

    private void Start()
    {
        currentHealth = baseHealth;
    }
    
    public int GetInitialHealth()
    {
        return baseHealth;
    }

    public void TakeDamage(int damage)
    {
        onDamageTaken?.Invoke(Mathf.Min(currentHealth, damage));
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
            return;
        }
    }
    
    public void GainHealth(int health)
    {
        currentHealth += health;
        
        onHealthGained?.Invoke(health);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died");
        Destroy(gameObject);
    }
}
