using System;
using System.Collections;
using TurboTowers.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TurboTowers.Turrets.Controls;

namespace TurboTowers.Turrets.Combat
{
    public class HoldClickableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region Variables

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

        private bool isActiveInInputStyle = true;
        private bool _isHoldingButton;
        private float _elapsedTime;
        private bool _isBtnEnabled = true;

        #endregion

        #region EVENTS

        public event Action OnShortClicked;
        public event Action OnHoldClickMaxed;
        public event Action OnClickUp;
        public event Action OnClickDown;

        #endregion

        #region Unity Events

        private void OnEnable()
        {
            StartCoroutine(HandleInputListeners());
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }

        private IEnumerator HandleInputListeners()
        {
            if (!InputSwitchHandler.Instance) yield return null;

            InputSwitchHandler.Instance.OnInputStyleSelect += ToggleFiringButtonRendering;

            // while (!InputSwitchHandler.Instance)
            // {
            //     yield return null;
            // }
            // if ()
            //     yield return null;
            // else
            // {
            //     InputSwitchHandler.Instance.OnInputStyleSelect += ToggleFiringButtonRendering;
            // }
        }

        private void OnDisable()
        {
            InputSwitchHandler.Instance.OnInputStyleSelect -= ToggleFiringButtonRendering;
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState gameState)
        {
            _isBtnEnabled = gameState is GameState.InGame or GameState.InGameBoss;
        }

        private void Update()
        {
            if (!isActiveInInputStyle) return;

            ManageButtonInteraction();
        }

        #endregion


        #region Public Methods

        public float ElapsedTime
        {
            get { return _elapsedTime; }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isBtnEnabled) return;

            ToggleHoldingButton(true);
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isBtnEnabled) return;

            // every time button was released
            ManageButtonInteraction(true);
            ToggleHoldingButton(false);

            OnClickUp?.Invoke();
        }

        #endregion

        #region Private Methods

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

        private void ToggleFiringButtonRendering(InputStyle inputStyle)
        {
            switch (inputStyle)
            {
                case InputStyle.First:
                    isActiveInInputStyle = true;
                    transform.GetChild(0).gameObject.SetActive(true);
                    GetComponent<Button>().enabled = true;
                    GetComponent<Image>().enabled = true;
                    break;
                case InputStyle.Second:
                    isActiveInInputStyle = false;
                    transform.GetChild(0).gameObject.SetActive(false);
                    GetComponent<Button>().enabled = false;
                    GetComponent<Image>().enabled = false;
                    break;
            }
        }

        #endregion
    }
}