using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;

public class MainMenu : MonoBehaviour
{
    public UIBlock Root = null;
    
    private void Start()
    {
        Root = GetComponent<UIBlock>();
        Root.AddGestureHandler<Gesture.OnPress, PlayButton>(PlayButton.HandlePress);
    }
}
