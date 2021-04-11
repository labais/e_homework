using System;
using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

public class RestartButtonBehaviour : MonoBehaviour
{
    
    // Button click triggers this
    public void OnClick()
    {
        GameManager.I.RestartGame();
    }
}