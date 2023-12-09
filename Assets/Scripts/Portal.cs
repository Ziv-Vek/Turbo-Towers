using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, ITargetable
{
    [SerializeField] private Material targetedMaterial;
    private Material baseMaterial;

    void Start()
    {
        baseMaterial = GetComponent<Renderer>().material;
    }

    public void PaintTargeted()
    {
        GetComponent<Renderer>().material = targetedMaterial;
    }

    public void UnPaintTargeted()
    {
        GetComponent<Renderer>().material = baseMaterial;
    }
    
    public void PerformAction(Teleport attacker)
    {
        if (attacker != null)
        {
            attacker.TeleporHandler(attacker, this);
            DeactivatePortal();
        }
    }

    public void ActivatePortal()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
    }

    public void DeactivatePortal()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
    }
}
