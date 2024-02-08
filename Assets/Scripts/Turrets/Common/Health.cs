using System;
using System.Collections;
using System.Collections.Generic;
using TurboTowers.Map;
using TurboTowers.Map.Models;
using UnityEngine;
using UnityEngine.Events;

namespace TurboTowers.Turrets.Common
{
    public class Health : MonoBehaviour, ITargetable
    {
        //Config:
        [SerializeField] private float dissolveTime = 3f;
        [SerializeField] private int baseHealth = 1;
        [SerializeField] private int currentHealth = 1;

        //State:
        private bool isAlive = true;
        
        #region Events

        [SerializeField] private UnityEvent OnHealthGain;
        [SerializeField] private UnityEvent OnHealthLoss;
        [SerializeField] private UnityEvent OnTotalDeath;
        public event Action<int, BodyPart> onDamageTaken;
        public event Action<int> OnHit;
        public event Action<int> onHealthGained;
        public event Action OnDeath;
        #endregion
        
        private void Start()
        {
            currentHealth = baseHealth;
        }
    
        public int GetInitialHealth()
        {
            return baseHealth;
        }

        public void TakeDamage(int damage, BodyPart bodyPart)
        {
            OnHealthLoss?.Invoke();
            currentHealth -= damage;
        
            if (currentHealth <= 0)
            {
                Die();
                return;
            }
            
            OnHit?.Invoke(currentHealth);
            
            onDamageTaken?.Invoke(Mathf.Min(currentHealth, damage), bodyPart);
        }
    
        public void GainHealth(int health)
        {
            currentHealth += health;
        
            OnHealthGain?.Invoke();
            onHealthGained?.Invoke(health);
        }

        private void Die()
        {
            isAlive = false;
            OnTotalDeath?.Invoke();
            OnDeath?.Invoke();
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }
        
        public bool IsAlive()
        {
            return isAlive;
        }
    }
}

