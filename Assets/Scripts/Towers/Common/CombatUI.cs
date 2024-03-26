using System;
using System.Collections;
using System.Collections.Generic;
using TurboTowers.Turrets.Combat;
using TurboTowers.Turrets.Controls;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private GameObject powerUpButton;
    [SerializeField] private GameObject shootingSlider;
    [SerializeField] private GameObject cooldownTimer;

    private float cooldownTime;
    private InputStyle inputStyle;
    PlayerAttacker playerAttacker;

    private void OnEnable()
    {
        playerAttacker = GameObject.FindWithTag("Player").GetComponent<PlayerAttacker>();
        //playerAttacker.OnTurretFired += OnTurretFiredHandler;
    }

    private void OnDisable()
    {
        //playerAttacker.OnTurretFired -= OnTurretFiredHandler;
    }

    void Start()
    {
        cooldownTime = GameObject.FindWithTag("Player")?.GetComponent<PlayerAttacker>()?.GetCooldownTime() ?? 1f;
        inputStyle = GameObject.FindWithTag("Player").GetComponent<PlayerController>()?.inputStyle ?? InputStyle.First;
        
        if (inputStyle == InputStyle.Fourth)
        {
            powerUpButton.SetActive(false);
            shootingSlider.SetActive(false);
            //cooldownTimer.SetActive(true);
        }
        else
        {
            powerUpButton.SetActive(true);
            shootingSlider.SetActive(true);
            cooldownTimer.SetActive(false);
        }
        
        if (GameObject.FindWithTag("Player").GetComponent<PlayerController>()?.inputStyle == InputStyle.Fourth)
        {
            //GetComponent<CanvasGroup>().alpha = 0;
        }
    }
    
    private void OnTurretFiredHandler()
    {
        if (inputStyle == InputStyle.Fourth)
        {
            StartCoroutine(HandleCooldown());
        }
    }

    IEnumerator HandleCooldown()
    {
        cooldownTimer.SetActive(true);
        cooldownTimer.GetComponent<Image>().fillAmount = 0;
        
        while (cooldownTimer.GetComponent<Image>().fillAmount < 1)
        {
            cooldownTimer.GetComponent<Image>().fillAmount += Time.deltaTime / cooldownTime;
            yield return null;
        }
        
        cooldownTimer.SetActive(false);
    }
}
