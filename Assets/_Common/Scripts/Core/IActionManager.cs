using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class IAction {

    public int ActionID;
    public Transform Instance;
    public float ActionDuration;
    public float ElapsedTime;
    public Action OnActionEnd;
    public abstract bool Process();

    public void Clear(){
        Instance = null;
        OnActionEnd = null;
    }

    public bool SkipProcess(){
        ElapsedTime = Mathf.Min(ElapsedTime + Time.deltaTime, ActionDuration);
        if(ElapsedTime >= ActionDuration){
            return Process();
        }
        return false;
    }
}

public abstract class IActionManager<T> : CUpdateMonoBehaviour
where T : IAction
{
    protected List<T> _actions = new List<T>();
    
    protected int activeActions = 0;
    protected int actionCounter = 0;

    const int MAX_ACTIONS_PER_FRAME = 75;
    private int updateInterval = 0;

    public override void CUpdate() {
        if(activeActions == 0){
            enabled = false;
            return;
        }

        UpdateActions();
    }

    void UpdateActions(){
        List<int> toRemove = new List<int>();

        int currentInterval    =  updateInterval      * MAX_ACTIONS_PER_FRAME;
        int currentIntervalEnd = (updateInterval + 1) * MAX_ACTIONS_PER_FRAME;
        for(int i = 0; i < activeActions; i++) {
            T action = _actions[i];

            if( i >= currentInterval && i < currentIntervalEnd){
                if(action.Process()) toRemove.Add(i);
                continue;
            }

            if(action.SkipProcess()) toRemove.Add(i);
        }
        for(int i = toRemove.Count-1; i >= 0; i--) {
            RemoveAction(toRemove[i], true);
        }

        if(activeActions != 0){
            int maxIntervals = Mathf.Max(1, Mathf.RoundToInt(activeActions / (float)MAX_ACTIONS_PER_FRAME));
            updateInterval = (updateInterval + 1) % maxIntervals;
        }else{
            updateInterval = 0;
        }
    }

    public void InvalidateAction(int actionID, bool callOnActionEnd = false){

        for(int i = 0; i < activeActions; i++) {
            T action = _actions[i];
            if(actionID == action.ActionID){
                RemoveAction(i, callOnActionEnd);
                break;
            }
        }
    }
    
    private void RemoveAction(int index, bool callOnActionEnd){
        T temp = _actions[index];
        if(activeActions > 1){
            _actions[index] =_actions[activeActions-1];
            _actions[activeActions-1] = temp;
        }
        activeActions--;
        if(callOnActionEnd) temp.OnActionEnd?.Invoke();
        temp.Clear();
    }
}
