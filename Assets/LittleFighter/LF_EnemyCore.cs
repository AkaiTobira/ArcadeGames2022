using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LF_EnemyCoreState{
    Idle,
    Move,
    Attack,
    Hurt,
    Dead,
    AttackPrep,
    AfterAttackBreak,
}

public class LF_EnemyCore : LF_EnemyBase<LF_EnemyCoreState>
{
    private int _healthPoints = 10;
    private bool _ishurt;

    private float _attackDuration;
    private float _hurtDuration;
    private float _moveDuration;
    private float _attackPrepDuration;
    private float _attackBreakDuration;

    protected override void Awake()
    {
        _enemyLevel = LF_EnemySpawner.EnemyLevel;
        _healthPoints = stats.MaxHealthPoints + (_enemyLevel * stats.AdditionaHpPerLevel);

        base.Awake();
        ForceState(LF_EnemyCoreState.Idle, true);
    }

    public override int GetCurrentHp()
    {
        return _healthPoints;
    }

    public override int GetMaxHp()
    {
        return stats.MaxHealthPoints + (_enemyLevel * stats.AdditionaHpPerLevel);;
    }

    private void PlaySound(string[] sounds, bool randomPitch){
        if(sounds.Length <= 0) return;

        int soundId = Random.Range(0, sounds.Length);
        AudioSystem.PlaySample(sounds[soundId], 1, randomPitch);
    }

    protected override LF_EnemyCoreState CheckStateTransitions()
    {
        switch(ActiveState){
            case LF_EnemyCoreState.Idle:
                if(_healthPoints <= 0) return LF_EnemyCoreState.Dead;
                else if(_ishurt) return LF_EnemyCoreState.Hurt;
                else if(_startDelay > 0) return LF_EnemyCoreState.Idle;
                else if(_idleTimer > 0) return LF_EnemyCoreState.Idle;
                else if(_canStartAttack) return LF_EnemyCoreState.AttackPrep;
                else if(_canMove) return LF_EnemyCoreState.Move;
            break;
            case LF_EnemyCoreState.Move: 
                if(_healthPoints <= 0) return LF_EnemyCoreState.Dead;
                else if(_ishurt) return LF_EnemyCoreState.Hurt;
                else if(_canStartAttack) return LF_EnemyCoreState.AttackPrep;
                else if(_moveDuration < 0) return LF_EnemyCoreState.Idle;
            break;
            case LF_EnemyCoreState.Attack :
                if(_attackDuration < 0) return LF_EnemyCoreState.AfterAttackBreak;
            break;
            case LF_EnemyCoreState.Hurt: 
                if(_healthPoints <= 0) return LF_EnemyCoreState.Dead;
                else if(_hurtDuration < 0) return LF_EnemyCoreState.Idle;
            break;
            case LF_EnemyCoreState.AttackPrep:
                if(_healthPoints <= 0) return LF_EnemyCoreState.Dead;
                else if(_ishurt) return LF_EnemyCoreState.Hurt;
                else if(_canAttack) return LF_EnemyCoreState.Attack;
            break;
            case LF_EnemyCoreState.AfterAttackBreak:
                if(_healthPoints <= 0) return LF_EnemyCoreState.Dead;
                else if(_ishurt) return LF_EnemyCoreState.Hurt;
                else if(_attackBreakDuration < 0) return LF_EnemyCoreState.Idle;
            break;
            case LF_EnemyCoreState.Dead: break;
        }

        return ActiveState;
    }

    protected virtual void UpdateAttack(){}

    protected override void UpdateState()
    {
        switch(ActiveState){
            case LF_EnemyCoreState.Idle: 
                _idleTimer -= Time.deltaTime;
                _startDelay -= Time.deltaTime;

                ProcessMoveRequirements(LF_Player.Player.transform.position);
            break;
            case LF_EnemyCoreState.Move: 
                _moveDuration -= Time.deltaTime;

                ProcessMoveRequirements(LF_Player.Player.transform.position);
                ProcessMove(_directions);
            break;
            case LF_EnemyCoreState.Attack : 
                _attackDuration -= Time.deltaTime;
                if(_attackDuration < stats.AttackHitboxActivationTime) AttackBox.SetActive(true);
                UpdateAttack();
            break;
            case LF_EnemyCoreState.Hurt: 
                _hurtDuration -= Time.deltaTime;
            break;
            case LF_EnemyCoreState.Dead: break;
            case LF_EnemyCoreState.AfterAttackBreak:
                _attackBreakDuration -= Time.deltaTime;
            break;
            case LF_EnemyCoreState.AttackPrep:
                _attackPrepDuration -= Time.deltaTime;
                if(_attackPrepDuration < 0) _canAttack = true;
            break;
        }
    }

    public override int GetDamage()
    {
        PlaySound(stats.hitSounds, true);
        if(ActiveState == LF_EnemyCoreState.Attack) return stats.Damage;
        return 0;
    }

    public override void TakeDamage(int amount, MonoBehaviour source = null)
    {
        if(!_ishurt){

            PlaySound(stats.hurtSounds, true);
            _healthPoints -= amount;
            LF_EnemyHPBarsController.ShowHpBar(stats.Type, this, amount);
            _ishurt = true;
        }
    }

    protected virtual void OnAttackEnter(){}
    protected virtual void OnAttackPrepEnter(){}
    protected virtual void OnAttackBreakEnter(){}

    protected override void OnStateEnter(LF_EnemyCoreState enteredState)
    {
        switch(enteredState){
            case LF_EnemyCoreState.Idle: 
                _idleTimer = 2f;

                AttackBox.gameObject.SetActive(false);
                HitBox.gameObject.SetActive(true);
                break;
            case LF_EnemyCoreState.Attack:
                PlaySound(stats.attackSounds, true);
                _attackDuration = stats.AttackTimeDuration;
                _canAttack = false;
                HitBox.gameObject.SetActive(false);
                OnAttackEnter();
                break;
            case LF_EnemyCoreState.Move: 
                AttackBox.gameObject.SetActive(false);

                HitBox.gameObject.SetActive(true);
                _moveDuration = Random.Range(2f, stats.MoveTimeDuration);
                break;
            case LF_EnemyCoreState.Hurt: 
                _hurtDuration = stats.HitTimeDuration;
                _ishurt = false;
            break;
            
            case LF_EnemyCoreState.AfterAttackBreak:
                PlaySound(stats.afterAttackSound, true);

                _attackBreakDuration = stats.AttackBreakTimeDelay;
                AttackBox.gameObject.SetActive(false);
                HitBox.gameObject.SetActive(true);

                OnAttackBreakEnter();
            break;
            case LF_EnemyCoreState.Dead: 
                RequestDestroy(1f);
            break;
            case LF_EnemyCoreState.AttackPrep:
                PlaySound(stats.beforeAttackSound, true);
                _attackPrepDuration = stats.AttackPreparationTimeDelay;

                OnAttackPrepEnter();
            break;
        }
    }

    protected override void OnStateExit(LF_EnemyCoreState exitedState)
    {
        switch(exitedState){
            case LF_EnemyCoreState.Idle: 
                _startDelay = 0;
            break;
            case LF_EnemyCoreState.Move: break;
            case LF_EnemyCoreState.Attack: break;
            case LF_EnemyCoreState.Hurt: break;
            case LF_EnemyCoreState.Dead: break;
            case LF_EnemyCoreState.AttackPrep: break;
            case LF_EnemyCoreState.AfterAttackBreak: break;
        }
    }
}
