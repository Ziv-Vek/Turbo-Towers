using System;
using JetBrains.Annotations;
using Sirenix.Utilities;
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
        [SerializeField] private Rigidbody rb;
        
        private Health attacker;
        private GameObject parentColliderGO;    // the nearest parent with the collider to be aware of when the projectile is fired

        public Action<int, PointType, BodyPartType?> OnHit;

        private bool isHit = false;
        private int onlyOnce = 0;

        private bool isGhost;

        public void SimulateFire(Vector3 velocity, bool isGhost)
        {
            this.isGhost = isGhost;
            
            rb.AddForce(velocity, ForceMode.Impulse);
        }
        
        public float FirePowerMultiplier
        {
            get { return firePowerMultiplier; }
        }

        public void Fire(Vector3 fireVector, float firePowerMagnitude, Health attacker, Action<int, PointType, BodyPartType?> onHit, [CanBeNull] GameObject parentColliderGO)
        {
            if (parentColliderGO)
                this.parentColliderGO = parentColliderGO;
            LifeTimeHandler();
            this.attacker = attacker;
            OnHit = onHit;
            GetComponent<Rigidbody>().AddForce(fireVector * (firePowerMagnitude * firePowerMultiplier),
                ForceMode.VelocityChange);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (isGhost) return;
            if (parentColliderGO != null && other.gameObject == parentColliderGO) return;
            
            if (other.gameObject.CompareTag("Player")) return;

            if (onlyOnce == 0)
            {
                onlyOnce++;

                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<Rigidbody>().detectCollisions = false;

                if (other.gameObject.TryGetComponent(out IDamageable target))
                {
                        Debug.Log("play");
                        Debug.Log(attacker.gameObject.name);
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
            if (isGhost) return;
            
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
            GetComponentsInChildren<MeshRenderer>().ForEach((renderer) =>
            {
                renderer.enabled = false;
            });
            /*if (TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
                renderer.enabled = false;*/
            if (TryGetComponent<Collider>(out Collider collider))
                collider.enabled = false;
        }
    }
}