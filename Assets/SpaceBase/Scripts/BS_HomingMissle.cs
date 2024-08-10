using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_HomingMissle : BS_Missle
{
    Transform missleTarget;


    public void Setup(Vector3 direction, Transform target){
        missleTarget = target;
        base.Setup(direction);
    }

    protected override void Update(){
        if(Guard.IsValid(missleTarget)){
            Vector3 matchingDirection = (missleTarget.transform.position - transform.position).normalized;
            matchingDirection = _forwardDirection - matchingDirection;

            if(Mathf.Abs(_forwardDirection.x - matchingDirection.x) > 0.01f) 
                _forwardDirection.x += _forwardDirection.x - matchingDirection.x * 0.02f;
            if(Mathf.Abs(_forwardDirection.y - matchingDirection.y) > 0.01f) 
                _forwardDirection.y += _forwardDirection.y - matchingDirection.y * 0.02f;
            _forwardDirection = _forwardDirection.normalized;
        }

        base.Update();
    }


}
