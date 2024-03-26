using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmazingAssets.AdvancedDissolve;
using TurboTowers.Map.Models;

namespace TurboTowers.Turrets.Common
{
    public class BodyPart : MonoBehaviour, IDamageable
    {
        private Health health;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private BodyPartType bodyPartType;

        private void Start()
        {
            health = GetComponentInParent<Health>();
        }

        public Health GetTurretHealth()
        {
            return health;
        }
        
        public BodyPartType GetBodyPartType()
        {
            return bodyPartType;
        }
        
        public void HandleHit(int damage, Action<int, PointType, BodyPartType?> onHit)
        {
            Debug.Log("hit player");
            health.TakeDamage(damage, this);
            onHit?.Invoke(damage, health.GetPointType(), bodyPartType);
        }

        public Quaternion GetRotation()
        {
            return transform.rotation;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public float GetMeshBoundsHeight()
        {
            return meshRenderer.bounds.size.y;
        }
        
        public float GetMaxYBoundPosition()
        {
            return meshRenderer.bounds.max.y;
        }

        public Vector3 GetYBoundsWorldPoint()
        {
            var maxBound = meshRenderer.bounds.max;
            Matrix4x4 localToWorld = meshRenderer.transform.localToWorldMatrix;
            return localToWorld.MultiplyPoint3x4(maxBound);
        }

        public IEnumerator Dissolve(float dissolveTime)
        {
            var renderers = GetComponentsInChildren<MeshRenderer>();
            var materials = new List<Material>();
            foreach (MeshRenderer meshRenderer in renderers)
            {
                if (meshRenderer.material.shader.name == "Amazing Assets/Advanced Dissolve/Lit")
                {
                    materials.Add(meshRenderer.material);
                }
            }
            
            float elapsedTime = 0f;

            while (elapsedTime < dissolveTime)
            {
                elapsedTime += Time.deltaTime;

                foreach (var material in materials)
                {
                    AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(material,
                        AdvancedDissolveProperties.Cutout.Standard.Property.Clip,
                        Mathf.Lerp(0f, 1f, elapsedTime / dissolveTime));
                }

                yield return null;
            }

            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = false;
        } 
    }
   
}
