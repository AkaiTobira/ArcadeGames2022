using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class B_CONSTS
{
    public const float FLOAT_EPSILON = 0.01f;
    public const float SHOT_ACTION_COLDOWN = 0.2f;
    public const int MISSLE_COUNT = 5;
    public const float DISABLE_AFTER = 2.0f;
};

public enum B_PlayerStates{
        Idle,
        Move,
        Shoot,
        Dead
}


public class Berzerk : ESM.SMC_8D<B_PlayerStates>, IShootable
{
    private Vector2 _inputs;

    private bool    _willShoot;
    private bool    _isDead;

    private float   _shootTimer;
    
    private int _missleCount;
    public static Berzerk Instance;

    [SerializeField] BMissle _misslePrefab;


    #region  IShootable
    //Called from outside
    public void IncreaseMissleCounter() { _missleCount++;}
    public void DecreaseMissleCounter() { _missleCount--;}
    #endregion


    private void Awake() {
        Instance = this;
    }

    protected override void UpdateState(){
        _shootTimer -= Time.deltaTime;
    
        switch (ActiveState) {
            case B_PlayerStates.Idle:  break;
            case B_PlayerStates.Move:  ProcessMove(_inputs); break;
            case B_PlayerStates.Dead:  break;
            case B_PlayerStates.Shoot: ProcessMove(_inputs); break;
        }
    }

    public void Restart(){
        _isDead = false;
        ForceState(B_PlayerStates.Idle, true);
        SetActive(true);
    }

    protected override void OnStateEnter(B_PlayerStates enteredState)
    {
        switch (ActiveState) {
            case B_PlayerStates.Shoot:
                _shootTimer = B_CONSTS.SHOT_ACTION_COLDOWN;
                SpawnMissle();
            break;
            case B_PlayerStates.Dead:
                RequestDisable(B_CONSTS.DISABLE_AFTER);
            break;
            default : break;
        }
    }

    private void SpawnMissle(){
        
        Vector2 direction = CalculateMissleDirection();

        BMissle missle = Instantiate(_misslePrefab, 
            transform.position + (Vector3)(direction * 0.75f), 
            Quaternion.identity).GetComponent<BMissle>();

        missle.Setup(direction, this);
        (missle.transform as RectTransform).SetParent(transform.parent);
    }

    private Vector2 CalculateMissleDirection(){
        string direction = GetFacingDirection().ToString();
        Vector2 missleDirection = new Vector2(); 
    
        if(direction.Contains("L"))      missleDirection.x -= 1f;
        else if(direction.Contains("R")) missleDirection.x += 1f;

        if(direction.Contains("B"))      missleDirection.y -= 1;
        else if(direction.Contains("T")) missleDirection.y += 1;
        return missleDirection.normalized;
    }


    protected override void OnStateExit(B_PlayerStates enteredState)
    {
        switch (ActiveState) {
            default : break;
        }
    }

    public void Kill(){
        _isDead = true;
        TimersManager.Instance.FireAfter(3f, ()=>{
            BLevelsManager.PlayerDied();
            Restart();
        });
    }

    protected override B_PlayerStates CheckStateTransitions (){

        switch (ActiveState) {
            case B_PlayerStates.Idle:
                if(_isDead) return B_PlayerStates.Dead;
                else if(_willShoot && _missleCount < B_CONSTS.MISSLE_COUNT && _shootTimer < 0) return B_PlayerStates.Shoot;
                else if(_inputs.magnitude > CONSTS.FLOAT_EPSILON) return B_PlayerStates.Move;
                break;
            case B_PlayerStates.Move:
                if(_isDead) return B_PlayerStates.Dead;
                else if(_willShoot && _missleCount < B_CONSTS.MISSLE_COUNT && _shootTimer < 0) return B_PlayerStates.Shoot;
                else if(_inputs.magnitude < CONSTS.FLOAT_EPSILON) return B_PlayerStates.Idle;
                break;
            case B_PlayerStates.Dead: break;
            case B_PlayerStates.Shoot:
                if(_isDead) return B_PlayerStates.Dead;
                else if(_shootTimer < 0 && _inputs.magnitude > CONSTS.FLOAT_EPSILON) return B_PlayerStates.Move;
                else if(_shootTimer < 0) return B_PlayerStates.Idle;
                break;
        }

        _inputs.x = Input.GetAxisRaw("Horizontal");// + _mobileInputs.x;
        _inputs.y = Input.GetAxisRaw("Vertical");//   + _mobileInputs.y;
        _willShoot  = Input.GetKeyDown(KeyCode.Space);
        //_shootingRequirementsMeet = Input.GetKeyDown(KeyCode.LeftShift) && _shootingTimeColdown <= 0;

        return ActiveState;
    }

    protected override void ProcessMove(Vector2 directions){
        ProcessMove_Horizontal(directions.x);
        ProcessMove_Vertical(directions.y);
    }
}