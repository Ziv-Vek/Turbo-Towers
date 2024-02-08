using System;
using TurboTowers.Map;
using TurboTowers.Map.Models;
using UnityEngine;
using UnityEngine.Events;

public class Portal : MonoBehaviour, ITargetable
{
    public UnityEvent OnReachedPortal; 
    
    public void ActivatePortal()
    {
        GetComponent<Collider>().enabled = true;
        GetComponentInChildren<Renderer>().enabled = true;
    }

    public void DeactivatePortal()
    {
        GetComponent<Collider>().enabled = false;
        GetComponentInChildren<Renderer>().enabled = false;
        
        OnReachedPortal?.Invoke();
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