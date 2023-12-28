using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthHandler))]
public class EnemyController : MonoBehaviour, ITargetable
{
    [SerializeField] private Material targetedHeadMaterial;
    [SerializeField] private Material headTargetedMaterial;
    [SerializeField] private Renderer headRenderer;
    [SerializeField] private Renderer baseRenderer;

    private Material originalHeadMaterial;
    private Material originalBaseMaterial;

    private const float IntervalBetweenTargetUnpainting = 2f;

    private void Awake()
    {
        originalHeadMaterial = headRenderer.material;
        originalBaseMaterial = baseRenderer.material;
    }

    private void Start()
    {
        StartCoroutine(ContinuesUnpainingTarget());
    }

    private IEnumerator ContinuesUnpainingTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(IntervalBetweenTargetUnpainting);
            UnPaintTargeted();
        }
    }

    private void ResetMaterial()
    {
        headRenderer.material = originalHeadMaterial;
        baseRenderer.material = originalBaseMaterial;
    }
    
    public void PaintTargeted()
    {
        Debug.Log("painting: " + gameObject.name);
        headRenderer.material = targetedHeadMaterial;
        baseRenderer.material = headTargetedMaterial;
    }

    public void UnPaintTargeted()
    {
        ResetMaterial();
    }
}
