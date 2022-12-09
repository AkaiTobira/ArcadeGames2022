using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyStates{
    Idle,
    Move,
    Stare,
    Escape,
    Pumping,
    LookForPlayer,
    Dead
}

public class Enemy1 : StateMachineCharacter<EnemyStates>, IListenToGameplayEvents
{
    // Start is called before the first frame update

    [SerializeField][NonReorderable] private Animations[] Animations2;
    [SerializeField] public Transform PumpingPoint;

    [SerializeField] private Floor2 _currentFloor;

    const float TIME_OF_IDLING = 3f;
    const float TIME_OF_STARING = 2f;
    const float TIME_OF_ESCAPING = 6f;
    
    const float TIME_OF_STARING_COLDOWN = 2f;
    const float TIME_OF_PUMP_COLDOWN = 1f;
    const int TIMES_TO_DEAD_BY_TILE = 1;
    public const int MAX_PUMPING = 5;

    float _elapsedIdlingTime = 3;
    float _elapsedStareTime  = 0;
    float _elapsedStareTimeColdown = 0;


    Vector3 _targetMovePosition;
    Floor2 _previusFloor2;
    Floor2 _previusFloor;
    Floor2 _nextFloor;

    Vector3 _direction;

    int _pumpingStacks = 0;
    float _elapsedPumpTime  = 0;

    float _elapsedEscapeTime = 0;

    enum EnemyID{
        Enemy1,
        Enemy2,
        Enemy3,
    }

    [SerializeField] EnemyID _enemy;


    public void OnGameEvent(GameplayEvent gameplayEvent){
        if(gameplayEvent.type == GameplayEventType.EnemyHasBeenMurdered){
            Enemy1 enemy1 = (Enemy1)gameplayEvent.parameter;
            if(enemy1 == this) return;
            if(Vector3.Distance(enemy1.transform.position, transform.position) < 12f){
                ActiveState = EnemyStates.Escape;
                _elapsedEscapeTime = TIME_OF_ESCAPING;
            }
        }
    }

    void Start(){
    //    _graphicalsBaseOffset = Graphicals.transform.position - transform.position;

        //Need to override parent propeties as long as it cannot be locked with [NonReaodable]
    //    
        if(_enemy == EnemyID.Enemy1) EnemyVisualSlotManager.Enemy1Count += 1;
        if(_enemy == EnemyID.Enemy2) EnemyVisualSlotManager.Enemy2Count += 1;
        if(_enemy == EnemyID.Enemy3) EnemyVisualSlotManager.Enemy3Count += 1;
        
        _animations = Animations2;
        CanvasSorter.AddCanvas(Graphicals.GetComponent<Canvas>());
    //    DeadState = EnemyStates.Dead;

        _currentFloor = LevelController.GetClosestFloor(transform.position);


        Events.Gameplay.RegisterListener(this, GameplayEventType.EnemyHasBeenMurdered);
    }

    bool isUnregistered;
    private void Deregister(){
        if(isUnregistered) return;

        if(_enemy == EnemyID.Enemy1) EnemyVisualSlotManager.Enemy1Count += -1;
        if(_enemy == EnemyID.Enemy2) EnemyVisualSlotManager.Enemy2Count += -1;
        if(_enemy == EnemyID.Enemy3) EnemyVisualSlotManager.Enemy3Count += -1;

        AudioSystem.Instance.PlayEffect("DigDug_EnemyDead", 1);

        isUnregistered = true;
    }


    private void UpdatePhysic_Move(){
        if(Guard.IsValid(_currentFloor)){
            if(!Guard.IsValid(_nextFloor)){
                SetNewTarget();
            }else{
                Vector3 currentPosition = transform.position;
                currentPosition.z = 0;
                if(Vector3.Distance(currentPosition, _targetMovePosition) < 0.2f ){
                    UpdateMoveHistory();
                    SetNewTarget();
                }else{
                    _direction = (_targetMovePosition - currentPosition).normalized;
                    ProcessMove( _direction );
                }
            }
        }else{
            Debug.Log(name + ": No current valid ");
        }
    }

    private void UpdateMoveHistory(){
        _previusFloor2 = _previusFloor;
        _previusFloor = _currentFloor;
        _currentFloor = _nextFloor;
    }
    private void SetNewTarget(){
        _nextFloor    = (ActiveState == EnemyStates.Move)? GetNextFloor() : GetNextFloor_Escape();
        _targetMovePosition = _nextFloor.transform.position + 
            new Vector3( Random.Range(-0.5f,0.5f), Random.Range(-0.5f,0.5f), 0);
        _targetMovePosition.z = 0;
    }

    protected override void UpdateState(){
        _elapsedStareTimeColdown -= Time.deltaTime;

        switch (ActiveState) {
            case EnemyStates.Idle:
                OverrideAnimationUpdate = false;
                _elapsedIdlingTime -= Time.deltaTime;
                break;
            case EnemyStates.Move:
                UpdatePhysic_Move();
                break;
            case EnemyStates.LookForPlayer: break;
            case EnemyStates.Stare:
                _elapsedStareTime -= Time.deltaTime;
                break;
            case EnemyStates.Escape:
                _elapsedEscapeTime -= Time.deltaTime;
                UpdatePhysic_Move();
                break;
            case EnemyStates.Pumping:
                OverrideAnimationUpdate = true;
                if(_elapsedPumpTime <= 0){
                    _pumpingStacks -= 1;
                    if(_pumpingStacks >= 0){
                        SetAnimationFrame(_pumpingStacks, EnemyStates.Pumping);
                    }
                    _elapsedPumpTime = TIME_OF_PUMP_COLDOWN;
                }
                _elapsedPumpTime -= Time.deltaTime;
                break;
            case EnemyStates.Dead:
                break;
        }
    }

    protected override void OnStateEnter( EnemyStates enterState ){

        switch(enterState){
            case EnemyStates.Dead: 
                Deregister();
                RequestDisable(1.0f);
            break;
            case EnemyStates.Stare:
                _elapsedStareTime = TIME_OF_STARING;
                AudioSystem.Instance.PlayEffect("DigDug_EnemyStare", 1, true);
            break;
        }
                    
    }

    protected override void OnStateExit( EnemyStates exitState ){

        switch(exitState){
            case EnemyStates.Stare:
                _elapsedStareTimeColdown = TIME_OF_STARING_COLDOWN;
            break;
        }
    }


    public int GetPumpStacks(){
        return _pumpingStacks;
    }

    public void Pump(){
        if(_pumpingStacks > MAX_PUMPING) return;
        
        SetAnimationFrame(_pumpingStacks, EnemyStates.Pumping);
        _pumpingStacks += 1;
        AudioSystem.Instance.PlayEffect("DigDug_Pump", 1);

        if(_pumpingStacks >= MAX_PUMPING) Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.EnemyHasBeenMurdered, this));
        _elapsedPumpTime = TIME_OF_PUMP_COLDOWN;
        OverrideAnimationUpdate = true;
    }

    private Floor2 GetNextFloor_Escape(){

        Floor2 flor = _currentFloor;

        float distance = 0.0f;
        foreach( KeyValuePair<NeighbourSide, Floor2> valuePair in _currentFloor.Neighbours){
            if(valuePair.Value == null) continue;
            
            float calcDistance = Vector3.Distance(DigDugger.Player.transform.position, valuePair.Value.transform.position);

            if(calcDistance > distance){
                distance = calcDistance;
                flor = valuePair.Value;
            }
        }

        return flor;
    }

    private Floor2 GetNextFloor(){

        float probSum = 0.0f;
        foreach( KeyValuePair<NeighbourSide, Floor2> valuePair in _currentFloor.Neighbours){
            if(valuePair.Value == null) continue;
            if(_currentFloor.IsSideLocked(valuePair.Key)) continue;
            
            if(valuePair.Value == _previusFloor2){
                probSum += 2f;
            }else if(valuePair.Value == _previusFloor){
                probSum += 1f;
            }else{
                probSum += 15f;
            }
        }

        float prob = Random.Range(0, probSum);

        probSum = 0;
        foreach( KeyValuePair<NeighbourSide, Floor2> valuePair in _currentFloor.Neighbours){
            if(valuePair.Value == null) continue;
            if(_currentFloor.IsSideLocked(valuePair.Key)) continue;

            float currFlor = 0f;
            if(valuePair.Value == _previusFloor2){
                currFlor += 2f;
            }else if(valuePair.Value == _previusFloor){
                currFlor += 1f;
            }else{
                currFlor += 15f;
            }
            if(currFlor + probSum > prob) return valuePair.Value;

            probSum += currFlor;
        }

        if(!_currentFloor.IsSolid()){
            _currentFloor = LevelController.GetClosestFloor(transform.position);
        }


        return _currentFloor;
    }

    protected override EnemyStates CheckStateTransitions(){

        if(!Guard.IsValid(DigDugger.Player)) return ActiveState;

        switch (ActiveState) {
            case EnemyStates.Idle:
                //if(IsDeadByBlinking(TIMES_TO_DEAD_BY_TILE)) return EnemyStates.Dead;
                if(_pumpingStacks != 0) return EnemyStates.Pumping;
                else if(_elapsedIdlingTime <= 0) return EnemyStates.Move;
                break;
            case EnemyStates.Move:
                //if(IsDeadByBlinking(TIMES_TO_DEAD_BY_TILE)) return EnemyStates.Dead;
                if(_pumpingStacks != 0) return EnemyStates.Pumping;
                else if(Vector3.Distance(transform.position, DigDugger.Player.transform.position) < 2f && 
                    _elapsedStareTimeColdown <= 0) return EnemyStates.Stare;
                break;
            case EnemyStates.Stare:
                //if(IsDeadByBlinking(TIMES_TO_DEAD_BY_TILE)) return EnemyStates.Dead;
                if(_pumpingStacks != 0) return EnemyStates.Pumping;
                else if(Vector3.Distance(transform.position, DigDugger.Player.transform.position) > 4f) return EnemyStates.Move;
                else if(_elapsedStareTime <= 0) return EnemyStates.Move;
                break;
            case EnemyStates.LookForPlayer:
                //if(IsDeadByBlinking(TIMES_TO_DEAD_BY_TILE)) return EnemyStates.Dead;
                if(_pumpingStacks != 0) return EnemyStates.Pumping;
                break;
            case EnemyStates.Escape:
                //if(IsDeadByBlinking(TIMES_TO_DEAD_BY_TILE)) return EnemyStates.Dead;
                if(_pumpingStacks != 0) return EnemyStates.Pumping;
                else if(_elapsedEscapeTime <= 0) return EnemyStates.Idle;
                break;
            case EnemyStates.Pumping:
                //if(IsDeadByBlinking(TIMES_TO_DEAD_BY_TILE)) return EnemyStates.Dead;
                if(_pumpingStacks == MAX_PUMPING) return EnemyStates.Dead;
                if(_pumpingStacks == 0 && _elapsedPumpTime <= 0) return EnemyStates.Idle;
                
                break;
            case EnemyStates.Dead:break;
        }

        return ActiveState;
    }

    public bool IsDead(){
        return ActiveState == EnemyStates.Dead;
    }

    protected override Vector3 GetDirectionChange(){
        return new Vector3( 
            ( Mathf.Abs(_direction.x) > Mathf.Abs(_direction.y)) ? Mathf.Sign(_direction.x) : 0,
            ( Mathf.Abs(_direction.x) < Mathf.Abs(_direction.y)) ? Mathf.Sign(_direction.y) : 0,
            0
        );
    }

    protected override void ProcessMove(Vector2 directions ){
    //    if(IsBlinking()) directions *= 0.2f;
        if(ActiveState == EnemyStates.Escape) directions *= 2.0f;

        ProcessMove_Horizontal(directions.x);
        ProcessMove_Vertical(directions.y);
    }
}
