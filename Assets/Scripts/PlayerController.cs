using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // stats config:
    [SerializeField] private float touchDragDeadZone = 5f;
    [SerializeField] private float minAngleToPowerup = 60;
    [SerializeField] private float maxAngleToRotate = 50;
  
    // variables:
    private bool isTouching = false;
    bool isFirstTouch = true;
    bool isMoving = false;

    private Vector2 startPos = new Vector2();
  
    // cached ref:
    private Coroutine inputDragCoroutine;
    private Camera mainCam;
  
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;
  
    #region EVENTS

    public static event Action onTouchDragUp;
    public static event Action onTouchDragDown;
    public static event Action onTouchDragRight;
    public static event Action onTouchDragLeft;
    // public static event Action<Vector2> onTouchStarted;
    // public static event Action<Vector2> onTouchPerformed; 
    // public static event Action onTouchEnded;
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
    }

    private void OnDisable()
    {
        touchPressAction.performed += TouchPressed;
        touchPressAction.canceled -= TouchCanceled;
    }

    void TouchPressed(InputAction.CallbackContext context)
    {
        Vector2 startTouchPos = touchPositionAction.ReadValue<Vector2>();
        startPos = startTouchPos;
        
        isTouching = true;
        
        // onTouchStarted?.Invoke(startTouchPos);
        
        StartCoroutine(TouchDrag(startTouchPos));
    }

    void TouchCanceled(InputAction.CallbackContext context)
    {
        isTouching = false;
        // onTouchEnded?.Invoke();
    }

    IEnumerator TouchDrag(Vector2 startTouchPos)
    {
        float angle;
        
         var currentTouchPos = new Vector2();
        
         while (isTouching)
         {
             currentTouchPos = touchPositionAction.ReadValue<Vector2>();
             if (Vector2.Distance(startTouchPos, currentTouchPos) > touchDragDeadZone)
             {
                 //onTouchPerformed?.Invoke(currentTouchPos);
                angle = Mathf.Atan2(currentTouchPos.y - startPos.y, currentTouchPos.x - startPos.x) * 180 / Mathf.PI;

                HandlePowerupActionByTouchAngle(angle);
                HandelRotationActionByTouchAngle(angle);
             }
             yield return null;
         }
    }

    private void HandlePowerupActionByTouchAngle(float angle)
    {
        // Powerup:
        if (angle >= minAngleToPowerup && angle <= (180 - minAngleToPowerup))
        {
            onTouchDragUp?.Invoke();
        }
        // Powerdown:
        else if (angle <= -minAngleToPowerup && angle >= (-180 + minAngleToPowerup))
        {
            onTouchDragDown?.Invoke();
        }
    }
    private void HandelRotationActionByTouchAngle(float angle)
    {
        // Rotate right:
        if (Mathf.Abs(angle) <= maxAngleToRotate)
        {
            onTouchDragRight?.Invoke();
        } 
        // Rotate left:
        else if (Mathf.Abs(angle) >= (180 - maxAngleToRotate))
        {
            onTouchDragLeft?.Invoke();
        }
    }
}
