using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class BTimers{

    private static List<float> time = new List<float>{ 
          3f,   3f, 
          4f,   
          5f,   5f,
          6f,
          7f,   7f,   7f,
          9f,   9f,   9f,
         10f
    };

    private static bool isRandomized = false;

    private static void RandomizeTimes(){
        if(!isRandomized){
            for(int i = 0; i < time.Count; i++) {
                int a = Random.Range(0, time.Count);
                int b = Random.Range(0, time.Count);

                float temp = time[a];
                time[a] = time[b];
                time[b] = temp;
            }

            isRandomized = true;
        }
    }

    public static float GetTime(){
        RandomizeTimes();
        return time[Random.Range(0, time.Count)];
    }
}

public static class BENEMY_CONSTS{
    public static float SPEED_MULTIPLIER = 0.4f;
    public static float TIME_MULTIPLIER  = 0.99f;
    public static float MISSLE_SPEEDUP   = 0.1f;
    public static float SHOOT_COLDOWN    = 2f;
    public static float SHOT_COLDOWN_MULTIPLIER = 0.4f;
    public static float WALKING_BREAK = 0.2f;

    public static float SHOOTING_COLDOWN(){
        return Mathf.Max(SHOOT_COLDOWN - (SHOT_COLDOWN_MULTIPLIER * BLevelsManager.CurrentLevel), 1.25f);
    }
}

public class BEnemy : ESM.SMC_4D<B_PlayerStates>, IShootable
{
    private Vector2 _inputs;

    private bool    _willShoot;
    private bool    _isDead = true;
    private bool    _canSpawnMissle = true;

    private float   _shootTimer;
    private float   _walkingTimer;
    private float   _activateTimer;
    
    private int _missleCount;

    [SerializeField] BEnemyMissle _misslePrefab;
    [SerializeField] Transform[] _navPoints;

    private void OnEnable() {
        ForceState(B_PlayerStates.Idle, true);
    }

    #region  IShootable
    //Called from outside
    public void IncreaseMissleCounter() { _missleCount++;}
    public void DecreaseMissleCounter() { _missleCount--;}
    #endregion


    public void Initialize(){
        _isDead = true;
        ForceState(B_PlayerStates.Idle, true);
        RequestEnable();

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        box.enabled = true;

        _missleCount = 0;
    }

    private bool SeePlayer(){
        Vector3 positionDifference = Berzerk.Instance.transform.position - transform.position;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, positionDifference.normalized, positionDifference.magnitude);
        Debug.DrawLine(transform.position, transform.position + positionDifference, Color.green);
        for(int i = 0; i < hits.Length; i++) {
            if(hits[i].transform.CompareTag("Obstacle")) return false;
        }

        return true;
    }

    protected override void UpdateState(){
        _shootTimer    -= Time.deltaTime;
        _activateTimer -= Time.deltaTime;
        _walkingTimer  -= Time.deltaTime;
    
        switch (ActiveState) {
            case B_PlayerStates.Idle:  
                {
                    if(SeePlayer()) _activateTimer = -1;
                    if(_activateTimer < 0) CalculateMoveDirections();
                }
            break;
            case B_PlayerStates.Move:  
                {
                    ProcessMove(_inputs);
                    if(Guard.IsValid(Berzerk.Instance)){
                        CalculateMoveDirections();

                        Vector2 change = Berzerk.Instance.transform.position - transform.position;

                        _willShoot = 
                            Mathf.Abs(change.x) < 0.2f ||
                            Mathf.Abs(change.y) < 0.2f;

                        _willShoot |= Mathf.Abs( Mathf.Abs(change.x) - Mathf.Abs(change.y) ) < 0.2f;
                    }
                }
            break;
            case B_PlayerStates.Dead:  break;
            case B_PlayerStates.Shoot: 
                {
                    if(_shootTimer < (BENEMY_CONSTS.SHOOTING_COLDOWN() - 0.5f) && _canSpawnMissle ){
                    //    SpawnMissle();
                        _canSpawnMissle = false;
                    }
                }
            break;
        }
    }

    private float HitRay(int navPointIndex, Vector3 direction, float value){
        RaycastHit2D[] hits = Physics2D.RaycastAll(_navPoints[navPointIndex].position, direction, Mathf.Abs(value));
        Debug.DrawLine(_navPoints[navPointIndex].position, _navPoints[navPointIndex].position + (direction * Mathf.Abs(value)), Color.red);
        for(int i = 0; i < hits.Length; i++) {
            if(hits[i].transform.CompareTag("Obstacle")) return 0;
            if(hits[i].transform.CompareTag("Enemy") && hits[i].transform != transform) return 0;
            if(hits[i].transform.CompareTag("Player")) return 0;
        }

        return value;
    }

    private void CalculateMoveDirections(){
        if(Guard.IsValid(Berzerk.Instance)){
            _inputs = Berzerk.Instance.transform.position - transform.position;

            if(_inputs.magnitude <= 2){
                _inputs = new Vector2();
                _walkingTimer = BENEMY_CONSTS.WALKING_BREAK;
                return;
            }

            if(_inputs.x < 0){
                _inputs.x = HitRay(0, Vector3.left, _inputs.x);
                _inputs.x = HitRay(1, Vector3.left, _inputs.x);
            }else if(_inputs.x > 0){
                _inputs.x = HitRay(2, Vector3.right, _inputs.x);
                _inputs.x = HitRay(3, Vector3.right, _inputs.x);
            }

            if(_inputs.y < 0){
                _inputs.y = HitRay(1, Vector3.down, _inputs.y);
                _inputs.y = HitRay(3, Vector3.down, _inputs.y);
            }else if(_inputs.y > 0){
                _inputs.y = HitRay(0, Vector3.up, _inputs.y);
                _inputs.y = HitRay(2, Vector3.up, _inputs.y);
            }


            if(_inputs.x != 0 && _inputs.y != 0){
                _inputs.x = Mathf.Sign(_inputs.x);
                _inputs.y = Mathf.Sign(_inputs.y);
            }


            _inputs = _inputs.normalized;

        }
    }




    protected override void OnStateEnter(B_PlayerStates enteredState)
    {
        switch (ActiveState) {
            case B_PlayerStates.Idle:
                if(_isDead){
                    _activateTimer = BTimers.GetTime() * Mathf.Max(0.5f, Mathf.Pow(BENEMY_CONSTS.TIME_MULTIPLIER, BLevelsManager.CurrentLevel));
                    _isDead = false;
                }
            break;
            case B_PlayerStates.Shoot:
                _shootTimer = BENEMY_CONSTS.SHOOTING_COLDOWN();
                _canSpawnMissle = true;

            break;
            case B_PlayerStates.Dead:
                RequestDisable(B_CONSTS.DISABLE_AFTER);
                BLevelsManager.Points += 1;
                AudioSystem.PlaySample("Berzerk_EnemyDead", 1, true);
            break;
            default : break;
        }
    }

    protected override Vector3 GetDirectionChange(){
        if(ActiveState == B_PlayerStates.Shoot){
            
            Vector3 difference = Berzerk.Instance.transform.position - transform.position;
            difference.x = Mathf.Abs(difference.x)  > Mathf.Abs(difference.y) ? difference.x : 0;
            difference.y = Mathf.Abs(difference.y) >= Mathf.Abs(difference.x) ? difference.y : 0;

            return difference;
        }
        
        return base.GetDirectionChange();
    }

    private void SpawnMissle(){
        
        Vector2 direction = CalculateMissleDirection();

        BEnemyMissle missle = Instantiate(_misslePrefab, 
            transform.position + (Vector3)(direction * 0.75f), 
            Quaternion.identity).GetComponent<BEnemyMissle>();

        missle.Setup(direction, this);
        (missle.transform as RectTransform).SetParent(transform.parent);
    }

    private Vector2 CalculateMissleDirection(){
        string direction = GetFacingDirection().ToString();
        Vector2 missleDirection = new Vector2();
        
        Vector2 middle = Berzerk.Instance.transform.position - transform.position;

    
        if(direction.Contains("L"))      missleDirection.x -= 1f;
        else if(direction.Contains("R")) missleDirection.x += 1f;

        if(direction.Contains("B"))      missleDirection.y -= 1;
        else if(direction.Contains("T")) missleDirection.y += 1;
    
        
        float angle = Vector2.Angle(middle.normalized, missleDirection);
//        Debug.Log(name + " " + angle);
        if(angle > 40 && angle < 50){
            if( missleDirection.y != 0) missleDirection.x += Mathf.Sign(middle.x);
            else if( missleDirection.x != 0) missleDirection.y += Mathf.Sign(middle.y);

  //          Debug.Log(name + " " + angle + " " + missleDirection);
        }
        //Debug.Log();


    //    missleDirection.x = (Mathf.Abs(missleDirection.x) < 0.5f) ? Mathf.Sign(missleDirection.x): 0;
    //    missleDirection.y = (Mathf.Abs(missleDirection.y) < 0.5f) ? Mathf.Sign(missleDirection.y): 0;

    //    if(missleDirection.x == 0) missleDirection.x = (Mathf.Abs(middle.x) < 0.5f) ? Mathf.Sign(middle.x): 0;
    //    if(missleDirection.y == 0) missleDirection.y = (Mathf.Abs(middle.y) < 0.5f) ? Mathf.Sign(middle.y): 0;
        



    //    Debug.Log( name + " : " + (Berzerk.Instance.transform.position - transform.position) + " " +  missleDirection + " MyPos=" + transform.position + " Target=" + Berzerk.Instance.transform.position);


        return missleDirection.normalized;
    }


    protected override void OnStateExit(B_PlayerStates enteredState)
    {
        switch (ActiveState) {
            default : break;
        }
    }

    protected override B_PlayerStates CheckStateTransitions (){

        switch (ActiveState) {
            case B_PlayerStates.Idle:
                if(_isDead) return B_PlayerStates.Dead;
                else if(_willShoot && _missleCount < B_CONSTS.MISSLE_COUNT && _shootTimer < 0) return B_PlayerStates.Shoot;
                else if(_inputs.magnitude > CONSTS.FLOAT_EPSILON && _walkingTimer < 0) return B_PlayerStates.Move;
                break;
            case B_PlayerStates.Move:
                if(_isDead) return B_PlayerStates.Dead;
                else if(_willShoot && _missleCount < B_CONSTS.MISSLE_COUNT && _shootTimer < 0) return B_PlayerStates.Shoot;
                else if(_inputs.magnitude < CONSTS.FLOAT_EPSILON) return B_PlayerStates.Idle;
                break;
            case B_PlayerStates.Dead: break;
            case B_PlayerStates.Shoot:
                if(_isDead) return B_PlayerStates.Dead;
                else if(_shootTimer < 0 && _inputs.magnitude > CONSTS.FLOAT_EPSILON && _walkingTimer < 0) return B_PlayerStates.Move;
                else if(_shootTimer < 0) return B_PlayerStates.Idle;
                break;
        }

        //_isDead = Input.GetKeyDown(KeyCode.F);

        //_shootingRequirementsMeet = Input.GetKeyDown(KeyCode.M) && _shootingTimeColdown <= 0;
        return ActiveState;
    }

    public void Kill(){
        _isDead = true;
        if(Guard.IsValid(Graphicals)){
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            if(Guard.IsValid(box)) box.enabled = false;
        }
        BGeneralBoxController.Retributed += Mathf.Max(3 * (1.0f - (BLevelsManager.CurrentLevel/20.0f)), 1.0f);
    }

    protected override void ProcessMove(Vector2 directions){
        directions *= BLevelsManager.Timer/50f; //BENEMY_CONSTS.SPEED_MULTIPLIER * BLevelsManager.CurrentLevel;

        ProcessMove_Vertical(directions.y);
        ProcessMove_Horizontal(directions.x);
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy")){
            other.GetComponent<BEnemy>().Kill();
            Kill();
        }else if(other.CompareTag("Player")){
            other.GetComponent<Berzerk>().Kill();
            Kill();
        }else if(other.CompareTag("Missle")){
            Kill();
        }
    }
}
