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
        this.GetComponent<Rigidbody>().AddForce(fireVector * (firePowerMagnitude * firePowerMultiplier), ForceMode.VelocityChange);
    }
}
