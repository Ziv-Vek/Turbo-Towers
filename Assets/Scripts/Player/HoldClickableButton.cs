using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldClickableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _holdDuration;
    private float minPressDuration = 0.2f;
    public float MinPressDuration
    {
        get { return minPressDuration; }
    }
    public float HoldDuration
    {
        get { return _holdDuration; }
    }
    
    #region EVENTS

    public event Action OnShortClicked;
    public event Action OnHoldClickMaxed;
    public event Action OnClickUp;
    public event Action OnClickDown;

    #endregion

    private bool _isHoldingButton;
    private float _elapsedTime;
    public float ElapsedTime
    {
        get { return _elapsedTime; }
    }

    private void Update() => ManageButtonInteraction();
    
    public void OnPointerDown(PointerEventData eventData) => ToggleHoldingButton(true);

    public void OnPointerUp(PointerEventData eventData)
    {
        // every time button was released
        ManageButtonInteraction(true);
        ToggleHoldingButton(false);
        
        OnClickUp?.Invoke();
    }
    
    private void ToggleHoldingButton(bool isPointerDown)
    {
        if (isPointerDown)
            OnClickDown?.Invoke();
        
        _isHoldingButton = isPointerDown;

        if (isPointerDown)
            _elapsedTime = 0;
    }

    private void ManageButtonInteraction(bool isPointerUp = false)
    {
        if (!_isHoldingButton)
            return;

        if (isPointerUp)
        {
            Click();
            return;
        }

        _elapsedTime += Time.deltaTime;
        var isHoldClickDurationReached = _elapsedTime > _holdDuration;

        if (isHoldClickDurationReached)
            HoldClick();
    }

    private void Click()
    {
        // released button before hold duration reached
        OnShortClicked?.Invoke();
    }

    private void HoldClick()
    {
        // reached hold duration
        ToggleHoldingButton(false);
        
        
            OnHoldClickMaxed?.Invoke();
    }
}