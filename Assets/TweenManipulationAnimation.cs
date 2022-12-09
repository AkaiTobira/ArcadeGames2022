using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TweenManipulationAnimation : ScreenAnimation
{
    [SerializeField] private Transform _center; 
    [SerializeField] private Transform _bottom; 

    protected override void OnStart(){ base.OnStart(); }
    protected override void OnStateExit(State nextState) {}
    protected override void OnStateUpdate() {}
    protected override void OnStateEnter(State nextState) {

        switch (nextState) {
            case State.Showing:
                if(Guard.IsValid(ActiveAnimation._activateComponent))
                    ActiveAnimation._activateComponent.gameObject.SetActive(true);

                TweenManager.Instance.TweenTo(
                    (ActiveAnimation._image.transform as RectTransform), 
                    _center, 
                    ActiveAnimation._showTimeDuration);
            break;
            case State.Hiding:
                if(Guard.IsValid(ActiveAnimation._activateComponent)){
                    Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.SaveRankings));
                }

                TweenManager.Instance.TweenTo(
                    (ActiveAnimation._image.transform as RectTransform), 
                    _bottom, 
                    ActiveAnimation._hideTimeDuration,
                    () =>{
                        if(Guard.IsValid(ActiveAnimation._activateComponent)){
                            ActiveAnimation._activateComponent.gameObject.SetActive(false);
                        }
                    }
                    );
            break;
        }
    }
}
