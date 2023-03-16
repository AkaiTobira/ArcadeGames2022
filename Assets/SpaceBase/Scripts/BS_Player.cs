using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BS_PlayerState{
    Idle,
    Move,
    Dead
}

public class BS_Player : ESM.SMC_1D<BS_PlayerState>,
    ITakeDamage,
    IDealDamage
{

    [SerializeField] BoxCollider2D _hitBox;
    
    [SerializeField] GameObject _missle;
    [SerializeField] GameObject _missleBegin;
    [SerializeField] Transform _towerHead;
    [SerializeField] LF_PlayerHPBar _PlayerHPBar;
    
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
    [SerializeField] Transform[] _rayPoints;
    [SerializeField] GameObject _explodeAnimation;

    protected GameObject _aimedTarget;

    private void Awake() {
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
    protected override void UpdateState()
    {
        switch(ActiveState){
            case BS_PlayerState.Idle: break;
            case BS_PlayerState.Move:
                RotateMove();
                UpdateSpeed();
                ProcessMove(transform.up * calcMoveSpeed);

            break;
            case BS_PlayerState.Dead: break;
        }

        ProcessInputs();
        RotateTowerHead();
        ScanForAimHelper();
        Shoot();
    }

    private void ProcessInputs(){
        ProcessInputsMove();
        //ProcessInputsAttack();
    }

    private void ScanForAimHelper(){

        RaycastHit2D[] hits = Physics2D.RaycastAll(_missleBegin.transform.position, _towerHead.transform.up, 50, _EnemylayerMask);

        for(int i = 0; i < hits.Length; i++){
//            Debug.Log(hits[i].transform.name);
            ITakeDamage side = 
                hits[i].transform.gameObject.GetComponent<ITakeDamage>();
            if(side != null){
                _aimedTarget = hits[i].transform.gameObject;
                Debug.DrawLine(
                    _missleBegin.transform.position, 
                    hits[i].transform.position, 
                    Color.red);
                return;
            }
        }

        Debug.DrawLine(
            _missleBegin.transform.position, 
            _missleBegin.transform.position + _towerHead.transform.up * 50, 
            Color.blue);

        _aimedTarget = null;
    }


    private void Shoot(){
        if(!_shoot) return;

        LF_ColliderSide side = 
            Instantiate(
                _missle, 
                _missleBegin.transform.position, 
                Quaternion.identity, 
                transform.parent
            ).GetComponent<LF_ColliderSide>();
        side.SetParent(this);

        Vector3 direction = _towerHead.transform.up;


        //Aim Helper;
        if(Guard.IsValid(_aimedTarget)){
            Vector3 aimedTarget = (_aimedTarget.transform.position - transform.position).normalized;
            if(aimedTarget.sqrMagnitude < 2500 && Vector3.Angle(aimedTarget, _towerHead.transform.up) < 40){
                direction = (_aimedTarget.transform.position -  transform.position).normalized;
            }
        //    Debug.Log("Shoot" + direction + " " + transform.up + " " + Vector3.Angle(aimedTarget, _towerHead.transform.up));
        } 
        

        side.GetComponent<BS_Missle>().Setup(direction);
    }

    private void ProcessInputsMove(){
        _inputs.x = Input.GetAxisRaw("Horizontal");
        _inputs.y = Input.GetKey(KeyCode.LeftShift) ? 1.0f : 0.0f;
        _shoot    = Input.GetKeyDown(KeyCode.Space);
    }

    private void RotateTowerHead(){
        if(_inputs.x != 0){
            float rotation = Mathf.Sign(_inputs.x) * _towerRotation * Time.deltaTime;
            _towerHead.Rotate(new Vector3(0,0,  rotation));
            _cTowerRotation += rotation;

            if(_cTowerRotation >  360) _cTowerRotation -= 360;
            if(_cTowerRotation < -360) _cTowerRotation += 360;
        }
    }

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


        




            
        if(_inputs.y > 0){
            calcMoveSpeed = Mathf.Min(calcMoveSpeed +  _movePenalty*(_accelerationMove * Time.deltaTime), _movePenalty*_maxSpeed);
        }
        else 
            calcMoveSpeed = Mathf.Max(calcMoveSpeed - _movePenalty*(_spaceMoveFriction * Time.deltaTime), 0);
    }

    private void RotateMove(){

        if(Mathf.Abs(_cTowerRotation) > 45){

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

            _towerHead.Rotate( new Vector3(0,0, -rotationChange));
            transform.Rotate(  new Vector3(0,0,  rotationChange));

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
                RequestDisable(1f);
                HighScoreRanking.TryAddNewRecord(PointsCounter.Score);
                _explodeAnimation.SetActive(true);
            //    TimersManager.Instance.FireAfter(5, () => {
            //        _endScene.OnSceneLoadAsync();
            //    });

                TimersManager.Instance.FireAfter(5f, 
                    () =>
                Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.GameOver, GameOver.Dead)));

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
        if(_health < 0) return BS_PlayerState.Dead;

        switch(ActiveState){
            case BS_PlayerState.Idle:
                if(_inputs.y > 0) return BS_PlayerState.Move;
            break;
            case BS_PlayerState.Move: 
                if(calcMoveSpeed <= 0.1f) return BS_PlayerState.Idle;
            break;
            case BS_PlayerState.Dead: break;
        }

        return ActiveState;
    }

    public void TakeDamage(int amount, MonoBehaviour source = null){
        _health -= amount;
        _PlayerHPBar.SetupHp((float)_health / (float)_MaxHealthPoints);

        Debug.Log("Player hit for" + amount);

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
