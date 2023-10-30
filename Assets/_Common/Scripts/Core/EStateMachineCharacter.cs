
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;


namespace ESM{

    public enum AnimationSide{
        Common, // Common for all directions 
        Left,
        Right,
        Top,
        Bottom,
        LeftTop,
        LeftBottom,
        RightTop,
        RightBottom
    }


    //State Machine Animations Unit -> 4 direction
    [Serializable]
    public class SMA<StateType>
    where StateType : System.Enum
    {

        [Serializable] 
        protected class DirectionalAnimation{
            [SerializeField][NonReorderable] public AnimationSide Side;
            [SerializeField][NonReorderable] public Sprite[] Frames;
        }

        [SerializeField] public StateType State;
        [SerializeField][NonReorderable] protected DirectionalAnimation[] Sprites;
        [SerializeField] public bool Looped = true;
        [SerializeField] public bool UseRightAsLeft = true;

        private Dictionary<AnimationSide, Sprite[]> _hashedSprites = new Dictionary<AnimationSide, Sprite[]>();

        public Sprite[] GetSprites(AnimationSide side){
            if(_hashedSprites.TryGetValue(AnimationSide.Common, out Sprite[] value)){
                return value;
            }

            if(UseRightAsLeft && side == AnimationSide.Left)       side = AnimationSide.Right;
            if(UseRightAsLeft && side == AnimationSide.LeftBottom) side = AnimationSide.RightBottom;
            if(UseRightAsLeft && side == AnimationSide.LeftTop)    side = AnimationSide.RightTop;

            if(_hashedSprites.TryGetValue(side, out value )){
                return value;
            }

            for(int i = 0; i < Sprites.Length; i++) {
                DirectionalAnimation animation = Sprites[i];
                if(animation.Side == AnimationSide.Common){
                    _hashedSprites[AnimationSide.Common] = animation.Frames;
                    return animation.Frames;
                }
                
                if(animation.Side == side){
                    _hashedSprites[side] = animation.Frames;
                    return animation.Frames;
                }
            }

            return new Sprite[0];
        }

        public virtual AnimationSide GetSide(Vector3 positionChange, AnimationSide side){
            if( Mathf.Abs(positionChange.x) <= 0.01f && Mathf.Abs(positionChange.y) <= 0.01f) return side;
            if( Mathf.Abs(positionChange.x) > 0.01f)
                return (positionChange.x > 0) ? AnimationSide.Right : AnimationSide.Left;
            
            return (positionChange.y > 0) ? AnimationSide.Top : AnimationSide.Bottom;
        } 
    }

    //State Machine Animation Unity -> 8 dimensions version;
    [Serializable]
    public class SMA_8D<StateType> : SMA<StateType>
    where StateType : System.Enum
    {

        public override AnimationSide GetSide(Vector3 positionChange, AnimationSide side){
            if(positionChange.y == 0){
                if(positionChange.x == 0) return side;
                return (positionChange.x > 0) ? AnimationSide.Right : AnimationSide.Left;
            }else if(positionChange.x == 0){
                if(positionChange.y == 0) return side;
                return (positionChange.y > 0) ? AnimationSide.Top : AnimationSide.Bottom;
            }
            
            return (positionChange.x > 0) ?
                        ((positionChange.y > 0) ? AnimationSide.RightTop : AnimationSide.RightBottom) :
                        ((positionChange.y > 0) ? AnimationSide.LeftTop  : AnimationSide.LeftBottom) ;
        } 
    }

    //State Machine Animation Unity -> 2 dimensions version;
    [Serializable]
    public class SMA_2D<StateType> : SMA<StateType>
    where StateType : System.Enum
    {
        public override AnimationSide GetSide(Vector3 positionChange, AnimationSide side){
            if(positionChange.x < 0) return AnimationSide.Left;
            if(positionChange.x > 0) return AnimationSide.Right;
            return side;
        } 
    }

    [Serializable]
    public class SMA_1D<StateType> : SMA<StateType>
    where StateType : System.Enum
    {
        public override AnimationSide GetSide(Vector3 positionChange, AnimationSide side){
            return AnimationSide.Top;
        } 
    }

    // public interfaces
    public abstract class SMC_1D<StateType> :
    SMC<StateType, SMA_1D<StateType>>
        where StateType : System.Enum
    {};

    public abstract class SMC_2D<StateType> :
    SMC<StateType, SMA_2D<StateType>>
        where StateType : System.Enum
    {};


    public abstract class SMC_4D<StateType> : 
        SMC<StateType, SMA<StateType>>
        where StateType : System.Enum
    {};

    public abstract class SMC_8D<StateType> : 
        SMC<StateType, SMA_8D<StateType>>
        where StateType : System.Enum
    {};


    // base for public interface
    public abstract class SMC<StateType, AnimationType> : CMonoBehaviour
    where StateType : System.Enum
    where AnimationType : SMA<StateType>
    {

        [SerializeField][NonReorderable] protected Image Graphicals;
        [SerializeField] private float _animationFrameDuration = 0.3f;
        [SerializeField][NonReorderable] protected AnimationType[] _animations;
        [SerializeField][NonReorderable] protected float _moveSpeed = 10;
        [SerializeField] bool _mirrorSprites = false;
        [SerializeField] bool _ignoreSpriteDirectionClear = false;

        bool _stateChanged = false;
        protected StateType ActiveState;
        protected bool OverrideAnimationUpdate;
        AnimationSide _side = AnimationSide.Right;

        float _elapsedTimeAnimation = 0f;
        int _animationCurrentFrame = 0;

        Vector3 _previousPosition;


        protected virtual void Update()
        {
            UpdateState_Internal();
            UpdateGraphics_Internal();

            ProcessSideUpdate(GetDirectionChange());
            _previousPosition = transform.position;

            _stateChanged = false;
        }

        private void ProcessSideUpdate(Vector3 positionChange){
            if(positionChange.x == 0 && positionChange.y == 0){ return; }

            AnimationSide side = _side;
            _side = _animations[Convert.ToInt32(ActiveState)].GetSide(positionChange, _side);
            if(side != _side) SetAnimationFrame(0, ActiveState);

            SetDirectionScale();
        }

        private void SetDirectionScale(){
            if(_ignoreSpriteDirectionClear && 
                (_side == AnimationSide.Top ||
                _side == AnimationSide.Bottom || 
                _side == AnimationSide.Common)) return;

            Vector3 scale = Graphicals.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * GetScaleMultiplier();
            Graphicals.transform.localScale = scale;
        }

        private float GetScaleMultiplier(){
            float scaleMultipler = 1;
            if(_animations[Convert.ToInt32(ActiveState)].UseRightAsLeft){
                scaleMultipler = (
                    (_side == AnimationSide.Left) || 
                    (_side == AnimationSide.LeftTop) || 
                    (_side == AnimationSide.LeftBottom)
                    ? -1 
                    :  1);
            }
            if(_mirrorSprites) scaleMultipler *= -1;
            return scaleMultipler;
        }

        protected virtual Vector3 GetDirectionChange(){ return transform.position - _previousPosition;}

    #region "Animations"
        private void UpdateGraphics_Internal(){
            if(_stateChanged){
                SetNewAnimation();
                return;
            }

            _elapsedTimeAnimation -= Time.deltaTime;
            if(_elapsedTimeAnimation > 0) return;
            UpdateCurrentAnimation();
            _elapsedTimeAnimation += _animationFrameDuration;
        }

        protected virtual void CustomAnimationUpdate(){}

        protected void SetAnimationFrame(int frame, StateType state){
            _animationCurrentFrame = frame;

            int animationIndex = Convert.ToInt32(state);

            Sprite[] spritesheet = _animations[animationIndex].GetSprites(_side);
            if(spritesheet.Length > 0){
                Graphicals.sprite = spritesheet[_animationCurrentFrame];
            }else{
                Debug.LogError("No animation spritesheet for " + animationIndex + " _side " + _side );
            }
        }

        private void UpdateCurrentAnimation(){

            if(OverrideAnimationUpdate){
                CustomAnimationUpdate(); return;
            }

            int animationIndex = Convert.ToInt32(ActiveState);

            AnimationType animation = _animations[animationIndex];
            int animationLenght = 1;

            Sprite[] spritesheet = _animations[animationIndex].GetSprites(_side);

            if(spritesheet.Length > 0){
                animationLenght = spritesheet.Length;

                int nextFrame = _animationCurrentFrame;
                if(animation.Looped){
                    nextFrame = (_animationCurrentFrame + 1) % animationLenght;
                }else{
                    nextFrame = (nextFrame + 1 < animationLenght) ? nextFrame + 1: nextFrame;
                }
                SetAnimationFrame(nextFrame, ActiveState);
            }else{
                Debug.LogError("No animation spritesheet for " + ActiveState.ToString() + " _side " + _side );
            }
        }

        private void SetNewAnimation(){
            _elapsedTimeAnimation  = _animationFrameDuration;
            _animationCurrentFrame = -1;
            UpdateCurrentAnimation();
        }
    #endregion



    #region "Inputs && Physics"
        private void UpdateState_Internal(){
            CheckTransitions_Internal();
            UpdateState();
        }

        private void CheckTransitions_Internal(){
            StateType current = ActiveState;
            ActiveState = CheckStateTransitions();
            _stateChanged = Convert.ToInt32(ActiveState) != Convert.ToInt32(current);

            if(_stateChanged){
                OnStateExit(current);
                OnStateEnter(ActiveState);
            }
        }

        protected void ForceState(StateType state, bool ignoreCurrentState){
            if(!ignoreCurrentState) OnStateExit(ActiveState);

            ActiveState = state;
            OnStateEnter(state);
        }

        protected abstract void OnStateEnter(StateType enteredState);
        protected abstract void OnStateExit(StateType exitedState);

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


        protected abstract StateType CheckStateTransitions();
        /*
        
            switch (ActiveState) {
                case PlayerStates.Idle: return PlayerState.OtherState;
                case ...
            }
        */

        protected virtual void ProcessMove(Vector2 directions ){
            ProcessMove_Horizontal(directions.x * _moveSpeed);
            ProcessMove_Vertical(directions.y * _moveSpeed);
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

        public AnimationSide GetFacingDirection(){ return _side; }

        protected Vector2 DirectionToVector2(AnimationSide side){
            switch(side){
                case AnimationSide.Common: return Vector2.zero;
                case AnimationSide.Left: return Vector2.left;
                case AnimationSide.Right: return Vector2.right;
                case AnimationSide.Top: return Vector2.up;
                case AnimationSide.Bottom: return Vector2.down;
                case AnimationSide.LeftTop: return new Vector2(-1, 1);
                case AnimationSide.RightBottom: return new Vector2(1, 1);
                case AnimationSide.LeftBottom: return new Vector2(-1, -1);
                case AnimationSide.RightTop: return new Vector2(1, 1);
            }

            return Vector2.zero;
        }

        protected AnimationSide ReverseDirection(AnimationSide side){
            switch(side){
                case AnimationSide.Common: return AnimationSide.Common;
                case AnimationSide.Left: return AnimationSide.Right;
                case AnimationSide.Right: return AnimationSide.Left;
                case AnimationSide.Top: return AnimationSide.Top;
                case AnimationSide.Bottom: return AnimationSide.Bottom;
                case AnimationSide.LeftTop: return AnimationSide.RightTop;
                case AnimationSide.RightBottom: return AnimationSide.LeftBottom;
                case AnimationSide.LeftBottom: return AnimationSide.RightBottom;
                case AnimationSide.RightTop: return AnimationSide.LeftBottom;
            }
            return side;
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

        protected void RequestDestroy(float time){
            if(!_isDisabled){
                TimersManager.Instance.FireAfter(time, ()=>{
                    if(Guard.IsValid(this)) Destroy(gameObject);
                });
                _isDisabled = true;
            }
        }

        protected void RequestEnable(){ 
            if(Guard.IsValid(this)) gameObject.SetActive(true);
            _isDisabled = false; 
        }
    }
}