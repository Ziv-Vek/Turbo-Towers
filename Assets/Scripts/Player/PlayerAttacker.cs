using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerAttacker : MonoBehaviour
{
     [SerializeField] private HoldClickableButton powerBtn;
     
     [SerializeField] private Slider powerSlider;
     [SerializeField] private float powerupSpeed = 0.1f;
     [SerializeField] private float powerdownSpeed = 0.3f;
     [SerializeField] private Transform turretExit;
     [SerializeField] private Projectile projectile;
     [SerializeField] private TrajectoryLine trajectoryLine;

     private bool isPoweringUp = false;
     private bool isAvailableToFire = true;
     private Scene testScene;

     private InputSwitchHandler inputSwitchHandler;

     private void Awake()
     {
          inputSwitchHandler = InputSwitchHandler.Instance;
     }

     private void OnEnable()
     {
          // powerBtn.OnShortClicked += PowerUpStartHandler;
          powerBtn.OnHoldClickMaxed += Fire;
          powerBtn.OnClickUp += PowerClickedUpHandler;
          powerBtn.OnClickDown += PowerClickedDownHandler;
     }

     private void OnDisable()
     {
          // powerBtn.OnShortClicked -= PowerUpStartHandler;
          powerBtn.OnHoldClickMaxed -= Fire;
          powerBtn.OnClickUp -= PowerClickedUpHandler;
          powerBtn.OnClickDown -= PowerClickedDownHandler;
     }
     
     private void Start()
     {
          testScene = SceneManager.CreateScene("TestScene");
          powerSlider.maxValue = powerBtn.HoldDuration;
     }

     private void Update()
     {
          if (isAvailableToFire && isPoweringUp)
          {
               PowerUpHandler();
          }
     }

     private void PowerUpStartHandler()
     {
          isPoweringUp = true;
     }
     
     private void PowerUpHandler()
     {
          if (powerBtn.ElapsedTime < powerBtn.MinPressDuration)
               return;
          
          powerSlider.value += Time.deltaTime;
          trajectoryLine.ShowTrajectoryLine(turretExit.position, turretExit.up * (powerSlider.value / powerSlider.maxValue) * projectile.FirePowerMultiplier);
          if (powerSlider.value >= powerSlider.maxValue)
               Fire();
     }
     
     private void PowerClickedUpHandler()
     {
          if (powerBtn.ElapsedTime < powerBtn.MinPressDuration)
          {
               isPoweringUp = false;
               powerSlider.value = 0;
               return;
          };
          
          Fire();
     }

     private void PowerClickedDownHandler()
     {
          isPoweringUp = true;
     }

     private void Fire()
     {
          isPoweringUp = false;

          var projectile =
               Instantiate(this.projectile, turretExit.transform.position, Quaternion.identity);
          
          Debug.DrawRay(projectile.transform.position, turretExit.up * 100f, Color.red, 2f, false);
          projectile.Fire(turretExit.up, powerSlider.value / powerSlider.maxValue);
          powerSlider.value = 0;
     }
     
}
