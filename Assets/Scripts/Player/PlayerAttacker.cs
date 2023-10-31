using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttacker : MonoBehaviour
{
     [SerializeField] private HoldClickableButton powerBtn;
     
     [SerializeField] private Slider powerSlider;
     [SerializeField] private float powerupSpeed = 0.1f;
     [SerializeField] private float powerdownSpeed = 0.3f;
     [SerializeField] private Transform turretExit;
     

     private bool isPoweringUp = false;
     private bool isAvailableToFire = true;

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
          if (powerSlider.value >= powerSlider.maxValue)
               Fire();
     }
     
     private void PowerClickedUpHandler()
     {
          if (powerBtn.ElapsedTime < powerBtn.MinPressDuration)
          {
               isPoweringUp = false;
               powerSlider.value = 0;
          };
          
          Fire();
     }

     private void PowerClickedDownHandler()
     {
          isPoweringUp = true;
     }

     private void Fire()
     {
          powerSlider.value = 0;
          isPoweringUp = false;
          Debug.Log("Fire projectile!");
          
          
     }
     
}
