using System;
using System.Collections;
using JetBrains.Annotations;
using TurboTowers.Map;
using TurboTowers.Map.Models;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TurboTowers.Turrets.Controls
{
        public class PlayerController : MonoBehaviour
    {
        // stats config:
        [SerializeField] private float touchDragDeadZone = 5f;
        [SerializeField] private float minAngleToPowerup = 60;
        [SerializeField] private float maxAngleToRotate = 50;
        [SerializeField] private float minVerticalDelta = 2f;
      
        // variables:
        private bool isTouching = false;
        private int inputStyle = 1;

        private Vector2 previousTouchPos;
        
      
        // cached ref:
        private Coroutine inputDragCoroutine;
        
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
            float verticalAngle;
            float horizontalAngle;
            
             var currentTouchPos = new Vector2();
            
             while (isTouching)
             {
                 currentTouchPos = touchPositionAction.ReadValue<Vector2>();

                 (bool isOutsideOfDeadzone, bool isVerticalDeltaEnough) = IsOutsideMinimalDrag(startTouchPos, currentTouchPos);
                 
                 horizontalAngle = Mathf.Atan2(currentTouchPos.y - startTouchPos.y, currentTouchPos.x - startTouchPos.x) * 180 / Mathf.PI;
                 //horizontalAngle = Mathf.Atan2(currentTouchPos.y - previousTouchPos.y, currentTouchPos.x - previousTouchPos.x) * 180 / Mathf.PI;
                 //Debug.Log(angle.ToString());
                 
                 
                 // Calculate direction vector from previous to current touch position
                 Vector2 direction = currentTouchPos - previousTouchPos;

                 // Normalize the direction
                 direction.Normalize();
                
                 // Calculate the angle in radians and then convert to degrees
                 verticalAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                
                 // Normalize angle to 0 to 360 degrees if needed
                 //if (angle < 0) angle += 360;
                 
                 
                 if (isVerticalDeltaEnough)
                     HandleVerticalDragActionByTouchAngle(verticalAngle, currentTouchPos.y);
                 if (isOutsideOfDeadzone)
                     //HandleHorizontalDragActionByTouchDelta(direction.x);
                     HandelHorizontalActionByTouchAngle(horizontalAngle);

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
        
        private void HandleHorizontalDragActionByTouchDelta(float currentTouchY)
        {
            if (currentTouchY > 0)
                onHorizontalTouchDrag?.Invoke(1);
            else
                onHorizontalTouchDrag?.Invoke(-1);
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
    
}
