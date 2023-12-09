using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, ITargetable
{
    [SerializeField] private Material targetedMaterial;
    private Material baseMaterial;
    private bool isOccupied = false;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Teleportation teleportation))
        {
            teleportation.Teleport(transform.position);
            DeactivatePortal();
        }
    }

    private void ActivatePortal()
    {
        
    }
    
    private void DeactivatePortal()
    {
        
    }
}
