using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class AlphaManipulationAnimation : ScreenAnimation
{
    [SerializeField] private Image _curtain; 

    protected override void OnStart(){
        base.OnStart();
        SetColorRate(1.0f);
    }

    private void SetColorRate(float rate){
        Color color = _curtain.color;
        color.a = rate;
        _curtain.color = color;
    }

    protected override void OnStateExit(State nextState)
    {
        switch(nextState){
            case State.Showing : _curtain.color = ActiveAnimation._color; break;
        }
    }

    protected override void OnStateUpdate()
    {
        switch(CurrentState){
            case State.Hiding:  SetColorRate(1.0f - GetTimeRate()); break;
            case State.Showing: SetColorRate(GetTimeRate()); break;
        }
    }

    protected override void OnStateEnter(State nextState) {}
}
