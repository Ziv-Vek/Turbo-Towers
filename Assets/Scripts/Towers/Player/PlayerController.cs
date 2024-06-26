using System;
using System.Collections;
using System.Diagnostics;
using TurboTowers.Core;
using TurboTowers.Turrets.Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace TurboTowers.Turrets.Controls
{
    public enum InputStyle
    {
        First,
        Second,
        Third,
        Fourth
    }

    public class PlayerController : MonoBehaviour
    {
        [Header("Input Style")] [Space(10)] public InputStyle inputStyle = InputStyle.First;

        // config:
        [Header("Config:")] [Space(10)] [SerializeField]
        private float touchDragDeadZone = 5f;

        [SerializeField] private float minAngleToPowerup = 60;
        [SerializeField] private float maxAngleToRotate = 50;
        [SerializeField] private float minVerticalDelta = 2f;
        [SerializeField] private bool isReversePitch;
        

        // variables:
        private bool isTouching = false;
        private bool isTowerControlEnabled = true;
        private Vector2 previousTouchPos;

        // cached ref:
        private Coroutine inputDragCoroutine;

        private PlayerInput playerInput;
        private InputAction touchPositionAction;
        private InputAction touchPressAction;

        #region EVENTS

        public static event Action<int> onVerticalTouchDrag;
        public static event Action<int> onHorizontalTouchDrag;
        public static event Action<Vector2> onTouchStarted;
        public static event Action<Vector2> onTouchPerformed;
        public static event Action onTouchEnded;

        #endregion

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            touchPressAction = playerInput.actions.FindAction("TouchPress");
            touchPositionAction = playerInput.actions["TouchPosition"];
        }

        private void OnEnable()
        {
            touchPressAction.performed += TouchPressed;
            touchPressAction.canceled += TouchCanceled;
            touchPressAction.started += TouchStarted;

            StartCoroutine(HandleInputListeners());

            GetComponent<Health>().OnDeath += OnPlayerDeath;
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }

        /*private bool IsPointerOverUIObject()
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                return EventSystem.current.IsPointerOverGameObject(Touchscreen.current.primaryTouch.);
            }

            return false;
        }*/

        private void TouchStarted(InputAction.CallbackContext obj)
        {
            //if (IsPointerOverUIObject()) return;
            
            if (!isTowerControlEnabled) return;
            
            onTouchStarted?.Invoke(touchPositionAction.ReadValue<Vector2>());
        }

        private void OnDisable()
        {
            touchPressAction.performed += TouchPressed;
            touchPressAction.canceled -= TouchCanceled;
            touchPressAction.started -= TouchStarted;

            InputSwitchHandler.Instance.OnInputStyleSelect -= SetInputStyle;

            GetComponent<Health>().OnDeath -= OnPlayerDeath;
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private IEnumerator HandleInputListeners()
        {
            if (!InputSwitchHandler.Instance) yield return null;

            InputSwitchHandler.Instance.OnInputStyleSelect += SetInputStyle;
        }

        private void OnGameStateChanged(GameState newGameState)
        {
            isTowerControlEnabled = newGameState is GameState.InGame or GameState.InGameBoss;
        }

        private void SetInputStyle(InputStyle inputStyle)
        {
            this.inputStyle = inputStyle;
        }

        void TouchPressed(InputAction.CallbackContext context)
        {
            //if (IsPointerOverUIObject()) return;
            if (!isTowerControlEnabled) return;
            
            Vector2 startTouchPos = touchPositionAction.ReadValue<Vector2>();
            previousTouchPos = startTouchPos;

            isTouching = true;

            // onTouchStarted?.Invoke(startTouchPos);

            StartCoroutine(TouchDrag(startTouchPos));
        }

        void TouchCanceled(InputAction.CallbackContext context)
        {
            if (!isTowerControlEnabled) return;

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
                if (!isTowerControlEnabled)
                {
                    isTouching = false;
                    yield break;
                }
                
                currentTouchPos = touchPositionAction.ReadValue<Vector2>();

                (bool isOutsideOfDeadzone, bool isVerticalDeltaEnough) =
                    IsOutsideMinimalDrag(startTouchPos, currentTouchPos);

                horizontalAngle =
                    Mathf.Atan2(currentTouchPos.y - startTouchPos.y, currentTouchPos.x - startTouchPos.x) * 180 /
                    Mathf.PI;
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

                onTouchPerformed?.Invoke(currentTouchPos);
                yield return null;
            }
        }

        private (bool, bool) IsOutsideMinimalDrag(Vector2 startTouchPos, Vector2 currentTouchPos)
        {
            var isOutsideDeadzone = Vector2.Distance(startTouchPos, currentTouchPos) > touchDragDeadZone;
            var isVerticalDeltaEnough = Mathf.Abs(Mathf.Abs(currentTouchPos.y) - Mathf.Abs(previousTouchPos.y)) >
                                        minVerticalDelta;

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
            var pitchDirectionPreference = isReversePitch ? -1 : 1;  
            
            if (inputStyle == InputStyle.First || inputStyle == InputStyle.Third || inputStyle == InputStyle.Fourth)
            {
                // Vertical drag up:
                if (angle >= minAngleToPowerup && angle <= (180 - minAngleToPowerup))
                {
                    onVerticalTouchDrag?.Invoke(1 * pitchDirectionPreference);
                }
                // Vertical drag down:
                else if (angle <= -minAngleToPowerup && angle >= (-180 + minAngleToPowerup))
                {
                    onVerticalTouchDrag?.Invoke(-1 * pitchDirectionPreference);
                }
            }
            else if (inputStyle == InputStyle.Second)
            {
                // Check if dragging within allowed vertical angles:
                if (!(angle >= minAngleToPowerup && angle <= (180 - minAngleToPowerup))
                    && !(angle <= -minAngleToPowerup && angle >= (-180 + minAngleToPowerup)))
                    return;

                // Vertical drag up:
                if (currentTouchY > previousTouchPos.y)
                    onVerticalTouchDrag?.Invoke(1 * pitchDirectionPreference);
                else
                    // Vertical drag down:
                    onVerticalTouchDrag?.Invoke(-1 * pitchDirectionPreference);
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

        private void OnPlayerDeath()
        {
            GameManager.Instance.SetGameState(GameState.Defeat);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            
        }
        
        public void SetTouchControlEnabled(bool isEnabled)
        {
            isTowerControlEnabled = isEnabled;
        }
    }
}