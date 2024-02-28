using TurboTowers.SceneManagement;
using UnityEngine;

namespace TurboTowers.Core
{
    public class WinCanvas : MonoBehaviour
    {
        private void OnEnable()
        {
            GameManager.OnGameStateChanged += GameStateChangedHandler;
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= GameStateChangedHandler;
        }

        private void GameStateChangedHandler(GameState newGameState)
        {
            if (newGameState != GameState.Victory) return;

            GetComponent<CanvasGroup>().alpha = 1;
            GetComponent<CanvasGroup>().interactable = true;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        public void OnNextLevelBtnClick()
        {
            LevelLoader.Instance.LoadNextLevel();
        }
    }
}