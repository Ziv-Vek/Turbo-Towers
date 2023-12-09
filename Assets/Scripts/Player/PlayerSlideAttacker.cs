using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Serialization;

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

     // private InputSwitchHandler inputSwitchHandler;
     private int inputStyle = 1;

     #region Events
     public event Action OnTurretPowering;
     public event Action OnTurretFired;

     #endregion
     
     private void OnEnable()
     {
          PlayerController.onVerticalTouchDrag += PowerChangeHandler;
          PlayerController.onTouchEnded += TouchEndedHandler;
          
          StartCoroutine(HandleInputListeners());
     }

     private IEnumerator HandleInputListeners()
     {
          if (!InputSwitchHandler.Instance) yield return null;
          
          InputSwitchHandler.Instance.OnInputStyleSelect += InputStyleSelectHandler;
     }

     private void OnDisable()
     {
          PlayerController.onVerticalTouchDrag -= PowerChangeHandler;
          PlayerController.onTouchEnded -= TouchEndedHandler;


          InputSwitchHandler.Instance.OnInputStyleSelect -= InputStyleSelectHandler;
     }
     
     private void Start()
     {
          powerSlider.maxValue = maxHoldDuration;
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

     private void InputStyleSelectHandler(int inputStyle)
     {
          this.inputStyle = inputStyle;
     }

     private void PowerUpStartHandler()
     {
          isChangingPower = true;
     }

     private void PowerChangeHandler(int powerDirection)
     {
          if (inputStyle != 2) return;
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
          if (inputStyle != 2) return;
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
          
          projectile.Fire(turretExit.up, powerSlider.value / powerSlider.maxValue, GetComponent<Teleport>());
          powerSlider.value = 0;
          trajectoryLine.RemoveTrajectoryLine();

          OnTurretFired?.Invoke();
          Invoke(nameof(Cooldown), cooldownTime);
     }

     private void Cooldown()
     {
          isCanFire = true;
     }
}
