using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] private int baseHealth = 1;
    [SerializeField] private int currentHealth = 1;

    private void Start()
    {
        currentHealth = baseHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Taking damage: " + damage);
    }
}
