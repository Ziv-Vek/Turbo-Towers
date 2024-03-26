using System;
using System.Collections;
using TurboTowers.Turrets.Controls;
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
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadNextLevel()
        {
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex + 1 > SceneManager.sceneCountInBuildSettings - 1)
            {
                StartCoroutine(LoadScene(0));
            }
            else
            {
                StartCoroutine(LoadScene(++currentSceneIndex));
            }
        }

        public void ReloadScene()
        {
            StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
        }

        public void GotoMainMenu()
        {
            StartCoroutine(LoadScene(0));
        }
        
        IEnumerator LoadScene(int sceneIndex)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().SetTouchControlEnabled(false);
            yield return null;
            
            yield return SceneManager.LoadSceneAsync(sceneIndex);
            
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().SetTouchControlEnabled(true);
            //newController.enabled = false;
            yield return null;
            //newController.enabled = true;
        }
    }
}