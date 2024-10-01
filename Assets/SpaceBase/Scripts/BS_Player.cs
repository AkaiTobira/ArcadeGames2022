using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BS_PlayerState{
    Idle,
    Move,
    Dead
}

public class BS_Player : ESM.SMC_1D<BS_PlayerState>,
    ITakeDamage,
    IDealDamage,
    IUserControlled
{

    [SerializeField] BoxCollider2D _hitBox;
    [SerializeField] GameObject _missle;
    [SerializeField] GameObject _missleBegin;
    [SerializeField] GameObject _invicibleBarrier;
    [SerializeField] Transform _towerHead;
    [SerializeField] LF_PlayerHPBar _PlayerHPBar;
    [SerializeField] Image _invincibleBar;
    [SerializeField] BS_SpeedBar _speedBar;
    [SerializeField] int _MaxHealthPoints = 30;

    private Vector2 _inputs = new Vector2();
    private float _health;
    private float _invincibleTime;
    private bool _shoot;
    private bool _shootCon;
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

    [SerializeField] BS_TowerHeadSelector _mainTowers;
    [SerializeField] BS_MainTower _activeTower;


    int towerLevel = 0;
    UpgradeType _type = UpgradeType.TMissle;

    public UpgradeType GetUpgradeType() { return _type; }

    protected override void Awake() {
        base.Awake();
        _health = _MaxHealthPoints;
        _hitBox.gameObject.SetActive(true);
        PointsCounter.Reset();
        PlayerList<BS_Player>.Register(PlayerIndex.Player1, this);
        _activeTower = _mainTowers.GetTower(_type, towerLevel);
        BS_UIWeaponry.SetUIWeaponry(_type, towerLevel);
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
        switch (ActiveState)
        {
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
        ProcessInvincible();

        _activeTower.Shoot(_towerHead.transform.up, this, _shoot, _shootCon, false);
    }

    private void ProcessInvincible()
    {
        if (_invincibleTime > 0)
        {
            _invincibleBar.fillAmount = _invincibleTime / _invincibleTimeMax;
            _invincibleTime -= Time.deltaTime;
        }
        else
        {
            _invincibleBar.fillAmount = 0;
            _invicibleBarrier.SetActive(false);
        }
    }

    private void ProcessInputs(){
        ProcessInputsMove();
        //ProcessInputsAttack();
    }

    private void ProcessInputsMove(){
        _inputs.x = InputHandler.GetHorizontal();
        _inputs.y = InputHandler.GetKey(InputKey.Action_1_Player1, false) ? 1.0f : 0.0f;
        _shoot    = InputHandler.GetKey(InputKey.Action_2_Player1);
        _shootCon = InputHandler.GetKey(InputKey.Action_2_Player1, false);
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

            if(hit) Debug.Log(hit.transform.tag);

            if(hit && 
                (hit.collider.transform.CompareTag("HitBox") ||
                hit.collider.transform.CompareTag("Obstacle"))  ){

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

            _speedBar.Setup(calcMoveSpeed/_maxSpeed);
        }

        if(_inputs.y > 0){
            calcMoveSpeed = Mathf.Min(calcMoveSpeed +  _movePenalty*(_accelerationMove * Time.deltaTime), _movePenalty*_maxSpeed);
        }
        else 
            calcMoveSpeed = Mathf.Max(calcMoveSpeed - _movePenalty*(_spaceMoveFriction * Time.deltaTime), 0);
    }

    private void RotateMove(){

        if(Mathf.Abs(_cTowerRotation) > 15){

            float direction = 0;
            if(_cTowerRotation < 0) direction = -1;
            if(_cTowerRotation > 0) direction =  1;
            

            float rotationChange = 
                direction * 
                // /(1.0f - Mathf.Max( 0, (calcMoveSpeed - (_maxSpeed * 0.05f)) /_maxSpeed)) *
                _tankRotation  * (_type == UpgradeType.Laser ? 0.7f : 1.0f) * 
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
                _speedBar.Setup(0);
            break;
            case BS_PlayerState.Move: break;
            case BS_PlayerState.Dead: 
                RequestDisable(1f);
                _explodeAnimation.SetActive(true);
            //    TimersManager.Instance.FireAfter(5, () => {
            //        _endScene.OnSceneLoadAsync();
            //    });
            
                AudioSystem.PlaySample("SpaceBase_Explode", 1, true);
                TimersManager.Instance.FireAfter(2f, 
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

    public void TakeDamage(float amount, MonoBehaviour source = null){
        if(_invincibleTime > 0) return;

        _health -= amount;
        if(_health > _MaxHealthPoints) _health = _MaxHealthPoints;
        _PlayerHPBar.SetupHp((float)_health / (float)_MaxHealthPoints);

//        Debug.Log("Player hit for" + amount);

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

    public float GetDamage(){ 
        switch(_type)
        {
            case UpgradeType.TMissle: return 1;
            case UpgradeType.Rocket: return 3f + towerLevel*0.5f;
            case UpgradeType.Laser:  return (2.5f + towerLevel) * Time.deltaTime;
            case UpgradeType.Flamethower: return 0.75f + (0.2f * towerLevel); 
        }    

        return 0; 
    }

    private float _invincibleTimeMax;

    public void InvincibleCollected(float strenght){
        _invincibleTimeMax = strenght;
        _invincibleTime = _invincibleTimeMax;
        _invincibleBar.fillAmount = _invincibleTime/_invincibleTimeMax;
        _invicibleBarrier.SetActive(true);
    }

    public void SwapWeapon(UpgradeType type)
    {
        if(type == _type)  towerLevel += 1;
        else{
            towerLevel = 0;
            _type = type;
        }

        _activeTower = _mainTowers.GetTower(type, towerLevel);
        BS_UIWeaponry.SetUIWeaponry(_type, towerLevel);
    }

    public PlayerIndex GetPlayerIndex() { return PlayerIndex.Player1; }
}
