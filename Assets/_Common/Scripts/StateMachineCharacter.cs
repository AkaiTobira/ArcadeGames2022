
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;


public static class STATEMACHINE_CONSTS{
    public const float ANIMATION_FRAME_DURATION = 0.3f;
}


public abstract class SMC<T> : CMonoBehaviour
where T : System.Enum
{

    [Serializable]
    public class Animations{
        [SerializeField][NonReorderable] public Sprite[] FramesLeft;
        [SerializeField][NonReorderable] public Sprite[] FramesRight;
        [SerializeField][NonReorderable] public Sprite[] FramesBottom;
        [SerializeField][NonReorderable] public Sprite[] FramesTop;
        [SerializeField] public bool Looped = true;
        [SerializeField] public bool UseRightAsLeft = true;
    }

    [SerializeField] protected Image Graphicals;
    protected Animations[] _animations;
    [SerializeField] float _moveSpeed = 10;



    bool _stateChanged = false;
    protected T ActiveState;
    protected bool OverrideAnimationUpdate;
    NeighbourSide _side = NeighbourSide.NS_Right;

    float _elapsedTimeAnimation = 0f;
    int _animationCurrentFrame = 0;

    Vector3 _previousPosition;


    protected virtual void Update()
    {
        UpdateState_Internal();
        UpdateGraphics_Internal();
        ProcessSideUpdate();

        _stateChanged = false;
    }

    private void ProcessSideUpdate(){

        Vector3 positionChange = GetDirectionChange();
        if(positionChange.x == 0 && positionChange.y == 0){ return; }


        NeighbourSide side = _side;
        if(positionChange.y == 0){
            _side = (positionChange.x > 0) ? NeighbourSide.NS_Right : NeighbourSide.NS_Left;
        }else{
            _side = (positionChange.y > 0) ? NeighbourSide.NS_Top : NeighbourSide.NS_Bottom;
        }

        float scaleMultipler = 1;
        if(_animations[Convert.ToInt32(ActiveState)].UseRightAsLeft){
            scaleMultipler = ((_side == NeighbourSide.NS_Left) ? -1 : 1);
        }

        if(side != _side) SetAnimationFrame(0, ActiveState);
        

        Vector3 scale = Graphicals.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * scaleMultipler;
        Graphicals.transform.localScale = scale;

        _previousPosition = transform.position;
    }

    protected virtual Vector3 GetDirectionChange(){
        return transform.position - _previousPosition;
    }

#region "Animations"
    private void UpdateGraphics_Internal(){
        if(_stateChanged){
            SetNewAnimation();
            return;
        }

        _elapsedTimeAnimation -= Time.deltaTime;
        if(_elapsedTimeAnimation > 0) return;
        UpdateCurrentAnimation();
        _elapsedTimeAnimation += STATEMACHINE_CONSTS.ANIMATION_FRAME_DURATION;
    }

    protected virtual void CustomAnimationUpdate(){

    }

    protected void SetAnimationFrame(int frame, T state){
        _animationCurrentFrame = frame;

        int animationIndex = Convert.ToInt32(state);

        switch (_side) {
            case NeighbourSide.NS_Left:
                if(_animations[animationIndex].UseRightAsLeft){
                    Graphicals.sprite = _animations[animationIndex].FramesRight[_animationCurrentFrame];
                }else{
                    Graphicals.sprite = _animations[animationIndex].FramesLeft[_animationCurrentFrame];
                }
            break;
            case NeighbourSide.NS_Right:
                Graphicals.sprite = _animations[animationIndex].FramesRight[_animationCurrentFrame];
            break;
            case NeighbourSide.NS_Top:
                Graphicals.sprite = _animations[animationIndex].FramesTop[_animationCurrentFrame];
            break;
            case NeighbourSide.NS_Bottom:
                Graphicals.sprite = _animations[animationIndex].FramesBottom[_animationCurrentFrame];
            break;
        }
    }

    private void UpdateCurrentAnimation(){

        if(OverrideAnimationUpdate){
            CustomAnimationUpdate(); return;
        }

        int animationIndex = Convert.ToInt32(ActiveState);

        Animations animation = _animations[animationIndex];
        int animationLenght = 1;

        switch (_side) {
            case NeighbourSide.NS_Left:
                if(_animations[animationIndex].UseRightAsLeft){
                    animationLenght = animation.FramesRight.Length;
                }else{
                    animationLenght = animation.FramesLeft.Length;
                }
            break;
            case NeighbourSide.NS_Right:
                animationLenght = animation.FramesRight.Length;
            break;
            case NeighbourSide.NS_Top:
                animationLenght = animation.FramesTop.Length;
            break;
            case NeighbourSide.NS_Bottom:
                animationLenght = animation.FramesBottom.Length;
            break;
        }

        int nextFrame = _animationCurrentFrame;
        if(animation.Looped){
            nextFrame = (_animationCurrentFrame + 1) % animationLenght;
        }else{
            nextFrame = (nextFrame + 1 < animationLenght) ? nextFrame + 1: nextFrame;
        }

        SetAnimationFrame(nextFrame, ActiveState);
    }

    private void SetNewAnimation(){
        _elapsedTimeAnimation = STATEMACHINE_CONSTS.ANIMATION_FRAME_DURATION;
        UpdateCurrentAnimation();
    }
#endregion



#region "Inputs && Physics"
    private void UpdateState_Internal(){
        CheckTransitions_Internal();
        UpdateState();
    }

    private void CheckTransitions_Internal(){
        T current = ActiveState;
        ActiveState = CheckStateTransitions();
        _stateChanged = Convert.ToInt32(ActiveState) != Convert.ToInt32(current);

        if(_stateChanged){
            OnStateExit(current);
            OnStateEnter(ActiveState);
        }
    }

    protected abstract void OnStateEnter(T enteredState);
    protected abstract void OnStateExit(T exitedState);

    protected abstract void UpdateState();
    /*  
        switch (ActiveState) {
            case PlayerStates.Idle:  break;
            case PlayerStates.Move:  ProcessMove(_inputs); break;
            case PlayerStates.Dig:   ProvessMoveInDigging(_inputs); break;
            case PlayerStates.Dead:  break;
            case PlayerStates.Shoot: break;
        }
    */    


    protected abstract T CheckStateTransitions();
    /*
    
        switch (ActiveState) {
            case PlayerStates.Idle: return PlayerState.OtherState;
            case ...
        }
    */

    protected virtual void ProcessMove(Vector2 directions ){
        //ProcessMove_Horizontal(directions.x);
        //ProcessMove_Vertical(directions.y);
    }

    protected void ProcessMove_Horizontal(float horizontal){
        if(horizontal != 0) {
            transform.position += new Vector3(horizontal * _moveSpeed * Time.deltaTime, 0,0);
        }
    }

    protected void ProcessMove_Vertical(float vertical){
        if(vertical != 0){
            transform.position += new Vector3(0,vertical * _moveSpeed * Time.deltaTime,0);
        }
    }

    public NeighbourSide GetFacingDirection(){
        return _side;
    }


#endregion

    bool _isDisabled = false;
    protected void RequestDisable( float time ){
        if(!_isDisabled){
            TimersManager.Instance.FireAfter(time, ()=>{
                if(Guard.IsValid(this)) gameObject.SetActive(false);
            });
            _isDisabled = true;
        }
    }
}
