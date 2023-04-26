using System.Collections.Generic;
using System;
using UnityEngine;

public class ScaleAction : IAction
{
    public Vector3      StartingScale;
    public Vector3      ScaleBy;

    override public bool Process(){
        if(!Guard.IsValid(Instance)) return true;

        if(ActionDuration <= 0 || ElapsedTime >= ActionDuration){
            Instance.localScale = StartingScale + ScaleBy;
            return true;
        }

        float timeCoef = ElapsedTime/ActionDuration;
        Instance.localScale = StartingScale + ScaleBy * timeCoef;
        ElapsedTime = Mathf.Min(ElapsedTime + Time.deltaTime, ActionDuration);

        return false;
    }
} 

public class ScaleManager : IActionManager<ScaleAction>
{
    public static ScaleManager Instance;

    void Awake() {
        if(Instance == null){
            Instance = this;
            enabled = false;
            DontDestroyOnLoad(this);
        }else{
            Destroy(gameObject);
        }
    }

    public int ScaleBy(Transform toScale, Vector3 scaleBy, float time = 0, Action OnEnd = null){
        enabled = true;
        if(_actions.Count > activeActions){
            ScaleAction action       = _actions[activeActions];
            action.StartingScale     = toScale.localScale;
            action.ScaleBy           = scaleBy;
            action.ActionDuration    = time;
            action.ElapsedTime       = 0;
            action.Instance          = toScale;
            action.OnActionEnd       = OnEnd;
            action.ActionID          = actionCounter;
            activeActions++;
            return actionCounter++;
        }

        _actions.Add(
            new ScaleAction{
                StartingScale = toScale.localScale,
                ScaleBy = scaleBy,
                ActionDuration = time,
                ElapsedTime = 0,
                Instance = toScale,
                OnActionEnd = OnEnd,
                ActionID = actionCounter
            }
        );
        activeActions++;
        return actionCounter++;
    }

    #region interfaces
        public int ScaleAs(Transform toScale, Transform target, float time){
            return ScaleBy(toScale, target.localScale - toScale.localScale,  time , null);
        }

        public int ScaleAs(Transform toScale, Transform target, Action OnEnd){
            return ScaleBy(toScale, target.localScale - toScale.localScale,  0 , OnEnd);
        }

        public int ScaleAs(Transform toScale, Transform target, float time = 0, Action OnEnd = null){
            return ScaleBy(toScale, target.localScale - toScale.localScale,  time , OnEnd);
        }

        public int ScaleTo(Transform toScale, Vector3 scale, float time){
            return ScaleBy(toScale,  scale - toScale.localScale ,  time , null);
        }

        public int ScaleTo(Transform toScale, Vector3 scale, Action OnEnd){
            return ScaleBy(toScale, scale  - toScale.localScale,  0 , OnEnd);
        }

        public int ScaleTo(Transform toScale, Vector3 scale, float time, Action OnEnd){
            return ScaleBy(toScale, scale  - toScale.localScale,  time , OnEnd);
        }

    #endregion
}
