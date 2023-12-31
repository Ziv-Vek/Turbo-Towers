using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // stats config:
    [SerializeField] private float touchDragDeadZone = 5f;
    [SerializeField] private float minAngleToPowerup = 60;
    [SerializeField] private float maxAngleToRotate = 50;
    [SerializeField] private float minVerticalDelta = 2f;
  
    // variables:
    private bool isTouching = false;
    bool isFirstTouch = true;
    bool isMoving = false;
    private int inputStyle = 1;

    // private Vector2 startPos = new Vector2();
    private Vector2 previousTouchPos = new Vector2();
  
    // cached ref:
    private Coroutine inputDragCoroutine;
    private Camera mainCam;
  
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;

    #region EVENTS

    public static event Action<int> onVerticalTouchDrag;
    public static event Action<int> onHorizontalTouchDrag;
    // public static event Action<Vector2> onTouchStarted;
    // public static event Action<Vector2> onTouchPerformed; 
    public static event Action onTouchEnded;
    #endregion

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions.FindAction("TouchPress");
        touchPositionAction = playerInput.actions["TouchPosition"];
        
        mainCam = Camera.main;
    }

    private void OnEnable() {
        touchPressAction.performed += TouchPressed;
        touchPressAction.canceled += TouchCanceled;
        
        StartCoroutine(HandleInputListeners());
    }

    private void OnDisable()
    {
        touchPressAction.performed += TouchPressed;
        touchPressAction.canceled -= TouchCanceled;
        
        InputSwitchHandler.Instance.OnInputStyleSelect -= SetInputStyle;
    }
    
    private IEnumerator HandleInputListeners()
    {
        if (!InputSwitchHandler.Instance) yield return null;
          
        InputSwitchHandler.Instance.OnInputStyleSelect += SetInputStyle;
    }

    private void SetInputStyle(int inputStyle)
    {
        this.inputStyle = inputStyle;
    }

    void TouchPressed(InputAction.CallbackContext context)
    {
        Vector2 startTouchPos = touchPositionAction.ReadValue<Vector2>();
        // startPos = startTouchPos;
        previousTouchPos = startTouchPos;
        
        isTouching = true;
        
        // onTouchStarted?.Invoke(startTouchPos);
        
        StartCoroutine(TouchDrag(startTouchPos));
    }

    void TouchCanceled(InputAction.CallbackContext context)
    {
        isTouching = false;
        onTouchEnded?.Invoke();
    }

    IEnumerator TouchDrag(Vector2 startTouchPos)
    {
        float angle;
        
         var currentTouchPos = new Vector2();
        
         while (isTouching)
         {
             currentTouchPos = touchPositionAction.ReadValue<Vector2>();

             (bool isOutsideOfDeadzone, bool isVerticalDeltaEnough) = IsOutsideMinimalDrag(startTouchPos, currentTouchPos);
             
             angle = Mathf.Atan2(currentTouchPos.y - startTouchPos.y, currentTouchPos.x - startTouchPos.x) * 180 / Mathf.PI;
             // if (Vector2.Distance(startTouchPos, currentTouchPos) > touchDragDeadZone)

             // Debug.Log($"current touch y is {currentTouchPos.y}, previous touch y is {previousTouchPos.y}");
             // Debug.Log($"is vertical valid: {isVerticalDeltaEnough}");
             
             if (isVerticalDeltaEnough) 
                 HandleVerticalDragActionByTouchAngle(angle, currentTouchPos.y);
             if (isOutsideOfDeadzone)
                 HandelHorizontalActionByTouchAngle(angle);

           previousTouchPos = currentTouchPos;
             
             yield return null;
         }
    }

    private (bool, bool) IsOutsideMinimalDrag(Vector2 startTouchPos, Vector2 currentTouchPos)
    {
        var isOutsideDeadzone = Vector2.Distance(startTouchPos, currentTouchPos) > touchDragDeadZone;
        var isVerticalDeltaEnough = Mathf.Abs(Mathf.Abs(currentTouchPos.y) - Mathf.Abs(previousTouchPos.y)) > minVerticalDelta;

        return (isOutsideDeadzone, isVerticalDeltaEnough);
    }

    private void HandleVerticalDragActionByTouchAngle(float angle, float currentTouchY)
    {
        if (inputStyle == 1)
        {
            // Vertical drag up:
            if (angle >= minAngleToPowerup && angle <= (180 - minAngleToPowerup))
            {
                onVerticalTouchDrag?.Invoke(1);
            }
            // Vertical drag down:
            else if (angle <= -minAngleToPowerup && angle >= (-180 + minAngleToPowerup))
            {
                onVerticalTouchDrag?.Invoke(-1);
            }
        } else if (inputStyle == 2)
        {
            // Check if dragging within allowed vertical angles:
            if (!(angle >= minAngleToPowerup && angle <= (180 - minAngleToPowerup))
                && !(angle <= -minAngleToPowerup && angle >= (-180 + minAngleToPowerup)))
                return;
            
            // Vertical drag up:
            if (currentTouchY > previousTouchPos.y)
                onVerticalTouchDrag?.Invoke(1);
            else
            // Vertical drag down:
                onVerticalTouchDrag?.Invoke(-1);
        }
    }
    
    private void HandelHorizontalActionByTouchAngle(float angle)
    {
        // Horizontal drag right:
        if (Mathf.Abs(angle) <= maxAngleToRotate)
        {
            onHorizontalTouchDrag?.Invoke(1);
        } 
        // Horizontal drag left:
        else if (Mathf.Abs(angle) >= (180 - maxAngleToRotate))
        {
            onHorizontalTouchDrag?.Invoke(-1);
        }
    }
}
