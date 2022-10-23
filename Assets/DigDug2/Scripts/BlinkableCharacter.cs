
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;


public abstract class BlinkableCharacter<T> : StateMachineCharacter<T>
where T : System.Enum
{
    int _isInDangerousZone = 0;

    float _elapsedTimeBlinking  = 0f;
    int _blinkCurrentFrame = 0;

    protected T DeadState;


#region "On Terrraformed Ground"

    public bool IsDead(){
        return Convert.ToInt32(ActiveState) == Convert.ToInt32(DeadState);
    }

    private void ProcessBlink(){
        //lock at client request
        return;
        if(Convert.ToInt32(ActiveState) == Convert.ToInt32(DeadState)) return;

        if(_isInDangerousZone > 0){
            _elapsedTimeBlinking -= Time.deltaTime;
            if(_elapsedTimeBlinking < 0){
                _elapsedTimeBlinking += CONSTS.ANIMATION_ONE_BLINK_DURAION;
                _blinkCurrentFrame += 1;
            }

            Color newColor = Color.white;
            newColor.a = _elapsedTimeBlinking/CONSTS.ANIMATION_ONE_BLINK_DURAION;
            Graphicals.color = newColor;

        }else{
            Graphicals.color = Color.white;
        }
    }

    protected override void Update(){
        base.Update();
        ProcessBlink();
    }

    public bool IsBlinking(){
        return _isInDangerousZone >= 1;
    }

    public bool IsDeadByBlinking(int requiredToMeet){
        return _blinkCurrentFrame > requiredToMeet;
    }

    public virtual void SetupBlink(bool isInDangerousZone){
        _isInDangerousZone += (isInDangerousZone) ? 1 : -1;
        _isInDangerousZone = Mathf.Max(0, _isInDangerousZone); 
        _elapsedTimeBlinking = 0;
        _blinkCurrentFrame   = 0;
    }

#endregion




}