using System;
using UnityEngine;

namespace TurboTowers.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static event Action<GameState> OnGameStateChanged;
        public GameState initialLevelState = GameState.InGameCutscene;
        public LevelType levelType = LevelType.KillBoss;
        private GameState currentGameState = GameState.Null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetGameState(initialLevelState);
            
            //TODO: remove previous line

/*#if UNITY_EDITOR
            OnGameStateChanged?.Invoke(GameState.InGame);
#endif*/
        }

        public void SetGameState(GameState newGameState)
        {
            if (newGameState == currentGameState) return;

            currentGameState = newGameState;

            switch (newGameState)
            {
                case GameState.MainMenu:
                    break;
                case GameState.InGame:
                    break;
                case GameState.InGameBoss:
                    break;
                case GameState.Victory:
                    Debug.Log("Victory!");
                    HandleVictory();
                    break;
                case GameState.Defeat:
                    HandleDefeat();
                    break;
                case GameState.Paused:
                    break;
                case GameState.InGameSettings:
                    break;
                case GameState.InGameCutscene:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
            }

            OnGameStateChanged?.Invoke(newGameState);
        }

        private void HandleDefeat()
        {
        }

        private void HandleVictory()
        {
        }

        public void OnCameraDollyFinished()
        {
            SetGameState(GameState.InGame);
        }
        
        public void HandleAllTowersKilled()
        {
            if (levelType == LevelType.KillAllTowers)
                SetGameState(GameState.Victory);
        }
    }

    public enum GameState
    {
        MainMenu,
        InGame,
        InGameBoss,
        Victory,
        Defeat,
        Paused,
        InGameSettings,
        InGameCutscene,
        Null
    }
    
    public enum LevelType
    {
        KillAllTowers,
        KillBoss,
        TimeTrial,
    }
}