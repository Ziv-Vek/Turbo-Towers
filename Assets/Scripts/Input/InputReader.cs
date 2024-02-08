using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TurboTowers
{
    [CreateAssetMenu(menuName = "TurboTowers/InputReader")]
    public class InputReader : ScriptableObject, GameInput.IGamePlayActions, GameInput.IUIActions
    {
        private GameInput gameInput;

        private void OnEnable()
        {
            if (gameInput == null)
            {
                gameInput = new GameInput();
                
                gameInput.GamePlay.SetCallbacks(this);
                gameInput.UI.SetCallbacks(this);
                
                SetGameplay();
            }
        }

        public void SetGameplay()
        {
            gameInput.GamePlay.Enable();
            gameInput.UI.Disable();
        }

        public void SetUI()
        {
            gameInput.GamePlay.Disable();
            gameInput.UI.Enable();
        }
        

        public event Action<Vector2> MoveEvent;

        public event Action PressEvent;
        public event Action PressCancelledEvent;

        public event Action UIPressEvent;
        public event Action UIPressCancelledEvent;
        

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        void GameInput.IGamePlayActions.OnPress(InputAction.CallbackContext context)
        {
            Debug.Log("OnPress clicked");
            
            if (context.phase == InputActionPhase.Performed)
            {
                PressEvent?.Invoke();
            }

            if (context.phase == InputActionPhase.Canceled)
            {
                PressCancelledEvent?.Invoke();
            }
        }

        void GameInput.IUIActions.OnPress(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                PressEvent?.Invoke();
                SetUI();
            }
        }
        
        //Same but for cancelled event and with SetGamePlay();
    }
}

