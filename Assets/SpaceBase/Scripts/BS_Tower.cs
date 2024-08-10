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
    [SerializeField] UpgradeType _type;
    [SerializeField] BS_Base _correlatedBase;
    [SerializeField] int _MaxHealthPoints = 30;
    [SerializeField] int _points;
    [SerializeField] GameObject _explodeAnimation;
    [SerializeField] float _towerRotation = 60;
    [SerializeField] float _shotDelay = 3.5f;
    [SerializeField] BS_TowerHeadSelector _towerSelector;


    private float _health;
    private BS_MainTower _turret;
    private Transform _turretTransform;
    private float _playerDetectedTimer = 0;
    private float _shootTimer = 0;
    private const float TIME_OF_ATTACK = 15f;
    private BS_Player player = null;

    protected override void Awake() {
        base.Awake();
        _health = _MaxHealthPoints;
        _hitBox.gameObject.SetActive(true);
        _turret = _towerSelector.GetTower(_type, 0);
        _turretTransform = _towerSelector.transform;
    }



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
        if(_type != UpgradeType.Flamethower && _type != UpgradeType.Laser){
            if(_shootTimer > 0) return;
            _shootTimer = _shotDelay;
        }

        _turret.Shoot(_turretTransform.up, this, true, true);
    }

    private void RotatePatrol(){
        _turretTransform.Rotate(new Vector3(0, 0, _towerRotation * Time.deltaTime));
    }

    private void FocusTowerOnPlayer(){

        if(player == null) {
            RotatePatrol();
            return;
        }

        if(_turret.RotationLocked) return;
        Vector3 direction = (player.transform.position - transform.position).normalized;

        Vector3 forwardVector = _turretTransform.up;
        float currentAngle = Vector3.Angle(forwardVector, -direction);

        float change = _towerRotation * Time.deltaTime;
        change = (change > currentAngle) ? currentAngle : change;

        Vector3 forwardVectorTemp1 = Quaternion.Euler(0, 0,  change) * forwardVector;
        Vector3 forwardVectorTemp2 = Quaternion.Euler(0, 0, -change) * forwardVector;

        if(currentAngle < Vector3.Angle(forwardVectorTemp1, -direction)) 
            _turretTransform.Rotate(new Vector3(0, 0, change));
        else if(currentAngle < Vector3.Angle(forwardVectorTemp2, -direction)) 
            _turretTransform.Rotate(new Vector3(0, 0, -change));



        //dDebug.Log( Vector3.Angle(_towerHead.transform.up, direction) + " " + Vector3.Dot(_towerHead.transform.up, direction));

        //

    }

    protected override void OnStateEnter(BS_TowerState enteredState)
    {
        switch(ActiveState){
            case BS_TowerState.Patrol: break;
            case BS_TowerState.Dead: 
                _hitBox.gameObject.SetActive(false);
                _turretTransform.gameObject.SetActive(false);
                _turret.RotationLocked = false;
                PointsCounter.AddPoints(PlayerIndex.Player1, _points);
                
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

    public void TakeDamage(float amount, MonoBehaviour source = null){
        _health -= amount;
        _playerDetectedTimer = TIME_OF_ATTACK;

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

        switch(_type){
            case UpgradeType.Missle: return 1;
            case UpgradeType.Laser: return 2 * Time.deltaTime;
            case UpgradeType.Flamethower: return 8 * Time.deltaTime;
            case UpgradeType.Mad: return 1;
        }

        return 0; 
    }
}

