using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ESM;

namespace DigDug{
    public abstract class DD_MoveCore<T> : ESM.SMC_4D<T>
    where T : System.Enum
    {
        [SerializeField] Transform[] rayPoints;
        [SerializeField] LayerMask   _blockslayerMask;
        [SerializeField] Transform[] _debugPoints;

        protected Vector2 _direction = new Vector2();
        bool _keepDirection = false;
        Vector2[,] _points = new Vector2[3,3];
        protected Vector2 _inputs = new Vector2();
        protected AnimationSide _pressedDirection  = AnimationSide.Common;
        protected AnimationSide _lastMoveDirection = AnimationSide.Common;

        protected override void UpdateState(){
            if(_debugPoints.Length == 0) return; 
            for(int i = 0; i < 3; i++) {
                for(int j = 0; j < 3; j++) {
                    _debugPoints[i * 3 + j].position = _points[i,j];
                }
            }
        }

        protected void CalculateDirections(){

            if(_lastMoveDirection != _pressedDirection){
                if(AreAligned(_pressedDirection, _lastMoveDirection)){
                    _lastMoveDirection = _pressedDirection;
                }else{
                    _keepDirection = true;
                }
            }

            if(_keepDirection){
                if(Vector2.SqrMagnitude(_points[1,1] - (Vector2)transform.position) < 0.1f ){
                    _keepDirection = false;
                    _lastMoveDirection = _pressedDirection;
                }
            }

            Vector2 movePoint = GetPoint(_lastMoveDirection);
            Vector2 moveDistance = movePoint - (Vector2)transform.position;
            if(Vector2.SqrMagnitude(moveDistance) < 0.01f) _direction = Vector2.zero;
            else{ _direction = moveDistance.normalized; }
        }

        protected (RaycastHit2D, RaycastHit2D) GetHits(int rayIndex1, int rayIndex2, Vector2 direction){
            float distance = 0.075f;
            Debug.DrawLine(rayPoints[rayIndex1].position, rayPoints[rayIndex1].position + ((Vector3)direction * distance), Color.magenta);
            Debug.DrawLine(rayPoints[rayIndex2].position, rayPoints[rayIndex2].position + ((Vector3)direction * distance), Color.magenta);



            return (Physics2D.Raycast(
                        rayPoints[rayIndex1].position,
                        direction, 
                        distance,
                        _blockslayerMask), 
                    Physics2D.Raycast(
                        rayPoints[rayIndex2].position,
                        direction, 
                        distance,
                        _blockslayerMask));
        }

        protected void UpdateMove(){
            FillPoints();
            if(_inputs.sqrMagnitude > 0){
                CalculateDirections();

                ProcessMove( (_direction.normalized / _moveSpeed) * GetMoveModifier());
            }
        }

        protected virtual float GetMoveModifier(){
            return 1f;
        }

        private void FillPoints(){
            const float distance = 1.3f;

            Vector2[] horizonatl = new Vector2[]{
                DD_Path.GetClosestHorizontalPoint(transform.position - new Vector3(distance,0)),
                DD_Path.GetClosestHorizontalPoint(transform.position),
                DD_Path.GetClosestHorizontalPoint(transform.position + new Vector3(distance,0)),
            };

            Vector2[] vertialcs = new Vector2[]{
                DD_Path.GetClosestVerticalPoint(transform.position - new Vector3(0,distance)),
                DD_Path.GetClosestVerticalPoint(transform.position),
                DD_Path.GetClosestVerticalPoint(transform.position + new Vector3(0,distance)),
            };

            for(int i = 0; i < horizonatl.Length; i++) {
                for(int j = 0; j < vertialcs.Length; j++){
                    _points[i,j].x = horizonatl[i].x;
                    _points[i,j].y = vertialcs[j].y;
                }
            }
        }
        private bool AreAligned(ESM.AnimationSide _pressed, ESM.AnimationSide _last){
            switch(_pressed){
                case ESM.AnimationSide.Common:
                    switch(_last){
                        case ESM.AnimationSide.Common: return true;
                        default: return false;
                    }
                case ESM.AnimationSide.Bottom:
                    switch(_last){
                        case ESM.AnimationSide.Top: return true;
                        default: return false;
                    }
                case ESM.AnimationSide.Top:
                    switch(_last){
                        case ESM.AnimationSide.Bottom: return true;
                        default: return false;
                    }
                case ESM.AnimationSide.Left:
                    switch(_last){
                        case ESM.AnimationSide.Right: return true;
                        default: return false;
                    }
                case ESM.AnimationSide.Right:
                    switch(_last){
                        case ESM.AnimationSide.Left: return true;
                        default: return false;
                    }
            }
            return false;
        }
        private Vector2 GetPoint(ESM.AnimationSide side){
            switch(side){
                case ESM.AnimationSide.Common: return _points[1,1];
                case ESM.AnimationSide.Bottom: return _points[1,0];
                case ESM.AnimationSide.Top:    return _points[1,2];
                case ESM.AnimationSide.Left:   return _points[0,1];
                case ESM.AnimationSide.Right:  return _points[2,1];
            }

            return _points[1,1];
        }
    }

    public interface IPumpableEnemy{
        bool IsDead();
        Vector3 GetPumpingPoint();
        void Pump();
        int GetPumpStacks();
        bool CanBePumped();
    }


    public class DD_Player3 : DD_MoveCore<DD_PlayerStates>, ITakeDamage
    {
        private const float DIGGING_TIME = 0.3f;

        public static DD_Player3 Instance;

        const float SHOT_ACTION_LANDED_COLDOWN = 1.0f;
        const float SHOT_ACTION_NONLANDED_COLDOWN = 0.5f;
        const float SHOT_PUMP_ACTION_COLDOWN = 0.5f;
        const float SHOT_LANDING_TIME = 0.5f;
        const float DISTANCE_OF_SHOOT = 2.5f;
        const int MAX_PUMPING = 3;

        [SerializeField] LineRenderer _shootLineRenderer;
        [SerializeField] LineRenderer _landedShootLineRendrer;

        [SerializeField] LayerMask _enemyLayer;

        bool _shootingRequirementsMeet = false;
        bool _shootingFailed           = false;
        float _shootingTimeColdown = 0;
        float _shootingTimeElapsed = 0;
        float _lineShootingTimeElapsed = 0;
        Vector3 _landingPoint = new Vector3();
        Vector3 _startingPoint = new Vector3();
        IPumpableEnemy _pumpableEnemy = null;



        bool _lockMoving = false;
        bool _lockRotation = false;
        protected bool _shoot;
        private float _stateDuration;

        private void Awake() { Instance = this; }

        protected override void OnStateEnter(DD_PlayerStates enteredState)
        {
            switch(ActiveState){
                case DD_PlayerStates.Idle:
                break;
                case DD_PlayerStates.Moving:
                break;
                case DD_PlayerStates.Digging:
                    ProcessInputsMove();
                    _stateDuration = DIGGING_TIME;
                break;
            }
        }

        public void TakeDamage(int amount, MonoBehaviour source){
            Debug.Log("Player -> Damage Taken");
        }

        protected override void OnStateExit(DD_PlayerStates exitedState){

            if(exitedState == DD_PlayerStates.Shooting){
                GetLineRenderer().SetPosition(1, GetLineRenderer().GetPosition(0)); 
            }
        }

        protected override float GetMoveModifier()
        {
            return ((ActiveState == DD_PlayerStates.Digging)? 0.3f : 1f);
        }

        protected override void UpdateState()
        {
            _shootingTimeElapsed -= Time.deltaTime;
            _shootingTimeColdown -= Time.deltaTime;
            _lineShootingTimeElapsed -= Time.deltaTime;


            switch(ActiveState){
                case DD_PlayerStates.Idle: 
                    ProcessInputsMove();
                    TurnOffShooting();
                break;
                case DD_PlayerStates.Moving : 
                    ProcessInputsMove();
                    UpdateMove();
                break;
                case DD_PlayerStates.Digging :
                    UpdateMove();
                    _stateDuration -= Time.deltaTime;
                break;
                case DD_PlayerStates.Shooting: {
                    ProcessShooting();
                    ProcessShootingVisuals();

                    if(Guard.IsValid(_pumpableEnemy)){
                        if(_pumpableEnemy.IsDead()){
                            _shootingTimeElapsed -= SHOT_ACTION_NONLANDED_COLDOWN;
                        }
                    }

                    if(_shootingRequirementsMeet) PumpingShot();
                }break;
            }

            base.UpdateState();
        }   

        LineRenderer GetLineRenderer(){
        //    if(Guard.IsValid(_pumpableEnemy) && !_pumpableEnemy.IsDead()){
       //         _shootLineRenderer.SetPosition(1,_shootLineRenderer.GetPosition(0));

                return _landedShootLineRendrer;
        //    }

        //    _landedShootLineRendrer.SetPosition(1,_landedShootLineRendrer.GetPosition(0));
        //    return _shootLineRenderer;
        }


        private void ProcessShootingVisuals(){
            if(_lineShootingTimeElapsed > 0){

                Vector3 direction = GetShootingDirectionAndUpdateShotingStartPoint();
                _landingPoint = 
                    Guard.IsValid(_pumpableEnemy) ?
                    _pumpableEnemy.GetPumpingPoint() :
                    _startingPoint + (direction * DISTANCE_OF_SHOOT);

                GetLineRenderer().SetPosition(0, _startingPoint);

                Vector3 landingWay     = (_landingPoint - _startingPoint) * (1.0f - _lineShootingTimeElapsed/SHOT_LANDING_TIME);
                landingWay.z = 0;

                GetLineRenderer().SetPosition(0, _startingPoint);
                GetLineRenderer().SetPosition(1, _startingPoint + landingWay);
            }else{

                _landingPoint = 
                    Guard.IsValid(_pumpableEnemy) ?
                    _pumpableEnemy.GetPumpingPoint() :
                    _landingPoint;

                GetLineRenderer().SetPosition(0, _startingPoint);
                GetLineRenderer().SetPosition(1, _landingPoint);
            }
        }

        private void ProcessShooting(){

            if(Guard.IsValid(_pumpableEnemy)) return;

            _lockMoving   = true;
            _lockRotation = true;
            
            if(!_shootingFailed){
                _shootingTimeColdown = SHOT_PUMP_ACTION_COLDOWN;
                _shootingTimeElapsed = SHOT_ACTION_NONLANDED_COLDOWN;
                _shootingFailed = true;
                _lineShootingTimeElapsed = SHOT_LANDING_TIME;

                AudioSystem.Instance.PlayEffect("DigDug_Shoot", 1);
            }


            Vector3 direction     = GetShootingDirectionAndUpdateShotingStartPoint();

            RaycastHit2D[] hits = Physics2D.RaycastAll(_startingPoint, direction, DISTANCE_OF_SHOOT, _enemyLayer);
            _startingPoint.z = 0;

            for(int i = 0; i < (hits?.Length ?? 0); i++ ){
                LF_ColliderSide side = hits[i].collider.GetComponent<LF_ColliderSide>();
                if(Guard.IsValid(side)) _pumpableEnemy = side.GetParent().GetComponent<IPumpableEnemy>();
                if(Guard.IsValid(_pumpableEnemy)){
                    if(_pumpableEnemy.CanBePumped()){
                        _pumpableEnemy.Pump();
                        _shootingTimeElapsed = SHOT_ACTION_LANDED_COLDOWN;
                        return;
                    }else{
                        _pumpableEnemy = null;
                    }
                }
            }

        }

        private Vector3 GetShootingDirectionAndUpdateShotingStartPoint(){
            Vector3 direction = new Vector3();

            switch (GetFacingDirection()) {
                case ESM.AnimationSide.Left:
                    _startingPoint = Graphicals.transform.GetChild(0).transform.position;
                    direction = Vector3.left;
                    break;
                case ESM.AnimationSide.Right:
                
                    _startingPoint = Graphicals.transform.GetChild(0).transform.position;
                    direction = Vector3.right;
                    break;
                case ESM.AnimationSide.Top:
                
                    _startingPoint = Graphicals.transform.GetChild(1).transform.position;
                    direction = Vector3.up;
                    break;
                case ESM.AnimationSide.Bottom:
                
                    _startingPoint = Graphicals.transform.GetChild(2).transform.position;
                    direction = Vector3.down;
                    break;
            }
            _startingPoint.z = 0;
            return direction;
        }

        private void PumpingShot(){
            if(!Guard.IsValid(_pumpableEnemy)) return;
            if(_pumpableEnemy.GetPumpStacks() > MAX_PUMPING-1) return;

            _shootingTimeColdown = SHOT_PUMP_ACTION_COLDOWN;
            _shootingTimeElapsed = SHOT_ACTION_LANDED_COLDOWN;
            _pumpableEnemy.Pump();
        }

        private void TurnOffShooting(){
            _shootingFailed = false;
            _lockMoving     = false;
            _pumpableEnemy  = null;
            _lockRotation   = false;
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

        protected override DD_PlayerStates CheckStateTransitions()
        {
            switch(ActiveState){
                case DD_PlayerStates.Idle:
                    if(_shootingRequirementsMeet) return DD_PlayerStates.Shooting;
                    if(_inputs.magnitude > 0) return DD_PlayerStates.Moving;
                break;
                case DD_PlayerStates.Moving:
                    if(_shootingRequirementsMeet) return DD_PlayerStates.Shooting;
                    if(_inputs.magnitude <= 0) return DD_PlayerStates.Idle;
                    if(WillBeDigging()) return DD_PlayerStates.Digging;
                    
                break;
                case DD_PlayerStates.Digging:
                    if(_stateDuration <= 0) {
                        if(WillBeDigging() && _inputs.magnitude > 0) return DD_PlayerStates.Digging;
                        return DD_PlayerStates.Moving;
                    }
                break;
                case DD_PlayerStates.Shooting:
                    //if(IsDeadByBlinking(CONSTS.BLINK_TIMES_TO_BE_DEAD)) return PlayerStates.Dead;
                    if(_shootingTimeElapsed <= 0) return DD_PlayerStates.Idle;
                    break;
            }

            _shootingRequirementsMeet = Input.GetKeyDown(KeyCode.LeftShift) && _shootingTimeColdown <= 0;


            return ActiveState;
        }


        private void ProcessInputsMove(){
            _inputs.x = Input.GetAxisRaw("Horizontal");
            _inputs.y = Input.GetAxisRaw("Vertical");
            _shoot    = Input.GetKeyDown(KeyCode.Space);

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