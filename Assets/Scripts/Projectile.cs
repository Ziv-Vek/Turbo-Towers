using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float firePowerMultiplier = 2f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private int baseDamage = 1;

    private TeleportationController attacker;
    
    public float FirePowerMultiplier
    {
        get { return firePowerMultiplier; }
    }
    
    public void Fire(Vector3 fireVector, float firePowerMagnitude, TeleportationController attacker)
    {
        LifeTimeHandler();
        this.attacker = attacker;
        GetComponent<Rigidbody>().AddForce(fireVector * (firePowerMagnitude * firePowerMultiplier), ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) return;

        if (other.gameObject.TryGetComponent(out HealthHandler healthHandler))
        {
            healthHandler.TakeDamage(baseDamage);
        }

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

    private void LifeTimeHandler()
    {
        Destroy(gameObject, lifeTime);
    }
}
