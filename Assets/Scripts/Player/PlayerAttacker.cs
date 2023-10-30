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

     private bool isPoweringUp = false;

     private void OnEnable()
     {
          powerBtn.OnClicked += PowerUpStartHandler;
          powerBtn.OnHoldClicked += PowerUpHandler;
          powerBtn.OnClickUp += PowerUpStopHandler;
     }

     private void OnDisable()
     {
          powerBtn.OnClicked -= PowerUpStartHandler;
          powerBtn.OnHoldClicked -= PowerUpHandler;
          powerBtn.OnClickUp -= PowerUpStopHandler;
     }

     private void PowerUpStartHandler()
     {
          isPoweringUp = true;
     }
     
     public void PowerUpHandler()
     {
          
          
     }
     
     private void PowerUpStopHandler()
     {
          isPoweringUp = false;
     }



}
