using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ESM;

namespace DigDug{
    public enum DD_EnemyStates{
        Idle,
        Moving,
        ShadowMoving,
        Hurt,
        Dead,
    }

    public class DD_Enemy1 : DD_MoveCore<DD_EnemyStates>, ITakeDamage, IDealDamage, IPumpableEnemy
    {
        [SerializeField] Transform _pumpPoint;

        private const float SHADOWWALKS_TIME = 0.3f;

        const float TIME_OF_STARING_COLDOWN = 2f;
        const float TIME_OF_PUMP_COLDOWN = 1f;
        public const int MAX_PUMPING = 4;

        protected bool _shoot;
        private float _stateDuration;
        int _pumpingStacks = 0;
        float _elapsedPumpTime  = 0;

        private void OnEnable() {
            
        }

        public void TakeDamage(int amount, MonoBehaviour source){

        }

        public int GetDamage(){
            Debug.Log("Enemy -> Damage Delivered");
            return 1;
        } 

        protected override void OnStateEnter(DD_EnemyStates enteredState)
        {
            switch(ActiveState){
                case DD_EnemyStates.Idle:
                break;
                case DD_EnemyStates.Moving:
                break;
                case DD_EnemyStates.ShadowMoving:
                    ProcessInputsMove();
                    _stateDuration = SHADOWWALKS_TIME;
                break;
                case DD_EnemyStates.Dead:
                    RequestDisable(1.0f);
                break;
            }
        }

        public Vector3 GetPumpingPoint(){ return _pumpPoint.position; }
        public bool CanBePumped() { return ActiveState != DD_EnemyStates.ShadowMoving || ActiveState != DD_EnemyStates.Dead; }

        protected override void OnStateExit(DD_EnemyStates exitedState){

        //    if(exitedState == DD_EnemyStates.ShadowMoving){
        //        _lastMoveDirection = AnimationSide.Common;
        //    }

        }

        protected override float GetMoveModifier()
        {
            return ((ActiveState == DD_EnemyStates.ShadowMoving)? 0.2f : 1f);
        }

        protected override void UpdateState()
        {
            switch(ActiveState){
                case DD_EnemyStates.Idle: 
                    ProcessInputsMove();
                break;
                case DD_EnemyStates.Moving : 
                    ProcessInputsMove();
                    UpdateMove();
                break;
                case DD_EnemyStates.ShadowMoving :
                    UpdateMove();
                    _stateDuration -= Time.deltaTime;
                break;
                case DD_EnemyStates.Hurt :
                    OverrideAnimationUpdate = true;
                    if(_elapsedPumpTime <= 0){
                        _pumpingStacks -= 1;
                        if(_pumpingStacks >= 0){
                            SetAnimationFrame(_pumpingStacks, DD_EnemyStates.Hurt);
                        }
                        _elapsedPumpTime = TIME_OF_PUMP_COLDOWN;
                    }
                    _elapsedPumpTime -= Time.deltaTime;
                break;
            }

            base.UpdateState();
        }   

        public int GetPumpStacks(){
            return _pumpingStacks;
        }

        public void Pump(){
            if(_pumpingStacks > MAX_PUMPING) return;
            
            SetAnimationFrame(_pumpingStacks, DD_EnemyStates.Hurt);
            _pumpingStacks += 1;
            AudioSystem.Instance.PlayEffect("DigDug_Pump", 1);

            if(_pumpingStacks >= MAX_PUMPING) Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.EnemyHasBeenMurdered, this));
            _elapsedPumpTime = TIME_OF_PUMP_COLDOWN;
            OverrideAnimationUpdate = true;
        }

        private bool WillBeDigging(){

            int i1 = 0, i2 = 0;

            switch(GetFacingDirection()){
                case ESM.AnimationSide.Left:
                case ESM.AnimationSide.Right:   i1 = 1; i2 = 2; break;
                case ESM.AnimationSide.Bottom : i1 = 0; i2 = 1; break;
                case ESM.AnimationSide.Top :    i1 = 2; i2 = 3; break;
            }


            (RaycastHit2D, RaycastHit2D) _hits = GetHits(i1, i2, _direction.normalized);

            /*
            string str = "";
            if(Guard.IsValid(_hits.Item1.collider)){
                str += _hits.Item1.collider.name + " ";
            }
            if(Guard.IsValid(_hits.Item2.collider)){
                str += _hits.Item2.collider.name;
            }
            */
        //    Debug.Log(Guard.IsValid(_hits.Item1.collider) + " " +  Guard.IsValid(_hits.Item2.collider) + " : " + str);

            return Guard.IsValid(_hits.Item1.collider) || Guard.IsValid(_hits.Item2.collider);
        }

        protected override DD_EnemyStates CheckStateTransitions()
        {
            switch(ActiveState){
                case DD_EnemyStates.Idle:
                    if(_pumpingStacks != 0) return DD_EnemyStates.Hurt;
                    if(_inputs.magnitude > 0) return DD_EnemyStates.Moving;
                break;
                case DD_EnemyStates.Moving:
                    if(_pumpingStacks != 0) return DD_EnemyStates.Hurt;
                    if(_inputs.magnitude <= 0) return DD_EnemyStates.Idle;
                    if(WillBeDigging()) return DD_EnemyStates.ShadowMoving;
                break;
                case DD_EnemyStates.ShadowMoving:
                    if(_pumpingStacks != 0) return DD_EnemyStates.Hurt;
                    if(_stateDuration <= 0) {
                        if(WillBeDigging() && _inputs.magnitude > 0) return DD_EnemyStates.ShadowMoving;
                        return DD_EnemyStates.Moving;
                    }
                break;
                case DD_EnemyStates.Hurt:
                                //if(IsDeadByBlinking(TIMES_TO_DEAD_BY_TILE)) return EnemyStates.Dead;
                    if(_pumpingStacks == MAX_PUMPING - 1) return DD_EnemyStates.Dead;
                    if(_pumpingStacks == 0 && _elapsedPumpTime <= 0) return DD_EnemyStates.Idle;
                break;
                case DD_EnemyStates.Dead:
                break;
            }

            return ActiveState;
        }

        public bool IsDead(){
            return ActiveState == DD_EnemyStates.Dead;
        }

        public bool CanBeShoot(){
            return ActiveState != DD_EnemyStates.ShadowMoving;
        }

        private void ProcessInputsMove(){


    //        _inputs.x = Input.GetAxisRaw("Horizontal");
    //        _inputs.y = Input.GetAxisRaw("Vertical");

    //        _inputs = (DD_Player3.Instance.transform.position - transform.position).normalized;
    //        if(Mathf.Abs(_inputs.x) < 0.2f) _inputs.x = 0;
    //        if(Mathf.Abs(_inputs.y) < 0.2f) _inputs.y = 0;

            Vector3 aa = DD_NavMesh.GetNextPathPoint(transform.position, DD_Player3.Instance.transform.position);
    //        Debug.Log(aa + " " + transform.position + " : " + (aa - transform.position));
            _inputs = aa - transform.position;
        //    _shoot    = Input.GetKeyDown(KeyCode.N);
            if(Mathf.Abs(_inputs.x) < 0.5f) _inputs.x = 0;
            if(Mathf.Abs(_inputs.y) < 0.5f) _inputs.y = 0;

        

            switch(_pressedDirection){
                case AnimationSide.Common:
                    if(_inputs.x < 0) _pressedDirection = AnimationSide.Left;
                    if(_inputs.x > 0) _pressedDirection = AnimationSide.Right;
                    if(_inputs.y > 0) _pressedDirection = AnimationSide.Top;
                    if(_inputs.y < 0) _pressedDirection = AnimationSide.Bottom;
                return;
                case AnimationSide.Left:
                    if(_inputs.x == 0) _pressedDirection = AnimationSide.Common;
                    if(_inputs.x > 0)  _pressedDirection = AnimationSide.Right;
                return;
                case AnimationSide.Right:
                    if(_inputs.x == 0) _pressedDirection = AnimationSide.Common;
                    if(_inputs.x < 0)  _pressedDirection = AnimationSide.Left;
                return;
                case AnimationSide.Top:
                    if(_inputs.y == 0) _pressedDirection = AnimationSide.Common;
                    if(_inputs.y < 0)  _pressedDirection = AnimationSide.Bottom;
                return;
                case AnimationSide.Bottom:
                    if(_inputs.y == 0) _pressedDirection = AnimationSide.Common;
                    if(_inputs.y > 0)  _pressedDirection = AnimationSide.Top;
                return;
            }
        }
    }
}

