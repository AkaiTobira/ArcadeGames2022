using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LF_EnemyFighterState{
    Idle,
    Move,
    Attack,
    Hurt,
    Dead,
}


public class LF_EnemyFighter : LF_EnemyBase<LF_EnemyFighterState>
{
    [SerializeField] GameObject _hitBox;
    [SerializeField] GameObject _attackPunchBox;
    [SerializeField] private int _healthPoints = 10;

    private bool _canPunch = false;
    private bool _ishurt;
    

    const float ATTACK_PUNCH_TIME = 1.6f;
    const float HURT_TIME = 0.5f;


    private float _attackPunchTime = ATTACK_PUNCH_TIME;
    private float _hurtTime = HURT_TIME;

    protected override LF_EnemyFighterState CheckStateTransitions()
    {
        switch(ActiveState){
            case LF_EnemyFighterState.Idle:
                if(_healthPoints < 0) return LF_EnemyFighterState.Dead;
                else if(_ishurt) return LF_EnemyFighterState.Hurt;
                else if(_idleTimer > 0) return LF_EnemyFighterState.Idle;
                else if(Random.Range(0.0f, 1.0f) > 0.5f) return LF_EnemyFighterState.Attack;
            break;
            case LF_EnemyFighterState.Move: break;
            case LF_EnemyFighterState.Attack :
                if(_healthPoints < 0) return LF_EnemyFighterState.Dead;
                else if(_ishurt) return LF_EnemyFighterState.Hurt;
                else if(_attackPunchTime < 0) return LF_EnemyFighterState.Idle;
            break;
            case LF_EnemyFighterState.Hurt: 
                if(_healthPoints < 0) return LF_EnemyFighterState.Dead;
                else if(_hurtTime < 0) return LF_EnemyFighterState.Idle;
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
            break;
            case LF_EnemyFighterState.Move: break;
            case LF_EnemyFighterState.Attack : 
                _attackPunchTime -= Time.deltaTime;
                if(_attackPunchTime < ATTACK_PUNCH_TIME/2.0f) _attackPunchBox.SetActive(true);
            break;
            case LF_EnemyFighterState.Hurt: 
                _hurtTime -= Time.deltaTime;
            break;
            case LF_EnemyFighterState.Dead: break;
        }
    }

    public override int GetDamage()
    {
        if(ActiveState == LF_EnemyFighterState.Attack) return 2;
        return 0;
    }

    public override void TakeDamage(int amount)
    {
        if(!_ishurt){
            _healthPoints -= amount;
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
                _attackPunchTime = ATTACK_PUNCH_TIME;
                break;
            case LF_EnemyFighterState.Move: break;
            case LF_EnemyFighterState.Hurt: 
                _hurtTime = HURT_TIME;
                _ishurt = false;
            break;
            case LF_EnemyFighterState.Dead: 
                RequestDisable(1f);
            break;
        }
    }

    protected override void OnStateExit(LF_EnemyFighterState exitedState)
    {
        switch(exitedState){
            case LF_EnemyFighterState.Idle: break;
            case LF_EnemyFighterState.Move: break;
            case LF_EnemyFighterState.Attack: break;
            case LF_EnemyFighterState.Hurt: break;
            case LF_EnemyFighterState.Dead: break;
        }
    }
}
