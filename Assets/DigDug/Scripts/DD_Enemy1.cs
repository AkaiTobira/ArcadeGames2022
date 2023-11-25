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
        float _playerFollowTime = 0;
        float _playerRushTime = 50;
        float _ignoreDamage = 0.5f;
        float _doNothing = 0.5f;

        bool _cantStopThere = false;
        bool _isPursuingPlayer = false;

        AnimationSide _lookDirection;
        
        Transform _forceRunAway;
        
        private void OnEnable() { Setup(); }

        public void Setup(){
            _lookDirection = AnimationSide.Right; //GetFacingDirection();
            _stateDuration = 0;
            _pumpingStacks = 0;
            _ignoreDamage = 0.5f;
            _doNothing = 0.5f;
            _playerFollowTime = 0;
            _elapsedPumpTime  = 0;

            transform.position = transform.parent.position;
            _playerRushTime = Mathf.Max(50 - DD_BlocksManager.GetCurrentLevel(), 20);
            _attackBox.SetActive(true);

            _cantStopThere = false;
            _isPursuingPlayer = false;

            ForceState(DD_EnemyStates.Idle, true);
        
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
                    ProcessInputsMove();
                    _stateDuration = SHADOWWALKS_TIME;
                break;
                case DD_EnemyStates.Dead:
                    _attackBox.SetActive(false);
                    RequestDisable(0.6f);
                    PointsCounter.Score += 1000;
                    AudioSystem.PlaySample("DigDug_EnemyDie");

                    DD_GameController.NumberOfEnemies --;
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
            if(Guard.IsValid(_forceRunAway)){
                ProcessMove((_forceRunAway.transform.position - transform.position).normalized);
                if((_forceRunAway.transform.position - transform.position).magnitude < 1){
                    RequestDisable(0.1f);
                    _forceRunAway = null;
                }
            }else{
                if(inputMoveUpdate) ProcessInputsMove();
                UpdateMove();
            }
        }

        protected override void UpdateState(){
            _doNothing -= Time.deltaTime;
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
                    if(WillBeDigging()) return DD_EnemyStates.ShadowMoving;
                break;
                case DD_EnemyStates.ShadowMoving:
                    if(_pumpingStacks == MAX_PUMPING - 1) return DD_EnemyStates.Dead;
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

        public bool IsDead(){ return ActiveState == DD_EnemyStates.Dead; }
        public bool CanBeShoot(){ return ActiveState != DD_EnemyStates.ShadowMoving; }

        private bool IsFacingWall(AnimationSide side){
            Vector3 center = _pumpPoint.transform.position;

        //    Debug.DrawLine(center, center + (Vector3)DirectionToVector2(side) * _distanceA, UnityEngine.Color.green);
            RaycastHit2D[] hit2D2 = Physics2D.RaycastAll(center, DirectionToVector2(side), _distanceA, _solidMaskLayer);

         //   string a = hit2D2.Length.ToString()+ " : " + transform.parent.name+"/"+name +" : " + center + " " + side + "\n";
         //   for(int o = 0; o < hit2D2.Length; o++){
         //       RaycastHit2D raycast = hit2D2[o];
          //      a += raycast.collider.transform.parent.name + "/" + raycast.collider.name + ":" + raycast.point +  " " + DirectionToVector2(side) + " " + Vector2.Distance(raycast.point, center) + "\n";
           // }
           // Debug.Log(a);

            string raycastHits = hit2D2.Length.ToString() + " " + center+ "\n";
            for(int i = 0; i < hit2D2.Length; i++){
                RaycastHit2D raycast = hit2D2[i];

                //raycastHits += raycast.collider.transform.parent.name+"/"+raycast.collider.name + ":" + raycast.point + "::" + raycast.collider.transform.position +"\n ";
                if((raycast.collider.name.Contains("MoveBlock") || raycast.collider.name.Contains("Bricker")) && Vector2.Distance(raycast.point, center) < _distanceC){

                 //   raycastHits += transform.parent.name+"//"+name + " :" +side + "::" + raycast.point + " " + center + " "  + raycast.collider.transform.parent.name + "//" + raycast.collider.name + DirectionToVector2(side) + " " + Vector2.Distance(raycast.point, center);
                 //   Debug.Log(raycastHits);
                    return true;
                }
            }

          //  Debug.Log(raycastHits);
            return false;
        }



        private Vector3 GetNextPoint(){
//            Debug.Log(_lookDirection);
            _cantStopThere = !DD_NavMesh.CanStopThere(transform.position);
            if(_playerFollowTime > 0 || (_isPursuingPlayer && _cantStopThere)){ 
                return DD_NavMesh.GetNextPathPoint(transform.position, DD_Player3.Instance.transform.position); 
            }

            _isPursuingPlayer = false;
//            Debug.Log("First");
            Vector3 center = Graphicals.transform.position;
            bool selectNewDirection = IsFacingWall(_lookDirection);
            //Debug.Log("Need new direction : " + selectNewDirection);
            if(!selectNewDirection){
                Vector3 point = new Vector3();
                if(UpdateNavePoint_SawObjects(ref point)) return point;
            }

            if(_playerRushTime < 0){
                _playerRushTime = Mathf.Max(50 - DD_BlocksManager.GetCurrentLevel(), 20);
                _playerFollowTime = 6;
                _isPursuingPlayer = true;

                AudioSystem.PlaySample("DigDug_Danger");
            }

            if(Guard.IsValid(_forceRunAway)){ return _forceRunAway.transform.position;}
            if(selectNewDirection){ return UpdateNavePoint_PathBlocking(); }

            //Debug.Log("END" + " " + transform.position + (Vector3)DirectionToVector2(_lookDirection)*_distanceB);
            return DD_NavMesh.GetNextPathPoint(
                            transform.position, 
                            transform.position + (Vector3)DirectionToVector2(_lookDirection)*_distanceB);
        }

        private bool UpdateNavePoint_SawObjects(ref Vector3 endingPoint){
            Vector3 center = Graphicals.transform.position;
            RaycastHit2D[] hit2D = Physics2D.RaycastAll(center, DirectionToVector2(_lookDirection), _distanceA, _uiMaskLayer);
            for(int i = 0; i < hit2D.Length; i++){
                RaycastHit2D raycast = hit2D[i];
                Debug.DrawLine(raycast.point, center, (Vector2.Distance(raycast.point, center) < _distanceB) ? UnityEngine.Color.green : UnityEngine.Color.red);
                if(raycast.collider.tag.Contains("GameCon")){
                    if(_playerFollowTime <= 0){
                        _playerFollowTime = 6;
                        _isPursuingPlayer = true;
                        endingPoint = DD_NavMesh.GetNextPathPoint(transform.position, DD_Player3.Instance.transform.position);
                        return true;
                    }
                }else if(raycast.collider.name.Contains("Exit")){
                    if(Vector3.Distance(transform.position, raycast.collider.transform.position) < 3){
                        _forceRunAway = raycast.collider.transform;
              //          Debug.Log("Run away!");
                    };
                    endingPoint = raycast.collider.transform.position;
                    return true;
                }
            }

            return false;
        }


        private Vector3 UpdateNavePoint_PathBlocking(){
            Vector3 center = Graphicals.transform.position;
            List<AnimationSide> directionList = GetDirectionList();
         //   string listOfDirections = _lookDirection + " :: " + directionList[0] + directionList[1] + directionList[2];
         //   Debug.Log(listOfDirections);
            for(int i = 0; i < directionList.Count; i++){
                if(!IsFacingWall(directionList[i])){
                    _lookDirection = directionList[i];
                    return DD_NavMesh.GetNextPathPoint(
                        transform.position, 
                        transform.position + (Vector3)DirectionToVector2(_lookDirection)*_distanceB);
                }
            }
            return center;
        }

        private List<AnimationSide> GetDirectionList(){
            switch(_lookDirection){
                case AnimationSide.Right:
                case AnimationSide.Left:
                    return new List<AnimationSide>{
                        AnimationSide.Top, AnimationSide.Bottom, ReverseDirection(_lookDirection)
                    };
                case AnimationSide.Top:
                case AnimationSide.Bottom:
                    return new List<AnimationSide>{
                        AnimationSide.Right, AnimationSide.Left, ReverseDirection(_lookDirection)
                    };
            }

            return new List<AnimationSide> {_lookDirection};
        }

        private void ProcessInputsMove(){
            _playerFollowTime -= Time.deltaTime;
            _playerRushTime   -= Time.deltaTime;
            _ignoreDamage     -= Time.deltaTime;
            
            Vector3 nextNavPoint = GetNextPoint();
         //   Debug.Log("CALL" + nextNavPoint);

            _inputs = nextNavPoint - transform.position;
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

