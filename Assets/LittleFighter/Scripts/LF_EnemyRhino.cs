using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LF_EnemyRhinoState{
    Idle,
    Move,
    Attack,
    Hurt,
    Dead,
    AttackPrep,
    AfterAttackBreak,
}

public class LF_EnemyRhino : LF_EnemyBase<LF_EnemyRhinoState>
{
    [SerializeField] GameObject _hitBox;
    [SerializeField] GameObject _attackPunchBox;
    [SerializeField] private int _MaxHealthPoints = 10;
    [SerializeField] private float _attackPreparationTimeDelay = 0.5f;
    [SerializeField] private float _attackBreakTimeDelay = 0.5f;

    private int _healthPoints = 10;
    private bool _ishurt;

    const float ATTACK_PUNCH_TIME = 3f;
    const float HURT_TIME = 0.5f;
    const float MOVE_DURATION = 5f;
    const float ATTACK_PREP_DURATION = 0.3f;

    private float _attackPunchTime = ATTACK_PUNCH_TIME;
    private float _hurtTime = HURT_TIME;
    private float _moveDuration = MOVE_DURATION;
    private float _attackPunchPrepTime = ATTACK_PREP_DURATION;
    private float _attackBreakTime = 0.8f;

    private Vector3 _changeTargetPoint;

    const string SOUND_RHINO_HURT = "LittleFighter_RhinoHurt";
    const string SOUND_RHINO_HIT = "LittleFighter_RhinoHit";
    const string SOUND_RHINO_ATTACK_PREP1 = "LittleFighter_RhinoBeforeAttack1";
    const string SOUND_RHINO_ATTACK_PREP2 = "LittleFighter_RhinoBeforeAttack2";
    const string SOUND_RHINO_ATTACK_END   = "LittleFighter_RhinoAfterAttack";
    


    protected override void Awake()
    {
        _healthPoints = _MaxHealthPoints;
        base.Awake();
        ForceState(LF_EnemyRhinoState.Idle, true);

        _hitParameters[RayPoints.Left].ColliderName = "";
        _hitParameters[RayPoints.Right].ColliderName = "";
    }

    public override int GetCurrentHp()
    {
        return _healthPoints;
    }

    public override int GetMaxHp()
    {
        return _MaxHealthPoints;
    }

    protected override LF_EnemyRhinoState CheckStateTransitions()
    {
        switch(ActiveState){
            case LF_EnemyRhinoState.Idle:
                if(_healthPoints < 0) return LF_EnemyRhinoState.Dead;
                else if(_ishurt) return LF_EnemyRhinoState.Hurt;
                else if(_startDelay > 0) return LF_EnemyRhinoState.Idle;
                else if(_idleTimer > 0) return LF_EnemyRhinoState.Idle;
                else if(_canStartAttack) return LF_EnemyRhinoState.AttackPrep;
                else if(_canMove) return LF_EnemyRhinoState.Move;
            break;
            case LF_EnemyRhinoState.Move: 
                if(_healthPoints < 0) return LF_EnemyRhinoState.Dead;
                else if(_ishurt) return LF_EnemyRhinoState.Hurt;
                else if(_canStartAttack) return LF_EnemyRhinoState.AttackPrep;
                else if(_moveDuration < 0) return LF_EnemyRhinoState.Idle;
            break;
            case LF_EnemyRhinoState.Attack :
                if(_attackPunchTime < 0) return LF_EnemyRhinoState.AfterAttackBreak;
            break;
            case LF_EnemyRhinoState.Hurt: 
                if(_healthPoints < 0) return LF_EnemyRhinoState.Dead;
                else if(_hurtTime < 0) return LF_EnemyRhinoState.Idle;
            break;
            case LF_EnemyRhinoState.AttackPrep:
                if(_healthPoints < 0) return LF_EnemyRhinoState.Dead;
                else if(_ishurt) return LF_EnemyRhinoState.Hurt;
                else if(_canPunch) return LF_EnemyRhinoState.Attack;
            break;
            case LF_EnemyRhinoState.AfterAttackBreak:
                if(_healthPoints < 0) return LF_EnemyRhinoState.Dead;
                else if(_ishurt) return LF_EnemyRhinoState.Hurt;
                else if(_attackBreakTime < 0) return LF_EnemyRhinoState.Idle;
            break;
            case LF_EnemyRhinoState.Dead: break;
        }

        return ActiveState;
    }

    protected override void UpdateState()
    {
        switch(ActiveState){
            case LF_EnemyRhinoState.Idle: 
                _idleTimer -= Time.deltaTime;
                _startDelay -= Time.deltaTime;

                ProcessMoveRequirements(LF_Player.Player.transform.position);
            break;
            case LF_EnemyRhinoState.Move: 
                _moveDuration -= Time.deltaTime;

                ProcessMoveRequirements(LF_Player.Player.transform.position);
                ProcessMove(_directions);
            break;
            case LF_EnemyRhinoState.Attack : 
                _attackPunchTime -= Time.deltaTime;
                if(_attackPunchTime < ATTACK_PUNCH_TIME * 0.95f) _attackPunchBox.SetActive(true);

                ProcessMoveRequirements(_changeTargetPoint);
                ProcessMove(_directions * 2.3f);
            break;
            case LF_EnemyRhinoState.Hurt: 
                _hurtTime -= Time.deltaTime;
            break;
            case LF_EnemyRhinoState.Dead: break;
            
            case LF_EnemyRhinoState.AfterAttackBreak:
                _attackBreakTime -= Time.deltaTime;
            break;
            case LF_EnemyRhinoState.AttackPrep:
                _attackPunchPrepTime -= Time.deltaTime;
                if(_attackPunchPrepTime < 0) _canPunch = true;
            break;
        }
    }

    public override int GetDamage()
    {
        AudioSystem.PlaySample(SOUND_RHINO_HIT, 1, true);
        if(ActiveState == LF_EnemyRhinoState.Attack) return 6;
        return 0;
    }

    public override void TakeDamage(int amount, MonoBehaviour source = null)
    {
        if(!_ishurt){
            AudioSystem.PlaySample(SOUND_RHINO_HURT, 1, true);
            _healthPoints -= amount;
            LF_EnemyHPBarsController.ShowHpBar(_type, this, amount);
            _ishurt = true;
        }
    }

    protected override void OnStateEnter(LF_EnemyRhinoState enteredState)
    {
        switch(enteredState){
            case LF_EnemyRhinoState.Idle: 
                _idleTimer = 2f;

                _attackPunchBox.gameObject.SetActive(false);
                _hitBox.gameObject.SetActive(true);
                break;
            case LF_EnemyRhinoState.Attack: 
                _attackPunchTime = ATTACK_PUNCH_TIME;
                _canPunch = false;
                _hitBox.gameObject.SetActive(false);
                _ignoreDetection = true;
                break;
            case LF_EnemyRhinoState.Move: 
                _attackPunchBox.gameObject.SetActive(false);

                _hitBox.gameObject.SetActive(true);
                _moveDuration = Random.Range(2f, MOVE_DURATION);
                break;
            case LF_EnemyRhinoState.Hurt: 
                _hurtTime = HURT_TIME;
                _ishurt = false;
            break;
            
            case LF_EnemyRhinoState.AfterAttackBreak:
                AudioSystem.PlaySample(SOUND_RHINO_ATTACK_END, 1, true);
                _attackBreakTime = _attackBreakTimeDelay;
                _attackPunchBox.gameObject.SetActive(false);
                _hitBox.gameObject.SetActive(true);
                _ignoreDetection = false;
            break;
            case LF_EnemyRhinoState.Dead: 
                RequestDestroy(1f);
            break;
            case LF_EnemyRhinoState.AttackPrep:

                AudioSystem.PlaySample(
                    Random.Range(0,2) == 0 ? SOUND_RHINO_ATTACK_PREP1 : SOUND_RHINO_ATTACK_PREP2, 
                    1, 
                    true);

                _changeTargetPoint = LF_Player.Player.transform.position;
                Vector3 difference = _changeTargetPoint - transform.position;
                Vector3 direction  = difference.normalized;
                _changeTargetPoint = transform.position + (direction * difference.magnitude * 5);

                _attackPunchPrepTime = _attackPreparationTimeDelay;
            break;
        }
    }

    protected override void OnStateExit(LF_EnemyRhinoState exitedState)
    {
        switch(exitedState){
            case LF_EnemyRhinoState.Idle: 
                _startDelay = 0;
            break;
            case LF_EnemyRhinoState.Move: break;
            case LF_EnemyRhinoState.Attack: break;
            case LF_EnemyRhinoState.Hurt: break;
            case LF_EnemyRhinoState.Dead: break;
            case LF_EnemyRhinoState.AttackPrep: break;
            case LF_EnemyRhinoState.AfterAttackBreak: break;
        }
    }
}
