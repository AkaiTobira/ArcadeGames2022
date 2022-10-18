using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class IAction {
    public Transform Instance;
    public float ActionDuration;
    public float ElapsedTime;
    public Action OnActionEnd;
    public abstract bool Process();

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
            T temp = _actions[toRemove[i]];
            _actions[toRemove[i]] =_actions[activeActions-1];
            _actions[activeActions-1] = temp;
            activeActions--;
            temp.OnActionEnd?.Invoke();
        }

        if(activeActions != 0){
            int maxIntervals = Mathf.Max(1, Mathf.RoundToInt(activeActions / (float)MAX_ACTIONS_PER_FRAME));
            updateInterval = (updateInterval + 1) % maxIntervals;
        }else{
            updateInterval = 0;
        }
    }

}
