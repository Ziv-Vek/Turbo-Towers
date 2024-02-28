using System.Collections;
using System.Collections.Generic;
using TurboTowers.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TurboTowers.Prototype
{
    public class WinLoseOpener : MonoBehaviour
    {
        public void OpenWin()
        {
            GameManager.Instance.SetGameState(GameState.Victory);
        }

        public void OpenLose()
        {
            GameManager.Instance.SetGameState(GameState.Defeat);
        }
    }
}