using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurboTowers.SceneManagement
{
    public class LevelLoader : MonoBehaviour
    {
        public static LevelLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadNextLevel()
        {
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(++currentSceneIndex);
        }
        
        public void GotoMainMenu()
        {
            SceneManager.LoadScene(0);
        }
    }   
}

