using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using DG.Tweening;
using TurboTowers.Map;
using TurboTowers.Map.Models;
using TurboTowers.Helpers;

namespace TurboTowers.Turrets.Common
{
    public class GrowthHandler : MonoBehaviour, IMapManager
    {
        #region Variales

        // Cached ref:
        [SerializeField] Transform headTransform;
        [SerializeField] Transform baseTransform;
        [SerializeField] GameObject bodyPartPrefab;
        Transform bodyPartsParent;
        Health health;

        // State:
        [SerializeField] private PointType type;
        
        private List<GameObject> bodyPartsObj;

        // Config:
        [Header("Config:")]
        [Tooltip("Dissolve time of all turret's parts when the turret dies")]
        [SerializeField] private float dissolveTimeOnDeath = 3f;
        [Tooltip("Dissolve time of each body part when it's hit")]
        [SerializeField] private float dissolveTimePerBodyPart = 1f;
        [SerializeField] private float bodyPartDropTime = 0.5f;
        [SerializeField] private float headRiseTimePerBodyPart = 0.5f;
        [SerializeField] private float delayBetweenBodyPartsSpawn = 0.3f;
        
        const float GAP_BETWEEN_BODY_PARTS = 0.005f;
        
        #endregion

        #region Unity Events

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void Start()
        {
            bodyPartsObj = new List<GameObject>(10);
            var bodyParts = SpawnBodyParts();
            RegisterTower(health, type, baseTransform.position, bodyParts);
            RegisterPoint(health.GetComponent<ITargetable>(), type, baseTransform.position, bodyParts.ConvertAll((part) => GetComponent<IDamageable>()));
        }
        
        private void OnEnable()
        {
            health.onDamageTaken += Shrink;
            health.onHealthGained += Grow;
            health.OnDeath += OnDeathHandler;
        }

        private void OnDisable()
        {
            health.onDamageTaken -= Shrink;
            health.onHealthGained -= Grow;
            health.OnDeath -= OnDeathHandler;
        }

        #endregion
        

        #region Private Methods

        private List<BodyPart> SpawnBodyParts()
        {
            var initialHealth = health.GetInitialHealth();

            if (initialHealth < 1)
            {
                Debug.Log("health of " + gameObject.name + " is " + initialHealth + ". /n Need to be greater than 1");
                return null;
            }
            
            List<BodyPart> bodyParts = new List<BodyPart>(initialHealth);
            
            bodyPartsObj.Add(baseTransform.gameObject);
            bodyPartsObj.Add(headTransform.gameObject);
            InstantiateBodyPartsParent();

            var bodyPartHeight = bodyPartPrefab.GetComponent<BodyPart>().GetMeshBoundsHeight();

            for (int i = 1; i < initialHealth ; i++)
            {
                var newFloor = CreateFloor();
                bodyPartsObj.Insert(bodyPartsObj.Count - 1, newFloor);
                RaiseHead();
            }
            
            bodyPartsObj.ForEach((item) => bodyParts.Add(item.GetComponent<BodyPart>()));

            return bodyParts;
        }

        private void InstantiateBodyPartsParent()
        {
            bodyPartsParent = new GameObject("BodyParts").transform;
            bodyPartsParent.parent = transform;
            bodyPartsParent.localPosition = Vector3.zero;
            bodyPartsParent.localRotation = Quaternion.identity;
        }
        
        private void Grow(int hpGained)
        {
            if (hpGained < 1)
            {
                Debug.Log("health received is " + hpGained + ". Need to be greater than 1");
                return;
            }
            
            for (int i = hpGained ; i > 0; i--)
            {
                var newFloor = CreateFloor();
                
                bodyPartsObj.Insert(bodyPartsObj.Count - 1, newFloor);
                RaiseHead();
                
                AddDamageablePartToPoint(health, newFloor.GetComponent<BodyPart>());
            }
        }

        // Raises the head of the turret by one floor height
        private void RaiseHead()
        {
            Vector3 prevBodyPos = bodyPartsObj[bodyPartsObj.Count - 2].transform.position;
              headTransform.DOMoveY( prevBodyPos.y + bodyPartPrefab.GetComponent<BodyPart>().GetMeshBoundsHeight() + GAP_BETWEEN_BODY_PARTS * 2,
                headRiseTimePerBodyPart).SetEase(Ease.OutExpo);
        }

        // Creates a new floor and returns its GameObject
        private GameObject CreateFloor()
        {
            var newFloor = Instantiate(bodyPartPrefab, bodyPartsParent);
            var finalFloorScale = bodyPartPrefab.transform.localScale;
            newFloor.transform.localScale = Vector3.zero;

            if (bodyPartsObj.Count == 1)
            {
                var baseHighestYPos = baseTransform.GetComponent<MeshRenderer>() != null ? baseTransform.GetComponent<MeshRenderer>().bounds.max.y : baseTransform.GetComponentInChildren<MeshRenderer>().bounds.max.y;
                var yPos = baseHighestYPos + (bodyPartPrefab.GetComponent<BodyPart>().GetMeshBoundsHeight()) + GAP_BETWEEN_BODY_PARTS;
                newFloor.transform.position = new Vector3(baseTransform.position.x, yPos, baseTransform.position.z);
                newFloor.transform.rotation = baseTransform.rotation;

                return newFloor;
            }
            
            Vector3 prevBodyPos = bodyPartsObj[bodyPartsObj.Count - 2].transform.position;
            newFloor.transform.position = new Vector3(prevBodyPos.x,
                prevBodyPos.y + bodyPartPrefab.GetComponent<BodyPart>().GetMeshBoundsHeight() + GAP_BETWEEN_BODY_PARTS, prevBodyPos.z);
              
            newFloor.transform.DOScale(finalFloorScale,
                headRiseTimePerBodyPart).SetEase(Ease.OutExpo).SetDelay(delayBetweenBodyPartsSpawn);

            return newFloor;
        }

        private void Shrink(int damage, BodyPart hittedBodyPart)
        {
            if (hittedBodyPart == null)
            {
                Debug.Log("hittedBodyPart is null");
                return;
            }

            var bodyPartIndex = BodyPartIndex(hittedBodyPart);
            // hitted body part is the Head
            if (hittedBodyPart.GetBodyPartType() == BodyPartType.Head)
            {
                Debug.Log("Headshot!");
                hittedBodyPart = bodyPartsObj[bodyPartsObj.Count - 2].GetComponent<BodyPart>();
            }
            
            // hitted body part is the Base
            if (bodyPartIndex == 0)
            {
                hittedBodyPart = bodyPartsObj[1].GetComponent<BodyPart>();
            }
             
            RemoveDamageablePartFromPoint(health, hittedBodyPart);
            
            var bodyPartGO = hittedBodyPart.gameObject;
            
            if (bodyPartGO == baseTransform.gameObject)
            {
                StartCoroutine(ShrinkEffect(bodyPartsObj[1].GetComponent<BodyPart>()));
            }
            else if (bodyPartGO == headTransform.gameObject)
            {
                StartCoroutine(ShrinkEffect(bodyPartsObj[bodyPartsObj.Count - 2].GetComponent<BodyPart>()));
            }
            else
            {
                StartCoroutine(ShrinkEffect(hittedBodyPart));    
            }
        }

        private int BodyPartIndex(BodyPart hittedBodyPart)
        {
            return bodyPartsObj.FindIndex((part) => part == hittedBodyPart.gameObject);
        }

        IEnumerator ShrinkEffect(BodyPart hittedBodyPart)
        {
            yield return StartCoroutine(hittedBodyPart.Dissolve(dissolveTimePerBodyPart));
            
            var hittedBodyPartIndex = bodyPartsObj.IndexOf(hittedBodyPart.gameObject);
            bodyPartsObj.Remove(hittedBodyPart.gameObject);
            
            for (int i = hittedBodyPartIndex ; i < bodyPartsObj.Count; i++)
            {
                bodyPartsObj[i].transform.DOMoveY(bodyPartsObj[i].transform.position.y - hittedBodyPart.GetMeshBoundsHeight() - GAP_BETWEEN_BODY_PARTS, bodyPartDropTime).SetEase(Ease.OutExpo);
            }
            
            bodyPartsObj.Remove(hittedBodyPart.gameObject);
            Destroy(hittedBodyPart.gameObject);
        }
        
        private void OnDeathHandler()
        {
            StartCoroutine(ShrinkAllParts());
            UnRegisterPoint(health);
        }

        IEnumerator ShrinkAllParts()
        {
            for (int i = bodyPartsObj.Count - 1 ; i >= 0; i--)
            {
                StartCoroutine(bodyPartsObj[i].GetComponent<BodyPart>().Dissolve(dissolveTimeOnDeath));
                yield return new WaitForSeconds(0.2f);
            }

            gameObject.SetActive(false);
        }

        #endregion

        #region IMapManager

        public void RegisterPoint(ITargetable targetableHealth,
            PointType type,
            Vector3 basePosition,
            List<IDamageable> damageableParts)
        {
            if (MapManager.Instance != null)
                MapManager.Instance.RegisterPoint(targetableHealth, type, basePosition, damageableParts);
        }
        
        public void RegisterTower(Health health,
            PointType type,
            Vector3 basePosition,
            List<BodyPart> bodyParts)
        {
            if (MapManager.Instance != null)
                MapManager.Instance.RegisterTower(health, type, basePosition, bodyParts);
        }

        public void UnRegisterPoint(Health target)
        {
            if (MapManager.Instance != null)
            {
                MapManager.Instance.UnRegisterPoint(target);
            }
        }

        public void RemoveDamageablePartFromPoint(Health target, BodyPart bodyPart)
        {
            if (MapManager.Instance != null)
                MapManager.Instance.RemoveBodyPartFromPoint(health, bodyPart);
        }

        public void AddDamageablePartToPoint(Health target, BodyPart bodyPart)
        {
            if (MapManager.Instance != null)
                MapManager.Instance.AddDamageablePartToPoint(target, bodyPart);
        }

        #endregion
    }
}

