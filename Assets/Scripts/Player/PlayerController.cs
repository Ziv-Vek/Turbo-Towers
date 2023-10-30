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

    public static event Action<int> onVerticalTouchDrag;
    public static event Action<int> onHorizontalTouchDrag;
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

                HandleVerticalDragActionByTouchAngle(angle);
                HandelHorizontalActionByTouchAngle(angle);
             }
             yield return null;
         }
    }

    private void HandleVerticalDragActionByTouchAngle(float angle)
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
