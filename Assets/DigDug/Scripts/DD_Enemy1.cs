using System.Collections.Generic;
using UnityEngine;
using ESM;
using System.Drawing;

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
        public const int MAX_PUMPING = 4;
        const float TIME_OF_PUMP_COLDOWN = 1f;
        const float SHADOWWALKS_TIME = 0.3f;

        [SerializeField] Transform _pumpPoint;
        [SerializeField] LayerMask _uiMaskLayer;
        [SerializeField] LayerMask _solidMaskLayer;
        [SerializeField] GameObject _attackBox;
        [SerializeField] float _distanceA;
        [SerializeField] float _distanceB;
        [SerializeField] float _distanceC = 0.9f;
        [SerializeField] float _shadowMovefactor = 0.5f;

        int _pumpingStacks = 0;

        float _stateDuration;
        float _elapsedPumpTime  = 0;
        float _ignoreDamage = 0.5f;
        float _doNothing = 0.5f;
        private Vector3 _targetPoint = new Vector3();


        DD_EnemyBrain brain = null;

        public float  GetTileDistanceMultipler(){ return _distanceC; }

        public float GetDistanceToWall(){ return _distanceB; }

        private void OnEnable() { Setup(); }

        public void Setup(){
            _stateDuration = 0;
            _pumpingStacks = 0;
            _ignoreDamage = 0.5f;
            _doNothing = 0.5f;
            _elapsedPumpTime  = 0;

            transform.position = transform.parent.position;
            _attackBox.SetActive(true);

            Debug.Log(transform.parent.name);

            DD_GameController.ActiveEnemies.Add(transform.parent.GetComponent<DD_BrickController>());

            ForceState(DD_EnemyStates.Idle, true);
        
            brain = new DD_EnemyBrain(
                new Dictionary<DD_EnemyActions, AI.FSM.IState<DD_EnemyActions>>(){
                    {DD_EnemyActions.Idle,         new DD_EnemyIdle(this, 0.5f)},//new DD_EnemyIdle()},
                    {DD_EnemyActions.Walking,      new DD_EnemyWalking(this, _uiMaskLayer, _solidMaskLayer)},//new DD_EnemyWalking()},
                    {DD_EnemyActions.ShadowMoving, new DD_EnemyShadowWalking(this, 1.5f)},//new DD_EnemyShadowWalking()},
                    {DD_EnemyActions.ToExit,       new DD_EnemyToExit(this, _uiMaskLayer)},//new DD_EnemyToExit()},
                    {DD_EnemyActions.AfterPlayer,  new DD_EnemyPlayerPursit(this, 7f, _uiMaskLayer)},//new DD_EnemyPlayerPursit()},
                    {DD_EnemyActions.RanAway,      new DD_EnemyRunedAway(this)}
                },
                DD_EnemyActions.Idle
            );
            _targetPoint = brain.GetNextPoint();
            RequestEnable();
        }


        public void TakeDamage(int amount, MonoBehaviour source){

            CMonoBehaviour cMono = source as CMonoBehaviour;
            Debug.Log(source.transform.parent.name + "/" + source.name + " " + transform.parent.name + "/" + name);
          //  if(Guard.IsValid(cMono)){
           //     Debug.Log(GetNameWithParent() + " -> Take Damage from " + cMono.GetNameWithParent());
            
                if(source.name.Contains("Rock") && _ignoreDamage < 0){
                    _pumpingStacks =  MAX_PUMPING - 1;
             //   }
            }
        }

        public int GetDamage(){
            Debug.Log("Enemy -> Damage Delivered");
            return 1;
        } 

        protected override void OnStateEnter(DD_EnemyStates enteredState)
        {
            switch(ActiveState){
                case DD_EnemyStates.Idle:
    //                _lookDirection = AnimationSide.Right;
                break;
                case DD_EnemyStates.Moving:
                break;
                case DD_EnemyStates.ShadowMoving:
                    
                    _stateDuration = SHADOWWALKS_TIME;
                break;
                case DD_EnemyStates.Dead:
                    _attackBox.SetActive(false);
                    RequestDisable(0.6f);
                    PointsCounter.Score += 1000;
                    AudioSystem.PlaySample("DigDug_EnemyDie");

                    DD_GameController.ActiveEnemies.Remove(transform.parent.GetComponent<DD_BrickController>());
                break;
            }
        }

        public Vector3 GetPumpingPoint(){ return _pumpPoint.position; }
        public bool CanBePumped() { return ActiveState != DD_EnemyStates.ShadowMoving && ActiveState != DD_EnemyStates.Dead; }

        protected override void OnStateExit(DD_EnemyStates exitedState){
            if(DD_EnemyStates.Hurt == exitedState){ OverrideAnimationUpdate = false; }
        }

        protected override float GetMoveModifier(){
            return (ActiveState == DD_EnemyStates.ShadowMoving)? _shadowMovefactor : 1f;
        }

        private void UpdateState_Move(bool inputMoveUpdate = true){
            ProcessInputsMove();

            if(hasRunAway()){ 
                DD_GameController.ActiveEnemies.Remove(transform.parent.GetComponent<DD_BrickController>());
                RequestDisable(0.1f); 
            }


      //      if(Guard.IsValid(_forceRunAway)){
        //        ProcessMove((_forceRunAway.transform.position - transform.position).normalized);
       //         if((_forceRunAway.transform.position - transform.position).magnitude < 1){
        //            RequestDisable(0.1f);
        //            _forceRunAway = null;
        //        }
     //       }else{
            //    if(inputMoveUpdate) ProcessInputsMove();
                UpdateMove();
           // }
        }

        protected override void UpdateMove(){

        //    if(_inputs.sqrMagnitude > 0){

                _direction = GetPoint(AnimationSide.Common) - (Vector2)transform.position;

//                Debug.Log(_direction + " " + Vector2.Distance(GetPoint(AnimationSide.Common), (Vector2)transform.position));

                ProcessMove( (_direction.normalized / _moveSpeed) * GetMoveModifier() * DD_GameController.SpeedMultiplier);
        //    }
        }

        protected override void UpdateState(){
            _doNothing -= Time.deltaTime;
            brain.Update();
            
            AddToDebugLog(brain.GetActiveState().ToString(), true);

            switch(ActiveState){
                case DD_EnemyStates.Idle: 
                    ProcessInputsMove();
                break;
                case DD_EnemyStates.Moving:
                    UpdateState_Move();
                break;
                case DD_EnemyStates.ShadowMoving :
                    UpdateState_Move();
                    _stateDuration -= Time.deltaTime;
                break;
                case DD_EnemyStates.Hurt :
                    OverrideAnimationUpdate = true;
                    if(_elapsedPumpTime <= 0){
                        _pumpingStacks -= 1;
                        if(_pumpingStacks >= 0){ SetAnimationFrame(_pumpingStacks, DD_EnemyStates.Hurt); }
                        _elapsedPumpTime = TIME_OF_PUMP_COLDOWN;
                    }
                    _elapsedPumpTime -= Time.deltaTime;
                break;
            }

            base.UpdateState();
        }

        public int GetPumpStacks(){ return _pumpingStacks;}
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
            
            Debug.Log(Guard.IsValid(_hits.Item1.collider) + " " +  Guard.IsValid(_hits.Item2.collider) + " : " + str);
            */
            return Guard.IsValid(_hits.Item1.collider) || Guard.IsValid(_hits.Item2.collider);
        }

        protected override DD_EnemyStates CheckStateTransitions()
        {
            if(_doNothing > 0) return ActiveState;

            switch(ActiveState){
                case DD_EnemyStates.Idle:
                    if(_pumpingStacks == MAX_PUMPING - 1) return DD_EnemyStates.Dead;
                    if(_pumpingStacks != 0) return DD_EnemyStates.Hurt;
                    if(_inputs.magnitude > 0) return DD_EnemyStates.Moving;
                break;
                case DD_EnemyStates.Moving:
                    if(_pumpingStacks == MAX_PUMPING - 1) return DD_EnemyStates.Dead;
                    if(_pumpingStacks != 0) return DD_EnemyStates.Hurt;
                    if(_inputs.magnitude <= 0) return DD_EnemyStates.Idle;
                    if(isShadowWalking()) return DD_EnemyStates.ShadowMoving;
                break;
                case DD_EnemyStates.ShadowMoving:
                    if(_pumpingStacks == MAX_PUMPING - 1) return DD_EnemyStates.Dead;
                    if(_pumpingStacks != 0) return DD_EnemyStates.Hurt;
                    if(_stateDuration <= 0) {
                        if(isShadowWalking() && _inputs.magnitude > 0) return DD_EnemyStates.ShadowMoving;
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

        private bool isShadowWalking(){

            return WillBeDigging() || brain.GetState().GetType() == typeof(DD_EnemyShadowWalking);
        }

        private bool hasRunAway(){

            return brain.GetState().GetType() == typeof(DD_EnemyRunedAway);
        }

        public bool IsDead(){ return ActiveState == DD_EnemyStates.Dead; }
        public bool CanBeShoot(){ return ActiveState != DD_EnemyStates.ShadowMoving; }

        public AnimationSide GetDirection(){ return GetFacingDirection();}

        protected override Vector2 GetPoint(AnimationSide side)
        {
            return _targetPoint;
        }


        private void ProcessInputsMove(){
            _ignoreDamage -= Time.deltaTime;

            _targetPoint = brain.GetNextPoint();
            AddToDebugLog(_targetPoint.ToString() + " " + transform.position);

            _inputs = _targetPoint - transform.position;
            if(Mathf.Abs(_inputs.x) < 0.0f) _inputs.x = 0;
            if(Mathf.Abs(_inputs.y) < 0.0f) _inputs.y = 0;

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

