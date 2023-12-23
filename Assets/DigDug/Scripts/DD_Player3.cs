using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ESM;
using UnityEngine.UI;
using System.Threading;

namespace DigDug{

    public enum DD_PlayerStates{
        Idle,
        Moving,
        Shooting,
        Digging,
        Dead
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
        const float SHOT_PUMP_ACTION_COLDOWN = 0.75f;
        const float SHOT_LANDING_TIME = 0.5f;
        const float DISTANCE_OF_SHOOT = 1.5f;
        const int MAX_PUMPING = 3;

        [SerializeField] LineRenderer _shootLineRenderer;
        [SerializeField] LineRenderer _landedShootLineRendrer;

        [SerializeField] LayerMask _enemyLayer;
        [SerializeField] LayerMask _rockLayer;

        [SerializeField] Transform[] _points;

        bool _shootingRequirementsMeet = false;
        bool _shootingFailed           = false;
        bool _shootBrick               = false;
        float _shootingTimeColdown = 0;
        float _shootingTimeElapsed = 0;
        float _lineShootingTimeElapsed = 0;
        Vector3 _landingPoint = new Vector3();
        Vector3 _startingPoint = new Vector3();
        Vector3 _rockShotPoint = new Vector3();
        IPumpableEnemy _pumpableEnemy = null;


        protected bool _shoot;
        private bool _isDead;
        private float _stateDuration;

        [SerializeField] Color[] playerColors;

        protected override void Awake() { 
            Graphicals.GetComponent<UnityEngine.UI.Image>().color = playerColors[DD_PlayerSelector.PlayerSelected];
            
            base.Awake(); 
            Instance = this; 
        }

        private void OnEnable() {
            Setup();
        }

        public void Setup(){
            _shootingRequirementsMeet = false;
            _shootingFailed           = false;
            _shoot                    = false;
            _isDead                   = false;
            _shootBrick               = false;
            _shootingTimeColdown      = 0;
            _shootingTimeElapsed      = 0;
            _lineShootingTimeElapsed  = 0;
            _stateDuration            = 0;
            _landingPoint             = new Vector3();
            _startingPoint            = new Vector3();
            _rockShotPoint            = new Vector3();
            _pumpableEnemy            = null;

            _lastHorizontalDirection = AnimationSide.Left;
        
        }

        private void ProcessInputsMove(){
            _inputs.x = Input.GetAxisRaw("Horizontal");
            _inputs.y = Input.GetAxisRaw("Vertical");
            _shoot    = Input.GetKeyDown(KeyCode.N);

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


        protected override void UpdateMove()
        {
            /*
            for(int j = 0; j < rayPoints.Length; j++){
                RaycastHit2D[] raycastHit2s = Physics2D.RaycastAll( rayPoints[j].position, _inputs, 0.8f, _rockLayer); 
            
                Debug.DrawLine(Graphicals.transform.position, Graphicals.transform.position + (Vector3)_inputs * 0.8f, Color.cyan);
                Debug.DrawLine(rayPoints[0].position, rayPoints[0].position + (Vector3)_inputs * 0.8f, Color.cyan);
                Debug.DrawLine(rayPoints[1].position, rayPoints[1].position + (Vector3)_inputs * 0.8f, Color.magenta);
                Debug.DrawLine(rayPoints[2].position, rayPoints[2].position + (Vector3)_inputs * 0.8f, Color.red);
                Debug.DrawLine(rayPoints[3].position, rayPoints[3].position + (Vector3)_inputs * 0.8f, Color.blue);
            
                if(Mathf.Abs(_inputs.x)== 0 || Mathf.Abs(_inputs.y)== 0 ){

                string ss = "";
                for(int i = 0; i < raycastHit2s.Length; i++){
                    ss += raycastHit2s[i].collider.name + ":" + raycastHit2s[i].collider.tag + " ";
                    if(raycastHit2s[i].collider.tag.Contains("cle2")) return;
               }
            Debug.Log(ss);
            
                }

            }
            */
            base.UpdateMove();
        }

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
                case DD_PlayerStates.Shooting:
                    AudioSystem.PlaySample("DigDug_Shoot");
                break;
                case DD_PlayerStates.Dead:
                    RequestDisable(2);
                    AudioSystem.PlaySample("DigDug_PlayerDie");
                    TimersManager.Instance.FireAfter(5, () => Events.Gameplay.RiseEvent(GameplayEventType.GameOver));
                    TimersManager.Instance.FireAfter(4, () => AlphaManipolator.Show());
                break;
            }
        }

        public void TakeDamage(int amount, MonoBehaviour source){
            _isDead = true;
        }

        LineRenderer GetLineRenderer(){
        //    if(Guard.IsValid(_pumpableEnemy) && !_pumpableEnemy.IsDead()){
       //         _shootLineRenderer.SetPosition(1,_shootLineRenderer.GetPosition(0));

            if(Guard.IsValid(_pumpableEnemy)){
                return _landedShootLineRendrer;
            }

            return _shootLineRenderer;
        //    }

        //    _landedShootLineRendrer.SetPosition(1,_landedShootLineRendrer.GetPosition(0));
        //    return _shootLineRenderer;
        }


        protected override void OnStateExit(DD_PlayerStates exitedState){

            if(exitedState == DD_PlayerStates.Shooting){
                GetLineRenderer().SetPosition(1, GetLineRenderer().GetPosition(0)); 
            }
        }

        protected override float GetMoveModifier()
        {
            return (ActiveState == DD_PlayerStates.Digging)? 0.4f : 1f;
        }


        private void TurnOffShooting(){
            _shootingFailed = false;
            _pumpableEnemy  = null;
            _shootBrick = false;
        }

        private void ProcessShooting(){

            if(Guard.IsValid(_pumpableEnemy)) return;

            _shootingRequirementsMeet = false;
            
            if(!_shootingFailed){
                _shootingTimeColdown = SHOT_PUMP_ACTION_COLDOWN;
                _shootingTimeElapsed = SHOT_ACTION_NONLANDED_COLDOWN;
                _shootingFailed = true;
                _lineShootingTimeElapsed = GetShootLandingTime();

                AudioSystem.Instance.PlayEffect("DigDug_Shoot", 1);
            }

            Vector3 direction     = GetShootingDirectionAndUpdateShotingStartPoint();
            RaycastHit2D[] hits = Physics2D.RaycastAll(_startingPoint, direction, DISTANCE_OF_SHOOT, _enemyLayer);
            _startingPoint.z = 0;
            RaycastHit2D[] solidHits = Physics2D.RaycastAll(_startingPoint, direction, DISTANCE_OF_SHOOT, _blockslayerMask);

            if((solidHits?.Length ?? 0) > 0){
                _rockShotPoint = solidHits[0].point;
                        _shootBrick = true;
            }

            for(int i = 0; i < (hits?.Length ?? 0); i++ ){
                LF_ColliderSide side = hits[i].collider.GetComponent<LF_ColliderSide>();

                if((solidHits?.Length ?? 0) > 0){
                    float distanceA = Vector2.Distance(solidHits[0].point, transform.position);
                    float distanceB = Vector2.Distance(side.transform.position, transform.position);
                    
                    if(distanceA < distanceB){
                        GetLineRenderer().SetPosition(1, solidHits[0].point);
                        _rockShotPoint = solidHits[0].point;
                        _shootBrick = true;
                        return;
                    }else{
                        _shootBrick = false;
                    }
                }

                if(Guard.IsValid(side)) {
                    GetLineRenderer().SetPosition(1, GetLineRenderer().GetPosition(0)); 
                    _pumpableEnemy = side.GetParent().GetComponent<IPumpableEnemy>();
                }
                if(Guard.IsValid(_pumpableEnemy)){
                    if(_pumpableEnemy.CanBePumped()){
                        _pumpableEnemy.Pump();
                        _shootingTimeElapsed = SHOT_ACTION_LANDED_COLDOWN;
                        _shootingTimeColdown = SHOT_PUMP_ACTION_COLDOWN;
                        return;
                    }else{
                        _pumpableEnemy = null;
                    }
                }
            }
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


        private Vector3 GetLandingPoint(){
             if(_lineShootingTimeElapsed > 0){
                    Vector3 direction = GetShootingDirectionAndUpdateShotingStartPoint();
                    return Guard.IsValid(_pumpableEnemy) ?
                    _pumpableEnemy.GetPumpingPoint() :
                    _shootBrick ? 
                        _rockShotPoint :
                        _startingPoint + (direction * DISTANCE_OF_SHOOT);
             }else{
                    return Guard.IsValid(_pumpableEnemy) ?
                    _pumpableEnemy.GetPumpingPoint() :
                    _landingPoint;
             }
        }

        private float GetShootLandingTime(){
            
            if(false){
                

                float speed = DISTANCE_OF_SHOOT/SHOT_LANDING_TIME;
                float distance = Vector2.Distance(_startingPoint, _rockShotPoint);

                Debug.Log($"{speed} = {DISTANCE_OF_SHOOT}/{SHOT_LANDING_TIME} :: {distance/speed} = {distance}/{speed}");


                return distance/speed;
            }

            return SHOT_LANDING_TIME;
        }

        private void ProcessShootingVisuals(){
            if(_lineShootingTimeElapsed > 0){

                _landingPoint = GetLandingPoint();

                GetLineRenderer().SetPosition(0, _startingPoint);

                Vector3 landingWay     = 
                    (_landingPoint - _startingPoint) * 
                    (1.0f - _lineShootingTimeElapsed/GetShootLandingTime());

                landingWay.z = 0;

                GetLineRenderer().SetPosition(0, _startingPoint);
                GetLineRenderer().SetPosition(1, _startingPoint + landingWay);
            }else{

                _landingPoint = GetLandingPoint();


                GetLineRenderer().SetPosition(0, _startingPoint);
                GetLineRenderer().SetPosition(1, _landingPoint);
            }
        }
        private Vector3 GetShootingDirectionAndUpdateShotingStartPoint(){
            Vector3 direction = new Vector3();
            _startingPoint = Graphicals.transform.GetChild(0).transform.position;
            switch (GetRotatedDirection()) {
                case ESM.AnimationSide.Left:
                    direction = Vector3.left;
                    break;
                case ESM.AnimationSide.Right:
                    direction = Vector3.right;
                    break;
                case ESM.AnimationSide.Top:
                    direction = Vector3.up;
                    break;
                case ESM.AnimationSide.Bottom:    
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
                    if(_isDead) return DD_PlayerStates.Dead;
                    if(_shootingRequirementsMeet) return DD_PlayerStates.Shooting;
                    if(_inputs.magnitude > 0) return DD_PlayerStates.Moving;
                break;
                case DD_PlayerStates.Moving:
                    if(_isDead) return DD_PlayerStates.Dead;
                    if(_shootingRequirementsMeet) return DD_PlayerStates.Shooting;
                    if(_inputs.magnitude <= 0) return DD_PlayerStates.Idle;
                    if(WillBeDigging()) return DD_PlayerStates.Digging;
                break;
                case DD_PlayerStates.Digging:
                    if(_isDead) return DD_PlayerStates.Dead;
                    if(_stateDuration <= 0) {
                        if(WillBeDigging() && _inputs.magnitude > 0) return DD_PlayerStates.Digging;
                        return DD_PlayerStates.Moving;
                    }
                break;
                case DD_PlayerStates.Shooting:
                    if(_isDead) return DD_PlayerStates.Dead;
                    //if(IsDeadByBlinking(CONSTS.BLINK_TIMES_TO_BE_DEAD)) return PlayerStates.Dead;
                    if(_shootingTimeElapsed <= 0) return DD_PlayerStates.Idle;
                    break;
            }

            _shootingRequirementsMeet = Input.GetKeyDown(KeyCode.M) && _shootingTimeColdown <= 0;

            return ActiveState;
        }
    }
}