using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_Follower : ESM.SMC_1D<BS_PlayerState>,
    ITakeDamage,
    IDealDamage,
    IUseDetector
{

    [SerializeField] BoxCollider2D _hitBox;
    
    [SerializeField] GameObject _missle;
    [SerializeField] GameObject _missleBegin;
    [SerializeField] Transform _towerHead;
    
    [SerializeField] int _MaxHealthPoints = 30;

    private Vector2 _inputs = new Vector2();
    private int _health;
    private bool _shoot;

    private float _cTowerRotation = 0;
    private float _cTankRotation  = 0;
    protected float _movePenalty = 1;

    [SerializeField] float _towerRotation = 60;
    [SerializeField] float _tankRotation = 60;
    [SerializeField] float _maxSpeed = 10f;
    [SerializeField] float _spaceMoveFriction = 0.1f;
    [SerializeField] float _accelerationMove = 0.5f;
    [SerializeField] float calcMoveSpeed = 0;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] LayerMask _EnemylayerMask;
    [SerializeField] int _points;
    [SerializeField] Transform[] _rayPoints;
    [SerializeField] GameObject _explodeAnimation;

    private float _idleBreakTime = 2.0f;
    private float _shootTimer    = 2.0f;

    protected GameObject _aimedTarget;

    protected override void Awake() {
        base.Awake();
        _health = _MaxHealthPoints;
        _hitBox.gameObject.SetActive(true);
    }
/*
    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float _x = v.x*Mathf.Cos(angle) - v.y*Mathf.Sin(angle);
        float _y = v.x*Mathf.Sin(angle) + v.y*Mathf.Cos(angle);
        return new Vector2(_x,_y);  
    }
*/

    private float notEngineFaliure = 1f;
    protected override void UpdateState()
    {
        switch(ActiveState){
            case BS_PlayerState.Idle: 
                _idleBreakTime -= Time.deltaTime;
                FocusTowerOnPlayer();
            break;
            case BS_PlayerState.Move:
                notEngineFaliure -= Time.deltaTime;
                _engineFaliureTime -= Time.deltaTime;
                
                FocusTowerOnPlayer();
                if(notEngineFaliure < 0){
                    _engineFaliureTime = Random.Range(0.25f, 0.75f);
                    notEngineFaliure  += Random.Range(1f, 7f) + _engineFaliureTime;
                }

                if(Guard.IsValid(player)){

                    RotateMove();
                    UpdateSpeed();
                    ProcessMove(transform.up * calcMoveSpeed);
                }

            break;
            case BS_PlayerState.Dead: break;
        }

        ProcessInputs();
        //RotateTowerHead();
        Shoot();
    }

    private void ProcessInputs(){
        ProcessInputsMove();
        //ProcessInputsAttack();
    }

    private void Shoot(){
        if(_shootTimer > 0) {
            _shootTimer -= Time.deltaTime;
            return;
        }
        _shootTimer = 2f;


        
        AudioSystem.PlaySample("SpaceBase_GunE", 1, true);
        LF_ColliderSide side = 
            Instantiate(
                _missle, 
                _missleBegin.transform.position, 
                Quaternion.identity, 
                transform.parent
            ).GetComponent<LF_ColliderSide>();
        side.SetParent(this);
        side.GetComponent<BS_Missle>().Setup(_towerHead.transform.up);
    }

    private void RotatePatrol(){
        _towerHead.Rotate(new Vector3(0, 0, _towerRotation * Time.deltaTime));
        _cTowerRotation += _towerRotation * Time.deltaTime;

        if(_cTowerRotation > 360) _cTowerRotation -= 360;
    }

    BS_Player player = null;
    public void Detected(MonoBehaviour item){
        player = item.GetComponent<BS_Player>();
    }

    public void SignalLost(MonoBehaviour item){}

    private void FocusTowerOnPlayer(){

        if(player == null) {
            RotatePatrol();
            return;
        }

        Vector3 direction = (player.transform.position - transform.position).normalized;

        Vector3 forwardVector = _towerHead.transform.up;
        float currentAngle = Vector3.Angle(forwardVector, -direction);

        float change = _towerRotation * Time.deltaTime;
        change = (change > currentAngle) ? currentAngle : change;

        Vector3 forwardVectorTemp1 = Quaternion.Euler(0, 0,  change) * forwardVector;
        Vector3 forwardVectorTemp2 = Quaternion.Euler(0, 0, -change) * forwardVector;

        if(currentAngle < Vector3.Angle(forwardVectorTemp1, -direction)) {}
        else if(currentAngle < Vector3.Angle(forwardVectorTemp2, -direction)) change = -change;
        else{
            change = 0;
        }

        _towerHead.Rotate(new Vector3(0, 0, change));
        _cTowerRotation += change;

        
        if(_cTowerRotation >  360) _cTowerRotation -= 360;
        if(_cTowerRotation < -360) _cTowerRotation += 360;
    }

    private void ProcessInputsMove(){

        if(Guard.IsValid(player)){
            if((player.transform.position - transform.position).magnitude > 2){
                _inputs.y = 1.0f;
            }else{
                _inputs.y = 0.0f;
            }
        }
    }
/*
    private void RotateTowerHead(){
        if(_inputs.x != 0){
            float rotation = Mathf.Sign(_inputs.x) * _towerRotation * Time.deltaTime;
            _towerHead.Rotate(new Vector3(0,0,  rotation));
            _cTowerRotation += rotation;

            if(_cTowerRotation >  360) _cTowerRotation -= 360;
            if(_cTowerRotation < -360) _cTowerRotation += 360;
        }
    }
*/

    float _engineFaliureTime = 0;

    private void UpdateSpeed(){

        for(int i = 0; i < _rayPoints.Length; i++){
            RaycastHit2D hit = Physics2D.Raycast(_rayPoints[i].transform.position, transform.up, 0.5f, _layerMask);

        if(hit && 
            (hit.transform.CompareTag("HitBox") ||
             hit.transform.CompareTag("Obstacle"))  ){

            Debug.Log(hit.transform.tag);

Debug.DrawLine(
                _rayPoints[i].transform.position,
                _rayPoints[i].transform.position + transform.up, 
                Color.blue
            );

            calcMoveSpeed = 0;
            return;
        }else{
Debug.DrawLine(
                _rayPoints[i].transform.position,
                _rayPoints[i].transform.position + transform.up, 
                Color.green
            );
        }
        }



        if(_engineFaliureTime > 0){
            calcMoveSpeed = Mathf.Max(calcMoveSpeed - _movePenalty*(_spaceMoveFriction * Time.deltaTime) * 0.3f, 0);
        }else{
            if(_inputs.y > 0){
                calcMoveSpeed = Mathf.Min(calcMoveSpeed +  _movePenalty*(_accelerationMove * Time.deltaTime), _movePenalty*_maxSpeed);
            }
            else calcMoveSpeed = Mathf.Max(calcMoveSpeed - _movePenalty*(_spaceMoveFriction * Time.deltaTime), 0);
        }
    }

    private void RotateMove(){

        if(Mathf.Abs(_cTowerRotation) > 0){

            float direction = 0;
            if(_cTowerRotation < 0) direction = -1;
            if(_cTowerRotation > 0) direction =  1;
            

            float rotationChange = 
                direction * 
                // /(1.0f - Mathf.Max( 0, (calcMoveSpeed - (_maxSpeed * 0.05f)) /_maxSpeed)) *
                _tankRotation  * 
                Time.deltaTime;
            if(rotationChange > Mathf.Abs(_cTowerRotation)){
                rotationChange = _cTowerRotation;
            }


            _towerHead.Rotate( new Vector3(0,0, - rotationChange));
            transform.Rotate(  new Vector3(0,0,   rotationChange));

            _cTowerRotation -= rotationChange;
            _cTankRotation  += rotationChange;
        }
    }

    protected override void OnStateEnter(BS_PlayerState enteredState)
    {
        switch(ActiveState){
            case BS_PlayerState.Idle:
                _hitBox.gameObject.SetActive(true);
            break;
            case BS_PlayerState.Move: break;
            case BS_PlayerState.Dead: 
                RequestDestroy(1f);
                PointsCounter.Score += _points;
                _explodeAnimation.SetActive(true);
                
                AudioSystem.PlaySample("SpaceBase_Explode", 1, true);
            //    HighScoreRanking.TryAddNewRecord(PointsCounter.Score);
            //    TimersManager.Instance.FireAfter(5, () => {
            //        _endScene.OnSceneLoadAsync();
            //    });
            break;
        }
    }

    protected override void OnStateExit(BS_PlayerState exitedState)
    {
        switch(ActiveState){
            case BS_PlayerState.Idle: break;
            case BS_PlayerState.Move: break;
            case BS_PlayerState.Dead: break;
        }
    }

    protected override BS_PlayerState CheckStateTransitions()
    {
        switch(ActiveState){
            case BS_PlayerState.Idle:
                if(_health < 0) return BS_PlayerState.Dead;
                if(_inputs.y > 0) return BS_PlayerState.Move;
            break;
            case BS_PlayerState.Move:
                if(_health < 0) return BS_PlayerState.Dead;
                if(calcMoveSpeed <= 0.1f) return BS_PlayerState.Idle;
            break;
            case BS_PlayerState.Dead: break;
        }

        return ActiveState;
    }

    public void TakeDamage(int amount, MonoBehaviour source = null){

        _health -= amount;

/*
        if( !_ishurt 
            && (ActiveState != BS_PlayerState.Hurt
            ||  ActiveState != BS_PlayerState.Dead)
            ){
            _healthPoints -= amount;
            if(amount > 0) _ishurt = true;
            
            
            AudioSystem.PlaySample(
                (amount < 0) ?
                    SOUND_PLAYER_HEAL :
                    ((UnityEngine.Random.Range(0,2) == 0) ?
                        SOUND_PLAYER_HURT1 :
                        SOUND_PLAYER_HURT2
                    )
                , 1, true);


            _healthPoints = Math.Max( Mathf.Min(_healthPoints, _MaxHealthPoints), -1);

            _PlayerHPBar.SetupHp(_healthPoints/_MaxHealthPoints);
        }
*/
    }

    public int GetDamage(){ return 1; }
}
