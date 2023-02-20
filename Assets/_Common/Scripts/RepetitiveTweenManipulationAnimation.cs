using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RepetitiveTweenManipulationAnimation : TweenManipulationAnimation
{
    [SerializeField] private RectTransform _up;
    [SerializeField] private float _spamTime = 20f;

    protected override void OnStart()
    {
        base.OnStart();
        enabled = false;
        ActiveAnimation._image.gameObject.SetActive(false);

        Events.Gameplay.RegisterListener(this, GameplayEventType.SpamWithWindow);

        TimersManager.Instance.FireAfter(0.2f, () => {
            if(!Guard.IsValid(this)) return;

            for(int i = 0; i < _screens.Count; i++) {
                TweenManager.Instance.TweenTo(
                    (_screens[i]._image.transform as RectTransform), 
                    _up, 
                    0f);
            }
        });
    }

    public override void OnGameEvent(GameplayEvent gameplayEvent){
        base.OnGameEvent(gameplayEvent);

        if(gameplayEvent.type == GameplayEventType.SpamWithWindow){
            //Debug.Log(GameplayEventType.SpamWithWindow.ToString() + " Event handled");
            OnStart();
            ActiveAnimation._image.gameObject.SetActive(true);

            
            DigDugger.Player.enabled = false;
            enabled = true;
        }
    }

    public void RestartTimers(){
        DigDugger.Player.enabled = true;

        TimersManager.Instance.FireAfter(
            _spamTime, 
            () => {
                Events.Gameplay.RiseEvent( new GameplayEvent(GameplayEventType.SpamWithWindow));
            //    Debug.Log("Timer fired");
            } );
        Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.ContinueAnimation));
        //Debug.Log("Timer started and continue rised");
        //    OnStateEnter(State.Hiding);
        //SetState(State.Hiding, ActiveAnimation._hideTimeDuration);
    }



    /*
    protected override void OnStart(){}
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
    */
}
