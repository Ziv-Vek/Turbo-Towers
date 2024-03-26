using System;
using System.Collections;
using TurboTowers.Map.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TurboTowers.Turrets.Common;
using TurboTowers.Turrets.Controls;

namespace TurboTowers.Turrets.Combat
{
    [RequireComponent(typeof(Health))]
    public class PlayerAttacker : MonoBehaviour
    {
        // Cached refs:
        [SerializeField] private HoldClickableButton powerBtn;
        [SerializeField] private Slider powerSlider;
        [SerializeField] private float powerupSpeed = 0.1f;
        [SerializeField] private float powerdownSpeed = 0.3f;
        [SerializeField] private Transform turretExit;
        [SerializeField] private Projectile projectile;
        [SerializeField] private TrajectoryLine trajectoryLine;
        [SerializeField] private float cooldownTime = 1f;
        [Tooltip("For third input style")]
        [SerializeField] private float shootingVelocity = 10f;

        private bool isPoweringUp = false;
        private bool isAvailableToFire = true;
        private Scene testScene;
        private Health health;
        private bool isCooledDown;
        private float cooldownCounter = 0;
        private GameObject parentColliderGO;    // the nearest parent with the collider to be aware of when the projectile is fired

        // private InputSwitchHandler inputSwitchHandler;
        private InputStyle inputStyle = InputStyle.First;

        #region Events

        public event Action OnTurretPowering;
        public event Action OnTurretFired;
        public event Action OnShootingEnabled; 

        public Action<int, PointType, BodyPartType?> OnTurretHit;

        #endregion

        #region Unity Events

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            // powerBtn.OnShortClicked += PowerUpStartHandler;
            powerBtn.OnHoldClickMaxed += Fire;
            powerBtn.OnClickUp += PowerClickedUpHandler;
            powerBtn.OnClickDown += PowerClickedDownHandler;

            StartCoroutine(HandleInputListeners());
        }

        private IEnumerator HandleInputListeners()
        {
            if (!InputSwitchHandler.Instance) yield return null;

            InputSwitchHandler.Instance.OnInputStyleSelect += SetInputStyle;
        }

        private void OnDisable()
        {
            // powerBtn.OnShortClicked -= PowerUpStartHandler;
            powerBtn.OnHoldClickMaxed -= Fire;
            powerBtn.OnClickUp -= PowerClickedUpHandler;
            powerBtn.OnClickDown -= PowerClickedDownHandler;

            InputSwitchHandler.Instance.OnInputStyleSelect -= SetInputStyle;
        }

        private void Start()
        {
            inputStyle = GetComponent<PlayerController>().inputStyle;
            parentColliderGO = GetComponentInParent<Collider>()?.gameObject;
            powerSlider.maxValue = powerBtn.HoldDuration;
            OnTurretHit = HandleHit;
            isCooledDown = true;
            OnShootingEnabled?.Invoke();
        }

        private void Update()
        {
            if (inputStyle == InputStyle.Second) return;

            if (isCooledDown && isPoweringUp)
            {
                PowerUpHandler();
            }
        }

        #endregion

        private void SetInputStyle(InputStyle inputStyle)
        {
            this.inputStyle = inputStyle;
        }

        #region Powering Up and Firing

        private void PowerUpStartHandler()
        {
            isPoweringUp = true;
        }

        private void PowerUpHandler()
        {
            if (powerBtn.ElapsedTime < powerBtn.MinPressDuration)
                return;

            powerSlider.value += Time.deltaTime * powerupSpeed;
            trajectoryLine.ShowTrajectoryLine(turretExit.position,
                turretExit.forward * ((powerSlider.value / powerSlider.maxValue) * projectile.FirePowerMultiplier));
            if (powerSlider.value >= powerSlider.maxValue)
                Fire();
        }

        private void PowerClickedUpHandler()
        {
            if (!isCooledDown) return;

            if (powerBtn.ElapsedTime < powerBtn.MinPressDuration)
            {
                isPoweringUp = false;
                powerSlider.value = 0;
                return;
            }

            Fire();
        }

        private void PowerClickedDownHandler()
        {
            OnTurretPowering?.Invoke();
            isPoweringUp = true;
        }

        public void Fire()
        {
            if (!isCooledDown) return;
            
            StartCoroutine(HandleCooldown());
            if (inputStyle != InputStyle.Fourth)
            {
                isPoweringUp = false;
                cooldownCounter = 0;

                trajectoryLine.RemoveTrajectoryLine();

                var projectile =
                    Instantiate(this.projectile, turretExit.transform.position,
                        Quaternion.identity);

                projectile.Fire(turretExit.forward,
                    powerSlider.value / powerSlider.maxValue,
                    GetComponent<Health>(), OnTurretHit, parentColliderGO);
                powerSlider.value = 0;

                OnTurretFired?.Invoke();    
            }
            else if (inputStyle == InputStyle.Fourth)
            {
                if (isCooledDown) return;
                StartCoroutine(HandleCooldown());
                var projectile =
                    Instantiate(this.projectile, turretExit.transform.position,
                        Quaternion.identity);

                projectile.Fire(turretExit.forward,
                    shootingVelocity,
                    GetComponent<Health>(), OnTurretHit, parentColliderGO);
                OnTurretFired?.Invoke();
            }
        }

        IEnumerator HandleCooldown()
        {
            isCooledDown = false;
            float counter = 0;
            
            while (counter < cooldownTime)
            {
                counter += Time.deltaTime;
                yield return null;
            }
            
            isCooledDown = true;
            
            OnShootingEnabled?.Invoke();
        }
        
        private void HandleHit(int damagePerformed, PointType hittedType, BodyPartType? bodyPartType)
        {
            switch (hittedType)
            {
                case PointType.Enemy:
                    health.GainHealth(damagePerformed);
                    break;
            }
        }
        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(turretExit.position, 55f);
            Gizmos.DrawLine(turretExit.position, turretExit.position + turretExit.up * 55f);
        }

        #endregion

        #region Getter Methods

        public Projectile GetProjectile()
        {
            return projectile;
        }
        
        public Vector3 GetShootingVelocity()
        {
            return turretExit.forward * shootingVelocity;
        }
        
        public float GetShootingVelocityMagnitude()
        {
            return shootingVelocity;
        }
        
        public Vector3 GetTurretExitPosition()
        {
            return turretExit.position;
        }
        
        public float GetCooldownTime()
        {
            return cooldownTime;
        }

        #endregion
    }
}