using System;
using JetBrains.Annotations;
using TurboTowers.Core;
using TurboTowers.Map;
using TurboTowers.Map.Models;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace TurboTowers.Movement
{
    public class Portal : MonoBehaviour, ITargetable
    {
        public UnityEvent<GameState> OnReachedPortal;

        [Tooltip("Initialize this next state when player reaches the portal")]
        public GameState nextState;

        public void ActivatePortal()
        {
            GetComponent<Collider>().enabled = true;
            GetComponentInChildren<Renderer>().enabled = true;
        }

        public void DeactivatePortal()
        {
            GetComponent<Collider>().enabled = false;
            GetComponentInChildren<Renderer>().enabled = false;

            if (nextState != null && OnReachedPortal != null)
                OnReachedPortal.Invoke(nextState);
        }

        private void OnDisable()
        {
            if (OnReachedPortal != null)
            {
                OnReachedPortal = null;
            }
        }

        public int GetCurrentHealth()
        {
            return 1;
        }

        public bool IsAlive()
        {
            return true;
        }
    }
}