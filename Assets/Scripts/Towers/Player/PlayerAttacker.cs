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
        [SerializeField] private HoldClickableButton powerBtn;

        [SerializeField] private Slider powerSlider;
        [SerializeField] private float powerupSpeed = 0.1f;
        [SerializeField] private float powerdownSpeed = 0.3f;
        [SerializeField] private Transform turretExit;
        [SerializeField] private Projectile projectile;
        [SerializeField] private TrajectoryLine trajectoryLine;
        [SerializeField] private float cooldownTime = 1f;

        private bool isPoweringUp = false;
        private bool isAvailableToFire = true;
        private Scene testScene;
        private Health health;
        private float cooldownCounter = 0;

        // private InputSwitchHandler inputSwitchHandler;
        private InputStyle inputStyle = InputStyle.First;

        #region Events

        public event Action OnTurretPowering;
        public event Action OnTurretFired;

        public Action<int, PointType> OnTurretHit;

        #endregion

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
            testScene = SceneManager.CreateScene("TestScene");
            powerSlider.maxValue = powerBtn.HoldDuration;
            OnTurretHit = HandleHit;
            cooldownCounter = 0;
        }

        private void Update()
        {
            if (inputStyle != InputStyle.First) return;

            cooldownCounter += Time.deltaTime;

            if (isAvailableToFire && isPoweringUp)
            {
                PowerUpHandler();
            }
        }

        private void SetInputStyle(InputStyle inputStyle)
        {
            this.inputStyle = inputStyle;
        }

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
            if (cooldownCounter < cooldownTime) return;

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

        private void Fire()
        {
            isPoweringUp = false;
            cooldownCounter = 0;

            trajectoryLine.RemoveTrajectoryLine();

            var projectile =
                Instantiate(this.projectile, turretExit.transform.position,
                    Quaternion.identity);

            projectile.Fire(turretExit.forward,
                powerSlider.value / powerSlider.maxValue,
                GetComponent<Health>(), OnTurretHit);
            powerSlider.value = 0;

            OnTurretFired?.Invoke();
        }

        private void HandleHit(int damagePerformed, PointType hittedType)
        {
            switch (hittedType)
            {
                case PointType.Enemy:
                    health.GainHealth(damagePerformed);
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(turretExit.position, 55f);
            Gizmos.DrawLine(turretExit.position, turretExit.position + turretExit.up * 55f);
        }
    }
}