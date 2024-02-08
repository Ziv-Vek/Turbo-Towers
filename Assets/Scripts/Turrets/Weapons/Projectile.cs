using System;
using TurboTowers.Turrets.Common;
using UnityEngine;

namespace TurboTowers.Turrets.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float firePowerMultiplier = 2f;
        [SerializeField] private float lifeTime = 5f;
        [SerializeField] private int baseDamage = 1;
    
        private Health attacker;
        
        public Action<int> OnHit;
        
        private bool isHit = false;
        
        public float FirePowerMultiplier
        {
            get { return firePowerMultiplier; }
        }
        
        public void Fire(Vector3 fireVector, float firePowerMagnitude, Health attacker, Action<int> onHit)
        {
            LifeTimeHandler();
            this.attacker = attacker;
            OnHit = onHit;
            GetComponent<Rigidbody>().AddForce(fireVector * (firePowerMagnitude * firePowerMultiplier), ForceMode.VelocityChange);
        }
    
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player")) return;
            
            if (isHit) return;
            isHit = true;

            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().detectCollisions = false;
    
            if (other.gameObject.TryGetComponent(out IDamageable target))
            {
                if (attacker != null)
                {
                    target.HandleHit(baseDamage, OnHit);
                }
            }
    
            if (TryGetComponent<DecalPainter>(out var decalPainter))
            {
                decalPainter.PaintDecal(other.GetContact(0).point);
                Destroy(gameObject);
            }
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Portal>() != null && attacker.TryGetComponent(out TeleportationController teleportationController))
            {
                teleportationController.Teleport(other.GetComponent<Portal>());
            }
            
            Destroy(gameObject);
        }
    
        private void LifeTimeHandler()
        {
            Destroy(gameObject, lifeTime);
        }
    }
}

