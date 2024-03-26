using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TurboTowers.Map.Models;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using TurboTowers.Turrets.Combat;
using TurboTowers.Turrets.Common;
using TurboTowers.Turrets.Controls;

namespace TurboTowers.Turrets.Movement
{
         public class PlayerSlideAttacker : MonoBehaviour
     {
          [SerializeField] private float maxHoldDuration = 5f;
          [SerializeField] private Slider powerSlider;  
          [SerializeField] private float cooldownTime = 1f;
          [FormerlySerializedAs("powerupSpeed")] [SerializeField] private float powerChangeSpeed = 0.1f;
          [SerializeField] private Transform turretExit;
          [SerializeField] private Projectile projectile;
          [SerializeField] private TrajectoryLine trajectoryLine;

          private bool isChangingPower = false;
          private bool isCanFire = true;
          private Scene testScene;
          private GameObject parentColliderGO;    // the nearest parent with the collider to be aware of when the projectile is fired


          // private InputSwitchHandler inputSwitchHandler;
          private InputStyle inputStyle = InputStyle.First;

          #region Events
          public event Action OnTurretPowering;
          public event Action OnTurretFired;
          
          public Action<int, PointType, BodyPartType?> OnTurretHit;

          #endregion
          
          private void OnEnable()
          {
               TurboTowers.Turrets.Controls.PlayerController.onVerticalTouchDrag += PowerChangeHandler;
               TurboTowers.Turrets.Controls.PlayerController.onTouchEnded += TouchEndedHandler;
               
               StartCoroutine(HandleInputListeners());
          }

          private IEnumerator HandleInputListeners()
          {
               if (!InputSwitchHandler.Instance) yield return null;
               
               InputSwitchHandler.Instance.OnInputStyleSelect += InputStyleSelectHandler;
          }

          private void OnDisable()
          {
               TurboTowers.Turrets.Controls.PlayerController.onVerticalTouchDrag -= PowerChangeHandler;
               TurboTowers.Turrets.Controls.PlayerController.onTouchEnded -= TouchEndedHandler;


               InputSwitchHandler.Instance.OnInputStyleSelect -= InputStyleSelectHandler;
          }
          
          private void Start()
          {
               parentColliderGO = GetComponentInParent<Collider>()?.gameObject;

               powerSlider.maxValue = maxHoldDuration;
               OnTurretHit = HandleHit;
          }
          
          private void Update()
          {
               // if (inputStyle != 2 ) return;
               //
               // if (isAvailableToFire && isChangingPower)
               // {
               //      PowerUpHandler();
               // }
          }

          private void InputStyleSelectHandler(InputStyle inputStyle)
          {
               this.inputStyle = inputStyle;
          }

          private void PowerUpStartHandler()
          {
               isChangingPower = true;
          }

          private void PowerChangeHandler(int powerDirection)
          {
               if (inputStyle != InputStyle.Second) return;
               if (!isCanFire) return;
               
               if (isChangingPower == false)
                    isChangingPower = true;
               
               OnTurretPowering?.Invoke();
               
               if (powerDirection == 1 && powerSlider.value < powerSlider.maxValue)
               {
                    PowerUpHandler();
               }
               else if (powerSlider.value > powerSlider.minValue)
               {
                    PowerDownHandler();
               }
          }

          private void TouchEndedHandler()
          {
               if (inputStyle != InputStyle.Second) return;
               if (!isChangingPower) return;
               
               Fire();
          }
          
          private void PowerUpHandler()
          {
               powerSlider.value += powerChangeSpeed;

               trajectoryLine.ShowTrajectoryLine(turretExit.position,
                    turretExit.up * (powerSlider.value / powerSlider.maxValue) * projectile.FirePowerMultiplier);
          }

          private void PowerDownHandler()
          {
               powerSlider.value -= powerChangeSpeed;
               trajectoryLine.ShowTrajectoryLine(turretExit.position,
                    turretExit.up * (powerSlider.value / powerSlider.maxValue) * projectile.FirePowerMultiplier);
          }
          
          private void PowerClickedUpHandler()
          {
               // if (powerBtn.ElapsedTime < powerBtn.MinPressDuration)
               // {
               //      isPoweringUp = false;
               //      powerSlider.value = 0;
               //      return;
               // };
               
               Fire();
          }

          private void PowerClickedDownHandler()
          {
               // isPoweringUp = true;
          }

          private void Fire()
          {
               isCanFire = false;
               isChangingPower = false;
               
               var projectile = Instantiate(this.projectile, turretExit.transform.position, Quaternion.identity);
               
               projectile.Fire(turretExit.up,
                    powerSlider.value / powerSlider.maxValue,
                    GetComponent<Health>(),
                    OnTurretHit, parentColliderGO);
               powerSlider.value = 0;
               trajectoryLine.RemoveTrajectoryLine();

               OnTurretFired?.Invoke();
               Invoke(nameof(Cooldown), cooldownTime);
          }

          private void Cooldown()
          {
               isCanFire = true;
          }
          
          private void HandleHit(int damagePerformed, PointType type, BodyPartType? bodyPartType)
          {
               Debug.Log("turret hit with damage: " + damagePerformed);
          }
     }
}
