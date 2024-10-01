using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_HomingMissle : BS_Missle
{
    Transform missleTarget;
    [SerializeField] Transform _aligment;
    [SerializeField] float _duration;

    float _elapsedDuration;
    Vector3 _startingDirection;

    public void Setup(Vector3 direction, Transform target){
        missleTarget = target;
        _startingDirection = direction;
        base.Setup(direction);   
    }

    protected override void Update(){
        if(Guard.IsValid(missleTarget)){
            Vector2 distance = missleTarget.transform.position - transform.position;
            Vector3 currentDirection = distance.normalized;
//            matchingDirection = _forwardDirection - matchingDirection;

            _elapsedDuration += Time.deltaTime;
            if(_elapsedDuration < _duration){
                float percent = _elapsedDuration / _duration;
                _forwardDirection = (_startingDirection * (1.0f-percent)) + (currentDirection * percent);
            }else{
                _forwardDirection = currentDirection;
            }

            _forwardDirection = _forwardDirection.normalized;
            _aligment.transform.up = _forwardDirection;
        }

        base.Update();
    }
}
