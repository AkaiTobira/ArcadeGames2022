using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ESM;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using System;

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
        [SerializeField] LayerMask _uiMaskLayer;
        [SerializeField] float _distanceA;
        [SerializeField] float _distanceB;
        

        private const float SHADOWWALKS_TIME = 0.3f;

        const float TIME_OF_STARING_COLDOWN = 2f;
        const float TIME_OF_PUMP_COLDOWN = 1f;
        public const int MAX_PUMPING = 4;

        protected bool _shoot;
        private float _stateDuration;
        int _pumpingStacks = 0;
        float _elapsedPumpTime  = 0;
        AnimationSide _lookDirection;
        AnimationSide _lastLookDirection;
        List<AnimationSide> _lookDirectionQueue = new List<AnimationSide>();

        private static AnimationSide[] _possibleDirections = { 
    
            AnimationSide.Top, AnimationSide.Right, AnimationSide.Bottom, AnimationSide.Left };

        private void OnEnable() {
            _lookDirection = GetFacingDirection();
            _pumpingStacks = 0;
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

        float followPlayer = 0;

        private Vector3 GetNextPoint(){

            if(followPlayer > 0){
                return DD_NavMesh.GetNextPathPoint(transform.position, DD_Player3.Instance.transform.position);
            }

            Vector3 center = Graphicals.transform.position;
            
            RaycastHit2D[] hit2D = Physics2D.RaycastAll(center, DirectionToVector2(_lookDirection), _distanceA, _uiMaskLayer);;
            Debug.DrawLine(center, center + (Vector3)DirectionToVector2(_lookDirection) * _distanceA, Color.green);


            bool selectNewDirection = false;
            for(int i = 0; i < hit2D.Length; i++){
                RaycastHit2D raycast = hit2D[i];
                Debug.Log(_lookDirection + " " +raycast.collider.name + " " + raycast.collider.name.Contains("Brick") + " " + Vector2.Distance(raycast.collider.transform.position, center) + " "  + (Vector2.Distance(raycast.collider.transform.position, center) < _distanceB));
                Debug.DrawLine(raycast.point, center, (Vector2.Distance(raycast.point, center) < _distanceB) ? Color.green : Color.red);

                //convert distance to only dimensional check like (x1 - x2 < r or y1 - y2 < r)

            //    bool distance = Math.Abs(raycast.collider.transform.position.x - center.x )
                if(raycast.collider.name.Contains("Brick") && Vector2.Distance(raycast.point, center) < _distanceB){
                    selectNewDirection = true;
                    break;
                }else if(raycast.collider.tag.Contains("GameCon")){
                    if(followPlayer <= 0){
                        followPlayer = 6;
                        return DD_NavMesh.GetNextPathPoint(transform.position, DD_Player3.Instance.transform.position);
                    }
                }
                else if(raycast.collider.name.Contains("Exit")){
                    return raycast.collider.transform.position;
                }
            }

            if(selectNewDirection){
                AnimationSide currentAnimation = _lookDirection;
                _lookDirection = GetNextAnimationSide();
                Debug.Log(_lookDirection);
            }else{
                _lastLookDirection = _lookDirection;
                _lookDirectionQueue = new List<AnimationSide>();
                return DD_NavMesh.GetNextPathPoint(transform.position, transform.position + (Vector3)DirectionToVector2(_lookDirection)*_distanceA );
            }

            return center;
        }

        private AnimationSide GetNextAnimationSide(){
            if(_lookDirectionQueue.Count != 0){
                AnimationSide animationSide = _lookDirectionQueue[0];
                _lookDirectionQueue.RemoveAt(0);
                return animationSide;
            }
            if(_lookDirectionQueue.Count < 10){
                switch(_lastLookDirection){
                    case AnimationSide.Right:
                    case AnimationSide.Left:
                        _lookDirectionQueue.Add(AnimationSide.Top);
                        _lookDirectionQueue.Add(AnimationSide.Bottom);
                    break;
                    case AnimationSide.Top:
                    case AnimationSide.Bottom:
                        _lookDirectionQueue.Add(AnimationSide.Right);
                        _lookDirectionQueue.Add(AnimationSide.Left);
                    break;
                }

                _lookDirectionQueue.Add(ReverseDirection(_lastLookDirection));
            }

            return _lookDirection;
        }

        private AnimationSide GetRandomAnimationSide(){
            int t = UnityEngine.Random.Range(0, 4);
            return _possibleDirections[t];
        }


        private void ProcessInputsMove(){
                followPlayer -= Time.deltaTime;

    //        _inputs.x = Input.GetAxisRaw("Horizontal");
    //        _inputs.y = Input.GetAxisRaw("Vertical");

    //        _inputs = (DD_Player3.Instance.transform.position - transform.position).normalized;
    //        if(Mathf.Abs(_inputs.x) < 0.2f) _inputs.x = 0;
    //        if(Mathf.Abs(_inputs.y) < 0.2f) _inputs.y = 0;

    //        Vector3 aa = DD_NavMesh.GetNextPathPoint(transform.position, DD_Player3.Instance.transform.position);
    //        Debug.Log(aa + " " + transform.position + " : " + (aa - transform.position));
            Vector3 aa = GetNextPoint();
            Debug.Log("TargetPoint " + aa);
            _inputs = aa - transform.position;
        //    _shoot    = Input.GetKeyDown(KeyCode.N);
            if(Mathf.Abs(_inputs.x) < 0.4f) _inputs.x = 0;
            if(Mathf.Abs(_inputs.y) < 0.4f) _inputs.y = 0;

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

