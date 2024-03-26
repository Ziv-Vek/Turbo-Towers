using System;
using System.Collections;
using System.Collections.Generic;
using TurboTowers.Turrets.Controls;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PreventTouchThroughUI : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
{
    Button button;
    private PlayerController playerController;
    private TouchManager touchManager;
    public UnityEvent onPointerClick;
    public UnityEvent onPointerUp;
    
    private void Start()
    {
        button = GetComponent<Button>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        touchManager = GameObject.FindWithTag("Player").GetComponent<TouchManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //playerController.SetTouchControlEnabled(false);
            onPointerClick?.Invoke();
            //StartCoroutine(EnableControls());
        }
    }
    
    private IEnumerator EnableControls()
    {
        yield return new WaitForSeconds(0.1f);
        playerController.SetTouchControlEnabled(true);
    }
    
    private void OnDestroy()
    {
        /*StopAllCoroutines();
        if (playerController != null)
        {
            playerController.SetTouchControlEnabled(true);
        }*/
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke();
    }
}
