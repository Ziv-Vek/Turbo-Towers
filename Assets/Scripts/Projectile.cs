using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float firePowerMultiplier = 2f;
    public float FirePowerMultiplier
    {
        get { return firePowerMultiplier; }
    }
    
    public void Fire(Vector3 fireVector, float firePowerMagnitude)
    {
        GetComponent<Rigidbody>().AddForce(fireVector * (firePowerMagnitude * firePowerMultiplier), ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) return;
        if (TryGetComponent<DecalPainter>(out var decalPainter))
        {
            decalPainter.PaintDecal(other.GetContact(0).point);
            Destroy(gameObject);
        }
        
    }
    
    
}
