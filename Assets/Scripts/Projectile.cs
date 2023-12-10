using System;
using JetBrains.Annotations;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float firePowerMultiplier = 2f;

    private TeleportationController attacker;
    
    public float FirePowerMultiplier
    {
        get { return firePowerMultiplier; }
    }
    
    public void Fire(Vector3 fireVector, float firePowerMagnitude, TeleportationController attacker)
    {
        this.attacker = attacker;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Portal>() != null)
        {
            attacker.Teleport(other.GetComponent<Portal>());
        }

        Destroy(gameObject);
    }

}
