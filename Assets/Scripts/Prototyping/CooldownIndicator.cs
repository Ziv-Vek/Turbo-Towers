using System.Collections;
using System.Collections.Generic;
using TurboTowers.Turrets.Combat;
using UnityEngine;
using UnityEngine.UI;

public class CooldownIndicator : MonoBehaviour
{
    [SerializeField] private GameObject cooldownTimer;
    
    PlayerAttacker playerAttacker;
    private float cooldownTime;
    
    private void OnEnable()
    {
        playerAttacker = GameObject.FindWithTag("Player").GetComponent<PlayerAttacker>();
        playerAttacker.OnTurretFired += OnTurretFiredHandler;
    }

    private void OnDisable()
    {
        playerAttacker.OnTurretFired -= OnTurretFiredHandler;
    }

    void Start()
    {
        cooldownTime = GameObject.FindWithTag("Player")?.GetComponent<PlayerAttacker>()?.GetCooldownTime() ?? 1f;
    }

    private void OnTurretFiredHandler()
    {
        StartCoroutine(HandleCooldown());
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