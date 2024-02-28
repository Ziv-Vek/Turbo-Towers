using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using JetBrains.Annotations;
using TurboTowers.Core;
using TurboTowers.Map;
using TurboTowers.Map.Models;
using TurboTowers.Turrets.Common;
using UnityEngine;
using Action = System.Action;

public class BossController : MonoBehaviour, IMapManager
{
    [SerializeField] private PointType type;
    
    private Health health;

    public event Action OnBossBegin;
    
    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDeath += OnDeathHandler;
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        health.OnDeath -= OnDeathHandler;
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Start()
    {
        RegisterTower(health, type, transform.position, null);
        health.SetInvinsible(true);
    }
    
    private void OnGameStateChanged(GameState newGameState)
    {
        if (newGameState == GameState.InGameBoss)
        {
            GetComponent<BehaviorTree>().EnableBehavior();
            OnBossBegin?.Invoke();
            health.SetInvinsible(false);
        }
        else
        {
            GetComponent<BehaviorTree>().DisableBehavior();
        }
    }
        
    public void RegisterTower(Health health,
        PointType type,
        Vector3 basePosition,
        [CanBeNull] List<BodyPart> bodyParts)
    {
        if (MapManager.Instance != null)
            MapManager.Instance.RegisterTower(health, type, basePosition, null);
    }

    public void RegisterPoint(ITargetable targetableHealth, PointType type, Vector3 basePosition, List<IDamageable> damageableParts)
    {
        /*if (MapManager.Instance != null)
            MapManager.Instance.RegisterPoint(targetableHealth, type, basePosition, damageableParts);*/
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
        //
    }

    public void AddDamageablePartToPoint(Health target, BodyPart bodyPart)
    {
        //
    }
    
    private void OnDeathHandler()
    {
        UnRegisterPoint(health);
        GetComponent<BehaviorTree>().DisableBehavior();
        GameManager.Instance.SetGameState(GameState.Victory);
    }
}
