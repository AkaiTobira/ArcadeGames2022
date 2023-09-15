using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public static class CONSTS
{
    public const float FLOAT_EPSILON = 0.01f;
    public const float ANIMATION_ONE_BLINK_DURAION = 0.5f;
    public const int BLINK_TIMES_TO_BE_DEAD = 30000000;
    public const float DIGGING_TIME = 0.6f;

    public const float SHOT_ACTION_LANDED_COLDOWN = 1.0f;
    public const float SHOT_ACTION_NONLANDED_COLDOWN = 0.5f;
    public const float SHOT_PUMP_ACTION_COLDOWN = 0.5f;
    public const float SHOT_LANDING_TIME = 0.5f;
    public const float DISTANCE_OF_SHOOT = 2.5f;
};

public enum PlayerStates{
        Idle,
        Move,
        Dig,
        Shoot,
        Dead
}


public class DigDugger : BlinkableCharacter<PlayerStates>
{

    public static DigDugger Player; 

    float _digginingMoveMultipler = 0.5f;

    bool _diggingRequirementsMeet  = false;
    bool _shootingRequirementsMeet = false;
    bool _shootingFailed           = false;

    bool _lockMoving = false;
    bool _lockRotation = false;

    float _shootingTimeColdown = 0;
    float _shootingTimeElapsed = 0;
    float _lineShootingTimeElapsed = 0;
    //int _pumpingStacks = 0;

    Vector2 _inputs = new Vector2();
    Vector3 _landingPoint = new Vector3();
    Vector3 _startingPoint = new Vector3();
    Enemy1 _pumpableEnemy = null;

    [SerializeField] LayerMask _enemyLayer;
    [SerializeField] Vector3 _graphicalsBaseOffset = new Vector3();
    [SerializeField] Vector3 _graphicalsDiggerOffset = new Vector3();
    [SerializeField] GameObject _uiDangerous;
    [SerializeField] LineRenderer _shootLineRenderer;

    [SerializeField][NonReorderable] private Animations[] Animations2;



    void Start(){
        Player = this;

        _graphicalsBaseOffset = Graphicals.transform.position - transform.position;

        //Need to override parent propeties as long as it cannot be locked with [NonReaodable]
        _animations = Animations2;
        
        CanvasSorter.AddCanvas(Graphicals.GetComponent<Canvas>());
        _uiDangerous.SetActive(false);
    //    DeadState = PlayerStates.Dead;
    }

    private bool reportDead = true;
    
// /    float _elapsedDangerTime = 0;

    protected override void UpdateState(){
        _shootingTimeElapsed -= Time.deltaTime;
        _shootingTimeColdown -= Time.deltaTime;
        _lineShootingTimeElapsed -= Time.deltaTime;


    //    _elapsedDangerTime += Time.deltaTime;
    //    if(_elapsedDangerTime > 0 && IsBlinking() ){
    //        AudioSystem.Instance.PlayEffect("DigDug_Danger", 1, true);
    //        _elapsedDangerTime = -0.6f;
    //    }

        switch (ActiveState) {
            case PlayerStates.Idle: 
                    TurnOffShooting();
                break;
            case PlayerStates.Move:  
                    _lockMoving = false;
                    ProcessMove(_inputs);
                break;
            case PlayerStates.Dig:   ProvessMoveInDigging(_inputs); break;
            case PlayerStates.Dead:  {
                if(reportDead){
                    Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.GameOver, GameOver.Dead));
                    reportDead = false;
                }
            
            } break;
            case PlayerStates.Shoot: {
                ProcessShooting();
                ProcessShootingVisuals();

                if(Guard.IsValid(_pumpableEnemy)){
                    if(_pumpableEnemy.IsDead()) _shootingTimeElapsed -= CONSTS.SHOT_ACTION_NONLANDED_COLDOWN;
                }

                if(_shootingRequirementsMeet) PumpingShot();
            }break;
        }
    }

    private void ProcessShootingVisuals(){
        if(_lineShootingTimeElapsed > 0){

            Vector3 direction = GetShootingDirectionAndUpdateShotingStartPoint();
            _landingPoint = 
                Guard.IsValid(_pumpableEnemy) ?
                _pumpableEnemy.PumpingPoint.position :
                _startingPoint + (direction * CONSTS.DISTANCE_OF_SHOOT);

            _shootLineRenderer.SetPosition(0, _startingPoint);

            Vector3 landingWay     = (_landingPoint - _startingPoint) * (1.0f - _lineShootingTimeElapsed/CONSTS.SHOT_LANDING_TIME);
            landingWay.z = 0;

            _shootLineRenderer.SetPosition(0, _startingPoint);
            _shootLineRenderer.SetPosition(1, _startingPoint + landingWay);
        }else{

            _landingPoint = 
                Guard.IsValid(_pumpableEnemy) ?
                _pumpableEnemy.PumpingPoint.position :
                _landingPoint;

            _shootLineRenderer.SetPosition(0, _startingPoint);
            _shootLineRenderer.SetPosition(1, _landingPoint);
        }
    }

    private void ProcessShooting(){

        if(Guard.IsValid(_pumpableEnemy)) return;

        _lockMoving   = true;
        _lockRotation = true;
        
        if(!_shootingFailed){
            _shootingTimeColdown = CONSTS.SHOT_PUMP_ACTION_COLDOWN;
            _shootingTimeElapsed = CONSTS.SHOT_ACTION_NONLANDED_COLDOWN;
            _shootingFailed = true;
            _lineShootingTimeElapsed = CONSTS.SHOT_LANDING_TIME;

            AudioSystem.Instance.PlayEffect("DigDug_Shoot", 1);
        }


        Vector3 direction     = GetShootingDirectionAndUpdateShotingStartPoint();

        RaycastHit2D[] hits = Physics2D.RaycastAll(_startingPoint, direction, CONSTS.DISTANCE_OF_SHOOT, _enemyLayer);
        _startingPoint.z = 0;

        for(int i = 0; i < (hits?.Length ?? 0); i++ ){
            _pumpableEnemy = hits[i].collider.GetComponent<Enemy1>();
            if(Guard.IsValid(_pumpableEnemy)){
                _pumpableEnemy.Pump();
                _shootingTimeElapsed = CONSTS.SHOT_ACTION_LANDED_COLDOWN;
                return;
            }
        }

    }

    private Vector3 GetShootingDirectionAndUpdateShotingStartPoint(){
        Vector3 direction = new Vector3();

        switch (GetFacingDirection()) {
            case NeighbourSide.NS_Left:
                _startingPoint = Graphicals.transform.GetChild(0).transform.position;
                direction = Vector3.left;
                break;
            case NeighbourSide.NS_Right:
            
                _startingPoint = Graphicals.transform.GetChild(0).transform.position;
                direction = Vector3.right;
                break;
            case NeighbourSide.NS_Top:
            
                _startingPoint = Graphicals.transform.GetChild(1).transform.position;
                direction = Vector3.up;
                break;
            case NeighbourSide.NS_Bottom:
            
                _startingPoint = Graphicals.transform.GetChild(2).transform.position;
                direction = Vector3.down;
                break;
        }
        _startingPoint.z = 0;
        return direction;
    }

    private void PumpingShot(){
        if(!Guard.IsValid(_pumpableEnemy)) return;
        if(_pumpableEnemy.GetPumpStacks() > Enemy1.MAX_PUMPING-1) return;

        _shootingTimeColdown = CONSTS.SHOT_PUMP_ACTION_COLDOWN;
        _shootingTimeElapsed = CONSTS.SHOT_ACTION_LANDED_COLDOWN;
        _pumpableEnemy.Pump();
    }

    public override void SetupBlink(bool isInDangerousZone)
    {
        base.SetupBlink(isInDangerousZone);
    //    _uiDangerous.SetActive(isInDangerousZone);
    }

    private void TurnOffShooting(){
        _shootingFailed = false;
        _lockMoving     = false;
        _pumpableEnemy  = null;
        _lockRotation   = false;
    }

    protected override void OnStateEnter(PlayerStates enteredState)
    {
        switch (ActiveState) {
            case PlayerStates.Idle : 
                SetupFixedDigging(false, null);  
                _shootLineRenderer.SetPosition(1, _shootLineRenderer.GetPosition(0)); 
            break;
            case PlayerStates.Move : 
                _shootLineRenderer.SetPosition(1, _shootLineRenderer.GetPosition(0)); 
            break;
            //case PlayerStates.Dead : RequestDisable(1.0f); break;
            default : break;
        }
    }

    protected override void OnStateExit(PlayerStates enteredState)
    {
        switch (ActiveState) {
            case PlayerStates.Dig: 
                SetupFixedDigging(false, null); 
                break;
            case PlayerStates.Shoot: 
                _shootLineRenderer.SetPosition(1, _shootLineRenderer.GetPosition(0)); 
                break;
            default : break;
        }
    }

    protected override PlayerStates CheckStateTransitions (){

        switch (ActiveState) {
            case PlayerStates.Idle:
                //if(IsDeadByBlinking(CONSTS.BLINK_TIMES_TO_BE_DEAD)) return PlayerStates.Dead;
                if(_shootingRequirementsMeet) return PlayerStates.Shoot;
                else if(_diggingRequirementsMeet) return PlayerStates.Dig;
                else if(_inputs.magnitude > CONSTS.FLOAT_EPSILON) return PlayerStates.Move;
                break;
            case PlayerStates.Move:
                //if(IsDeadByBlinking(CONSTS.BLINK_TIMES_TO_BE_DEAD)) return PlayerStates.Dead;
                if(_shootingRequirementsMeet) return PlayerStates.Shoot;
                else if(_diggingRequirementsMeet) return PlayerStates.Dig;
                else if(_inputs.magnitude < CONSTS.FLOAT_EPSILON) return PlayerStates.Idle;
                break;
            case PlayerStates.Dig:
                //if(IsDeadByBlinking(CONSTS.BLINK_TIMES_TO_BE_DEAD)) return PlayerStates.Dead;
                if(!_diggingRequirementsMeet) return PlayerStates.Idle;
                break;
            case PlayerStates.Dead:
                break;
            case PlayerStates.Shoot:
                //if(IsDeadByBlinking(CONSTS.BLINK_TIMES_TO_BE_DEAD)) return PlayerStates.Dead;
                if(_shootingTimeElapsed <= 0) return PlayerStates.Idle;
                break;
        }

        _inputs.x = Input.GetAxisRaw("Horizontal") + _mobileInputs.x;
        _inputs.y = Input.GetAxisRaw("Vertical")   + _mobileInputs.y;
        _diggingRequirementsMeet  = Input.GetKey(KeyCode.N);
        _shootingRequirementsMeet = Input.GetKeyDown(KeyCode.M) && _shootingTimeColdown <= 0;

        return ActiveState;
    }

    public bool IsDigging(){
        return ActiveState == PlayerStates.Dig;
    }

    private void ProvessMoveInDigging(Vector2 directions){
        if(IsBlinking()) directions *= 0.3f;

        if(_lockMoving) return;
        if(directions.y == 0){
            ProcessMove_Horizontal(directions.x * _digginingMoveMultipler);
            return;
        }
        ProcessMove_Vertical(directions.y * _digginingMoveMultipler);
    }


    protected override void ProcessMove(Vector2 directions){
        if(IsBlinking()) directions *= 0.3f;


        if(directions.y == 0){
            ProcessMove_Horizontal(directions.x);
            return;
        }
        ProcessMove_Vertical(directions.y);
    }

    protected override Vector3 GetDirectionChange(){
        if(_lockRotation) return new Vector3();
        return _inputs;
    }

    public void SetupFixedDigging(bool enable, RectTransform digger){
        _lockMoving = enable;
        if(enable){
            (Graphicals.transform as RectTransform).SetParent(digger);
            Graphicals.transform.position = digger.transform.position + _graphicalsDiggerOffset;
        }else{
            TimersManager.Instance.FireAfter(0.01f, () => { 
                    if(Guard.IsValid(this)){
                        (Graphicals.transform as RectTransform).SetParent(transform);
                        Graphicals.transform.position = transform.position + _graphicalsBaseOffset;
                        Graphicals.gameObject.SetActive(true); 
                    }
                });
        }
    }


    Vector2 _mobileInputs = new Vector2();


    public void OnHotizontalButtonPressed(int direction){
        _mobileInputs.x = direction;
    }

    public void OnHotizontalButtonReleased(){
        _mobileInputs.x = 0;
    }

    public void OnVerticalButtonReleased(){
        _mobileInputs.y = 0;
    }

    public void OnVerticalButtonPressed(int direction){
        _mobileInputs.y = direction;
    }

    public void OnShootButtonPressed(){
        _shootingRequirementsMeet = _shootingTimeColdown <= 0;
    }

    public void OnDigButtonPressed(){
        _diggingRequirementsMeet = true;
    }


}
