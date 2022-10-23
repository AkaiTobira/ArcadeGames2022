using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TweenManipulationAnimation : ScreenAnimation
{
    [SerializeField] private Transform _center; 
    [SerializeField] private Transform _bottom; 

    protected override void OnStart(){}
    protected override void OnStateExit(State nextState) {}
    protected override void OnStateUpdate() {}
    protected override void OnStateEnter(State nextState) {

        switch (nextState) {
            case State.Showing:
                TweenManager.Instance.TweenTo(
                    (ActiveAnimation._image.transform as RectTransform), 
                    _center, 
                    ActiveAnimation._showTimeDuration);
            break;
            case State.Hiding:
                TweenManager.Instance.TweenTo(
                    (ActiveAnimation._image.transform as RectTransform), 
                    _bottom, 
                    ActiveAnimation._hideTimeDuration);
            break;
        }
    }
}
