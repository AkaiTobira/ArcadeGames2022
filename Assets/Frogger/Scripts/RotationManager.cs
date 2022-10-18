using System.Collections.Generic;
using System;
using UnityEngine;


public class RotationAction : IAction{
    public Vector3       StartingRotation;
    public Vector3       RotateBy;

    override public bool Process(){
        if(!Guard.IsValid(Instance)) return true;

        Vector3 positon = Instance.position;

        if(ActionDuration <= 0 || ElapsedTime >= ActionDuration){
            Vector3 rotation = StartingRotation + RotateBy;
            Instance.rotation = Quaternion.Euler(rotation);
            Instance.position = positon;
            return true;
        }

        float timeCoef = ElapsedTime/ActionDuration;
        Vector3 rotation2 = StartingRotation + RotateBy * timeCoef;
        Instance.rotation = Quaternion.Euler(rotation2);
        Instance.position = positon;
        ElapsedTime = Mathf.Min(ElapsedTime + Time.deltaTime, ActionDuration);
        return false;
    }
} 

public class RotationManager : IActionManager<RotationAction>
{
    public static RotationManager Instance;

    void Awake() {
        if(Instance == null){
            Instance = this;
            enabled = false;
            DontDestroyOnLoad(this);
        }else{
            Destroy(gameObject);
        }
    }

    public void RotateBy(Transform toMove, Vector3 rotation, float time = 0, Action OnEnd = null){
        enabled = true;

        Vector3 startingRotation = toMove.rotation.eulerAngles;
        startingRotation = new Vector3(
            Mathf.RoundToInt(startingRotation.x / 5.0f) * 5,
            Mathf.RoundToInt(startingRotation.y / 5.0f) * 5,
            Mathf.RoundToInt(startingRotation.z / 5.0f) * 5
        ) ;

        if(_actions.Count > activeActions){
            RotationAction action   = _actions[activeActions];
            action.Instance         = toMove;
            action.StartingRotation = startingRotation;
            action.RotateBy         = rotation;
            action.ActionDuration   = time;
            action.ElapsedTime      = 0;
            action.OnActionEnd      = OnEnd;
            activeActions++;
            return;
        }

        _actions.Add(
            new RotationAction{
                StartingRotation = startingRotation,
                RotateBy = rotation,
                ActionDuration = time,
                ElapsedTime = 0,
                Instance = toMove,
                OnActionEnd = OnEnd
            }
        );
        activeActions++;
    }

    #region interfaces
        public void RotateBy(Transform toMove, Vector3 rotation, float time){
            RotateBy(toMove, rotation,  time , null);
        }

        public void RotateBy(Transform toMove, Vector3 rotation, Action OnEnd){
            RotateBy(toMove, rotation,  0 , OnEnd);
        }

        public void RotateTo(Transform toMove, Vector3 targetRotation, Action OnEnd){
            RotateBy(toMove, new Vector3(),  0 , OnEnd);
        }

        public void RotateTo(Transform toMove, Vector3 targetRotation, float time){

            Vector3 startingRotation = toMove.rotation.eulerAngles;
            startingRotation = new Vector3(
                Mathf.RoundToInt(startingRotation.x / 5.0f) * 5,
                Mathf.RoundToInt(startingRotation.y / 5.0f) * 5,
                Mathf.RoundToInt(startingRotation.z / 5.0f) * 5
            );

            RotateBy(toMove, targetRotation - startingRotation,  time , null);
        }

    #endregion
}
