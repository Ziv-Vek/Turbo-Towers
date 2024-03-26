using System.Collections;
using System.Collections.Generic;
using Nova;
using TurboTowers.SceneManagement;
using UnityEngine;

public class PlayButton : ItemVisuals
{
    public static void HandlePress(Gesture.OnPress evt, PlayButton target)
    {
        LevelLoader.Instance.LoadNextLevel();
    }
}
