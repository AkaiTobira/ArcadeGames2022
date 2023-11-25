using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BS_TowerState{
    Patrol,
    PlayerDetected,
    Dead
}

public class BS_Tower : ESM.SMC_1D<BS_TowerState>,
    ITakeDamage,
    IDealDamage,
    IUseDetector
{

    [SerializeField] BoxCollider2D _hitBox;
    
    [SerializeField] GameObject _missle;
    [SerializeField] GameObject _missleBegin;
    [SerializeField] Transform _towerHead;
    [SerializeField] BS_Base _correlatedBase;
    [SerializeField] int _MaxHealthPoints = 30;
    [SerializeField] int _points;
    [SerializeField] GameObject _explodeAnimation;

    private int _health;
    private bool _shoot;


    protected float _movePenalty = 1;

    [SerializeField] float _towerRotation = 60;

    protected override void Awake() {
        base.Awake();
        _health = _MaxHealthPoints;
        _hitBox.gameObject.SetActive(true);
    }

    private float _playerDetectedTimer = 0;
    private float _shootTimer = 0;
    

    private const float TIME_OF_ATTACK = 15f;

/*
    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float _x = v.x*Mathf.Cos(angle) - v.y*Mathf.Sin(angle);
        float _y = v.x*Mathf.Sin(angle) + v.y*Mathf.Cos(angle);
        return new Vector2(_x,_y);  
    }
*/


    public void SignalLost(MonoBehaviour item){
    //    _canStartAttack = false;
    }

    BS_Player player = null;
    public void Detected(MonoBehaviour item){
        _playerDetectedTimer = TIME_OF_ATTACK;
        player = item.GetComponent<BS_Player>();

        if(Guard.IsValid(_correlatedBase)) _correlatedBase.Detected(item);
    }

    protected override void UpdateState()
    {
        switch(ActiveState){
            case BS_TowerState.Patrol:
                RotatePatrol();
            break;
            case BS_TowerState.PlayerDetected:

                _shootTimer -= Time.deltaTime;
                _playerDetectedTimer -= Time.deltaTime;

                FocusTowerOnPlayer();
                Shoot();
            break;
            case BS_TowerState.Dead: break;
        }
    }


    private void Shoot(){
        if(_shootTimer > 0) return;
        _shootTimer = 3.5f;

        
        AudioSystem.PlaySample("SpaceBase_GunB", 1, true);

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
    }

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

        if(currentAngle < Vector3.Angle(forwardVectorTemp1, -direction)) 
            _towerHead.Rotate(new Vector3(0, 0, change));
        else if(currentAngle < Vector3.Angle(forwardVectorTemp2, -direction)) 
            _towerHead.Rotate(new Vector3(0, 0, -change));



        //dDebug.Log( Vector3.Angle(_towerHead.transform.up, direction) + " " + Vector3.Dot(_towerHead.transform.up, direction));

        //

    }

    protected override void OnStateEnter(BS_TowerState enteredState)
    {
        switch(ActiveState){
            case BS_TowerState.Patrol: break;
            case BS_TowerState.Dead: 
                _hitBox.gameObject.SetActive(false);
                _towerHead.gameObject.SetActive(false);
                PointsCounter.Score += _points;
                
                AudioSystem.PlaySample("SpaceBase_Explode", 1, true);
                _explodeAnimation.SetActive(true);
            //    TimersManager.Instance.FireAfter(5, () => {
            //        _endScene.OnSceneLoadAsync();
            //    });
            break;
        }
    }

    protected override void OnStateExit(BS_TowerState exitedState){}

    protected override BS_TowerState CheckStateTransitions()
    {
        switch(ActiveState){
            case BS_TowerState.Patrol:
                if(_health < 0 ) return BS_TowerState.Dead;
                else if(_playerDetectedTimer > 0) return BS_TowerState.PlayerDetected;
            break;
            case BS_TowerState.PlayerDetected:
                if(_health < 0 ) return BS_TowerState.Dead;
                else if(_playerDetectedTimer <= 0) return BS_TowerState.Patrol;
            break;
            case BS_TowerState.Dead: break;
        }

        return ActiveState;
    }

    public void TakeDamage(int amount, MonoBehaviour source = null){

        _health -= amount;
        _playerDetectedTimer = TIME_OF_ATTACK;

        Debug.Log("DAmage" + amount);


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

    public int GetDamage(){
        return 1;
    }
}

