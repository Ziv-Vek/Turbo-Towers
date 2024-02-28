using System;
using TurboTowers.Map.Models;
using TurboTowers.Movement;
using TurboTowers.Turrets.Common;
using UnityEngine;

namespace TurboTowers.Turrets.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float firePowerMultiplier = 2f;
        [SerializeField] private float lifeTime = 5f;
        [SerializeField] private int baseDamage = 1;
        [SerializeField] private ParticleSystem groundHitEffect;
        
        private Health attacker;

        public Action<int, PointType> OnHit;

        private bool isHit = false;
        private int onlyOnce = 0;

        public float FirePowerMultiplier
        {
            get { return firePowerMultiplier; }
        }

        public void Fire(Vector3 fireVector, float firePowerMagnitude, Health attacker, Action<int, PointType> onHit)
        {
            LifeTimeHandler();
            this.attacker = attacker;
            OnHit = onHit;
            GetComponent<Rigidbody>().AddForce(fireVector * (firePowerMagnitude * firePowerMultiplier),
                ForceMode.VelocityChange);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player")) return;

            if (onlyOnce == 0)
            {
                onlyOnce++;

                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<Rigidbody>().detectCollisions = false;

                if (other.gameObject.TryGetComponent(out IDamageable target))
                {
                    if (attacker != null)
                    {
                        target.HandleHit(baseDamage, OnHit);
                    }
                }

                if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    if (groundHitEffect != null)
                    {
                        var hitPos = other.GetContact(0).point;
                        groundHitEffect.transform.position = hitPos;
                        groundHitEffect.Play();
                        GetComponent<DecalPainter>().PaintDecal(hitPos);
                        
                    }
                }
                
                HideProjectile();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (onlyOnce == 0)
            {
                onlyOnce++;

                if (other.GetComponent<Portal>() != null &&
                    attacker.TryGetComponent(out TeleportationController teleportationController))
                {
                    HideProjectile();
                    StartCoroutine(teleportationController.Teleport(other.GetComponent<Portal>(), false));
                }

                HideProjectile();
            }
        }

        private void LifeTimeHandler()
        {
            Destroy(gameObject, lifeTime);
        }

        private void HideProjectile()
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
    }
}