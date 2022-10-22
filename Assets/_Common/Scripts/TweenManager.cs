using System.Collections.Generic;
using System;
using UnityEngine;

public class TweenAction : IAction
{
    public Vector3      StartingPosition;
    public Vector3      TranslateBy;

    override public bool Process(){
        if(!Guard.IsValid(Instance)) return true;

        if(ActionDuration <= 0 || ElapsedTime >= ActionDuration){
            Instance.position = StartingPosition + TranslateBy;
            return true;
        }

        float timeCoef = ElapsedTime/ActionDuration;
        Instance.position = StartingPosition + TranslateBy * timeCoef;
        ElapsedTime = Mathf.Min(ElapsedTime + Time.deltaTime, ActionDuration);

        return false;
    }
} 

public class TweenManager : IActionManager<TweenAction>
{
    public static TweenManager Instance;

    void Awake() {
        if(Instance == null){
            Instance = this;
            enabled = false;
            DontDestroyOnLoad(this);
        }else{
            Destroy(gameObject);
        }
    }

    public void TweenBy(Transform toMove, Vector3 translation, float time = 0, Action OnEnd = null){
        enabled = true;
        if(_actions.Count > activeActions){
            TweenAction action       = _actions[activeActions];
            action.StartingPosition  = toMove.position;
            action.TranslateBy       = translation;
            action.ActionDuration    = time;
            action.ElapsedTime       = 0;
            action.Instance          = toMove;
            action.OnActionEnd       = OnEnd;
            activeActions++;
            return;
        }

        _actions.Add(
            new TweenAction{
                StartingPosition = toMove.position,
                TranslateBy = translation,
                ActionDuration = time,
                ElapsedTime = 0,
                Instance = toMove,
                OnActionEnd = OnEnd
            }
        );
        activeActions++;
    }

    #region interfaces
        public void TweenTo(Transform toMove, Transform target, float time){
            TweenBy(toMove, target.position - toMove.position,  time , null);
        }

        public void TweenTo(Transform toMove, Transform target, Action OnEnd){
            TweenBy(toMove, target.position - toMove.position,  0 , OnEnd);
        }

        public void TweenTo(Transform toMove, Transform target, float time = 0, Action OnEnd = null){
            TweenBy(toMove, target.position - toMove.position,  time , OnEnd);
        }

        public void TweenTo(Transform toMove, Vector2 target, float time){
            TweenBy(toMove, target,  time , null);
        }

        public void TweenTo(Transform toMove, Vector2 target, Action OnEnd){
            TweenBy(toMove, target,  0 , OnEnd);
        }

        public void TweenTo(Transform toMove, Vector3 target, float time = 0, Action OnEnd = null){
            TweenBy(toMove, target - toMove.position,  time, OnEnd);
        }

        public void TweenBy(Transform toMove, Vector3 translation, float time){
            TweenBy(toMove, translation,  time , null);
        }

        public void TweenBy(Transform toMove, Vector3 translation, Action OnEnd){
            TweenBy(toMove, translation,  0 , OnEnd);
        }
    #endregion
}
