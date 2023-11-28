using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerSlideAttacker : MonoBehaviour
{
     [SerializeField] private float maxHoldDuration = 5f;
     [SerializeField] private Slider powerSlider;
     
     [SerializeField] private float powerupSpeed = 0.1f;
     [SerializeField] private float powerdownSpeed = 0.3f;
     [SerializeField] private Transform turretExit;
     [SerializeField] private Projectile projectile;
     [SerializeField] private TrajectoryLine trajectoryLine;

     private bool isChangingPower = false;
     private bool isAvailableToFire = true;
     private Scene testScene;

     // private InputSwitchHandler inputSwitchHandler;
     private int inputStyle = 1;
     
     private void OnEnable()
     {
          PlayerController.onVerticalTouchDrag += PowerChangeHandler;
          PlayerController.onTouchEnded += TouchEndedHandler;
          
          StartCoroutine(HandleInputListeners());
     }

     private IEnumerator HandleInputListeners()
     {
          if (!InputSwitchHandler.Instance) yield return null;
          
          InputSwitchHandler.Instance.OnInputStyleSelect += SetInputStyle;
     }

     private void OnDisable()
     {
          PlayerController.onVerticalTouchDrag -= PowerChangeHandler;
          PlayerController.onTouchEnded -= TouchEndedHandler;


          InputSwitchHandler.Instance.OnInputStyleSelect -= SetInputStyle;
     }
     
     private void Start()
     {
          powerSlider.maxValue = maxHoldDuration;
     }
     
     private void Update()
     {
          if (inputStyle != 2 ) return;
          
          if (isAvailableToFire && isChangingPower)
          {
               // PowerUpHandler();
          }
     }

     private void SetInputStyle(int inputStyle)
     {
          this.inputStyle = inputStyle;
     }

     private void PowerUpStartHandler()
     {
          isChangingPower = true;
     }

     private void PowerChangeHandler(int powerDirection)
     {
          if (isChangingPower == false)
               isChangingPower = true;
          
          if (powerDirection == 1 && powerSlider.value < powerSlider.maxValue)
          {
               PowerUpHandler();
               powerSlider.value += powerupSpeed;
          }
          else if (powerSlider.value > powerSlider.minValue)
          {
               
               powerSlider.value -= powerdownSpeed;
          }
     }

     private void TouchEndedHandler()
     {
          if (!isChangingPower) return;
          
          isChangingPower = false;
          Fire();

     }
     
     private void PowerUpHandler(int powerDirection)
     {
          // if (powerBtn.ElapsedTime < powerBtn.MinPressDuration)
          //      return;
          
          // powerSlider.value += Time.deltaTime;
          trajectoryLine.ShowTrajectoryLine(turretExit.position,
               turretExit.up * (powerSlider.value / powerSlider.maxValue) * projectile.FirePowerMultiplier);
          // if (powerSlider.value >= powerSlider.maxValue)
          //      Fire();
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
          // trajectoryLine.RemoveTrajectoryLine();
     }

     private void PowerClickedDownHandler()
     {
          // isPoweringUp = true;
     }

     private void Fire()
     {
          // isPoweringUp = false;
          //
          // var projectile =
          //      Instantiate(this.projectile, turretExit.transform.position, Quaternion.identity);
          //
          // projectile.Fire(turretExit.up, powerSlider.value / powerSlider.maxValue);
          // powerSlider.value = 0;
     }
}
