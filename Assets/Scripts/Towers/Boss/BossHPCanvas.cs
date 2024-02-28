using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Tools;
using TurboTowers.Turrets.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossHPCanvas : MonoBehaviour
{
    [SerializeField] private BossController controller;
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float scaleOutTime = 0.5f;
    [SerializeField] private float sliderScaleTime = 0.5f;
    
    [SerializeField] private RectTransform sliderRect;
    [SerializeField] private Slider slider;
    
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    private void OnEnable()
    {
        controller.OnBossBegin += OnBossBeginHandler;
        controller.GetComponent<Health>().onDamageTaken += OnHitHandler;
    }

    private void OnDisable()
    {
        if (controller)
        {
            controller.OnBossBegin -= OnBossBeginHandler;
            controller.GetComponent<Health>().onDamageTaken -= OnHitHandler;    
        }
    }

    private void OnBossBeginHandler()
    {
        slider.maxValue =controller.GetComponent<Health>().GetCurrentHealth();
        slider.value = slider.maxValue;
        FadeIn();
    }
    
    private void OnHitHandler(int damage, BodyPart hittedBodyPart)
    {
        DOTweenModuleUI.DOValue(slider, slider.value - damage, sliderScaleTime).SetEase(Ease.OutExpo);
    }
        
    private void FadeIn()
    {
        Vector2 initialSize = sliderRect.sizeDelta;
        sliderRect.sizeDelta = Vector2.zero;
        
        DOTweenModuleUI.DOFade(canvasGroup, 1, fadeInTime).SetEase(Ease.OutBack).OnComplete(() =>
        {
            sliderRect.DOSizeDelta(initialSize, scaleOutTime).SetEase(Ease.OutBack);
        });
    }
}
