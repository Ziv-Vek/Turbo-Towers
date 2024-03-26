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

        [Tooltip("For development purposes only. When true, the character does not lose health.")]
        [SerializeField] private bool isInvinsible = false;

        [SerializeField] private PointType myType;
        private HpBanner hpBanner;
        
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
            
            if (isInvinsible)
            {
                UnityEngine.Debug.Log("Invinsible mode is on for " + gameObject.name + "!");
            }

            hpBanner = gameObject.GetComponentInChildren<HpBanner>();
            UpdateHpBanner();
        }

        private void UpdateHpBanner()
        {
            if (hpBanner != null)
            {
                hpBanner.SetCurrentHealth(currentHealth);
            }
        }

        public int GetInitialHealth()
        {
            return baseHealth;
        }

        public void TakeDamage(int damage, BodyPart bodyPart)
        {
            if (isInvinsible) return;
            
            OnHealthLoss?.Invoke();
            currentHealth -= damage;
            
            UpdateHpBanner();
        
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
            
            UpdateHpBanner();
            
            
            //TODO: to do test
        }
        
        public PointType GetPointType()
        {
            return myType;
        }

        private void Die()
        {
            isAlive = false;
            
            if (MapManager.Instance) MapManager.Instance.UnRegisterPoint(this);
            
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
        
        public void SetInvinsible(bool isInvinsible)
        {
            this.isInvinsible = isInvinsible;
        }
    }
}

