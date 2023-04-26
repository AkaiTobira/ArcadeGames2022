using System.Collections.Generic;
using System;
using UnityEngine;

public class TimeAction : IAction{
    override public bool Process(){
        if(ActionDuration <= 0 || ElapsedTime >= ActionDuration){
            return true;
        }

        ElapsedTime = Mathf.Min(ElapsedTime + Time.deltaTime, ActionDuration);
        return false;
    }
} 

public class TimersManager : IActionManager<TimeAction>
{
    public static TimersManager Instance;

    void Awake() {
        if(Instance == null){
            Instance = this;
            enabled = false;
            DontDestroyOnLoad(this);
        }else{
            Destroy(gameObject);
        }
    }

    public void FireAfter(float time, Action OnEnd){
        enabled = true;

        if(_actions.Count > activeActions){
            TimeAction action       = _actions[activeActions];
            action.ActionDuration   = time;
            action.ElapsedTime      = 0;
            action.OnActionEnd      = OnEnd;
            action.ActionID = actionCounter;
            activeActions++;
            actionCounter++;
            return;
        }

        _actions.Add(
            new TimeAction{
                ActionDuration = time,
                ElapsedTime = 0,
                OnActionEnd = OnEnd,
                ActionID = actionCounter
            }
        );
        activeActions++;
        actionCounter++;
    }
}


