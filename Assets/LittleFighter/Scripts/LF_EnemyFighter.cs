using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LF_EnemyFighterState{
    Idle,
    Move,
    Attack,
    Hurt,
    Dead,
    AttackPrep,
}


public class LF_EnemyFighter : LF_EnemyBase<LF_EnemyFighterState>
{
    [SerializeField] GameObject _hitBox;
    [SerializeField] GameObject _attackPunchBox;
    [SerializeField] private int _MaxHealthPoints = 10;
    [SerializeField] private float _attackPreparationTimeDelay = 0.5f;

    [SerializeField] private string _HitSound;
    [SerializeField] private string _HurtSound;
    [SerializeField] private string _PrepAttackSound;
    [SerializeField] private string _AttackSound;


    private int _healthPoints = 10;
    private bool _ishurt;
    

    const float ATTACK_PUNCH_TIME = 0.75f;
    const float HURT_TIME = 0.5f;
    const float MOVE_DURATION = 6f;
    const float ATTACK_PREP_DURATION = 0.3f;

    private float _attackPunchTime = ATTACK_PUNCH_TIME;
    private float _hurtTime = HURT_TIME;
    private float _moveDuration = MOVE_DURATION;
    private float _attackPunchPrepTime = ATTACK_PREP_DURATION;

    protected override void Awake()
    {
        _healthPoints = _MaxHealthPoints;
        base.Awake();
        ForceState(LF_EnemyFighterState.Idle, true);
    }

    public override int GetCurrentHp()
    {
        return _healthPoints;
    }

    public override int GetMaxHp()
    {
        return _MaxHealthPoints;
    }

    protected override LF_EnemyFighterState CheckStateTransitions()
    {
        switch(ActiveState){
            case LF_EnemyFighterState.Idle:
                if(_healthPoints < 0) return LF_EnemyFighterState.Dead;
                else if(_ishurt) return LF_EnemyFighterState.Hurt;
                else if(_startDelay > 0) return LF_EnemyFighterState.Idle;
                else if(_idleTimer > 0) return LF_EnemyFighterState.Idle;
                else if(_canStartAttack) return LF_EnemyFighterState.AttackPrep;
                else if(_canMove) return LF_EnemyFighterState.Move;
            break;
            case LF_EnemyFighterState.Move: 
                if(_healthPoints < 0) return LF_EnemyFighterState.Dead;
                else if(_ishurt) return LF_EnemyFighterState.Hurt;
                else if(_canStartAttack) return LF_EnemyFighterState.AttackPrep;
                else if(_moveDuration < 0) return LF_EnemyFighterState.Idle;
            break;
            case LF_EnemyFighterState.Attack :
                if(_healthPoints < 0) return LF_EnemyFighterState.Dead;
                else if(_ishurt) return LF_EnemyFighterState.Hurt;
                else if(_attackPunchTime < 0) return LF_EnemyFighterState.Idle;
            break;
            case LF_EnemyFighterState.Hurt: 
                if(_healthPoints < 0) return LF_EnemyFighterState.Dead;
                else if(_hurtTime < 0) return LF_EnemyFighterState.Idle;
            break;
            case LF_EnemyFighterState.AttackPrep:
                if(_healthPoints < 0) return LF_EnemyFighterState.Dead;
                else if(_ishurt) return LF_EnemyFighterState.Hurt;
                else if(_canPunch) return LF_EnemyFighterState.Attack;
            break;
            case LF_EnemyFighterState.Dead: break;
        }

        return ActiveState;
    }

    protected override void UpdateState()
    {
        switch(ActiveState){
            case LF_EnemyFighterState.Idle: 
                _idleTimer -= Time.deltaTime;
                _startDelay -= Time.deltaTime;

                ProcessMoveRequirements(LF_Player.Player.transform.position);
            break;
            case LF_EnemyFighterState.Move: 
                _moveDuration -= Time.deltaTime;

                ProcessMoveRequirements(LF_Player.Player.transform.position);
                ProcessMove(_directions);
            break;
            case LF_EnemyFighterState.Attack : 
                _attackPunchTime -= Time.deltaTime;
                if(_attackPunchTime < ATTACK_PUNCH_TIME/2.0f) _attackPunchBox.SetActive(true);
            break;
            case LF_EnemyFighterState.Hurt: 
                _hurtTime -= Time.deltaTime;
            break;
            case LF_EnemyFighterState.Dead: break;
            case LF_EnemyFighterState.AttackPrep:
                _attackPunchPrepTime -= Time.deltaTime;
                if(_attackPunchPrepTime < 0) _canPunch = true;
            break;
        }
    }

    public override int GetDamage()
    {
        AudioSystem.PlaySample(_HitSound, 1, true);
        if(ActiveState == LF_EnemyFighterState.Attack) return 2;
        return 0;
    }

    public override void TakeDamage(int amount, MonoBehaviour source = null)
    {
        if(!_ishurt){
            _healthPoints -= amount;
            LF_EnemyHPBarsController.ShowHpBar(_type, this, amount);
            _ishurt = true;
        }
    }

    protected override void OnStateEnter(LF_EnemyFighterState enteredState)
    {
        switch(enteredState){
            case LF_EnemyFighterState.Idle: 
                _idleTimer = 2f;

                _attackPunchBox.gameObject.SetActive(false);
                _hitBox.gameObject.SetActive(true);
                break;
            case LF_EnemyFighterState.Attack: 
                AudioSystem.PlaySample(_AttackSound, 1, true);
                _attackPunchTime = ATTACK_PUNCH_TIME;
                _canPunch = false;
                break;
            case LF_EnemyFighterState.Move: 
                _attackPunchBox.gameObject.SetActive(false);

                _hitBox.gameObject.SetActive(true);
                _moveDuration = Random.Range(2f, MOVE_DURATION);
                break;
            case LF_EnemyFighterState.Hurt: 
                AudioSystem.PlaySample(_HurtSound);
                _hurtTime = HURT_TIME;
                _ishurt = false;
            break;
            case LF_EnemyFighterState.Dead: 
                RequestDestroy(1f);
            break;
            case LF_EnemyFighterState.AttackPrep:
                AudioSystem.PlaySample(_PrepAttackSound, 1, true);
                _attackPunchPrepTime = _attackPreparationTimeDelay;
            break;
        }
    }

    protected override void OnStateExit(LF_EnemyFighterState exitedState)
    {
        switch(exitedState){
            case LF_EnemyFighterState.Idle: 
                _startDelay = 0;
            break;
            case LF_EnemyFighterState.Move: break;
            case LF_EnemyFighterState.Attack: break;
            case LF_EnemyFighterState.Hurt: break;
            case LF_EnemyFighterState.Dead: break;
            case LF_EnemyFighterState.AttackPrep: 
            break;
        }
    }
}
