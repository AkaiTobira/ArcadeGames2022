using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHiveMinded{}
public interface ITakeDamage{
    void TakeDamage(int amount, MonoBehaviour source = null);
}
public interface IDealDamage{
    int GetDamage();
}

public interface IHasHpBar{
    int GetMaxHp();
    int GetCurrentHp();
}

public interface IUseDetector{
    void Detected(MonoBehaviour item);
    void SignalLost(MonoBehaviour item);
}

public abstract class LF_EnemyBase<StateType> : 
    ESM.SMC_2D<StateType>, 
    IHiveMinded, 
    ITakeDamage,
    IDealDamage,
    IHasHpBar,
    IUseDetector
where StateType : System.Enum
{
    private const float VERTICAL_SLOW = 0.5f;

    [SerializeField] protected LF_EnemyStats stats;

    [SerializeField] protected GameObject HitBox;
    [SerializeField] protected GameObject AttackBox;
    [SerializeField] protected GameObject[] _points;
    
    protected class HitParameters{
        public RayPoints[] RelatedDirections;
        public Vector2     Direction;
        public LayerMask   Layer;
        public string[]    ColliderName;
    }

    public abstract int GetMaxHp();
    public abstract int GetCurrentHp();

    protected float _startDelay = 0f;
    protected int _enemyLevel;

    public void SignalLost(MonoBehaviour item){
        _canStartAttack = false;
    }

    public void Detected(MonoBehaviour item){
        _canStartAttack = true;
    }

    protected override void Awake() {
        base.Awake();
        LF_EnemySpawner.Counter ++;
        _startDelay = 0;
    }

    protected virtual void OnDestroy() {
        PointsCounter.Score += stats.PointsAquire * (_enemyLevel + 1) ;
        LF_EnemySpawner.Counter --;
    }

    protected float _idleTimer;
    protected Vector2 _directions;
    protected bool _canMove;
    protected bool _canAttack;
    protected bool _canStartAttack;
    protected bool _ignoreDetection;

    protected void ProcessIdle(){
        _idleTimer -= Time.deltaTime;
    }


    protected enum RayPoints{
        Right,
        Left,
        Bottom,
        Top,
        RightBottom,
        RightUp,
        LeftUp,
        LeftBottom,
    }

    protected Dictionary<RayPoints, HitParameters> _hitParameters = new Dictionary<RayPoints, HitParameters>{
        {RayPoints.Left, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Left},//, RayPoints.LeftUp, RayPoints.LeftBottom},
            Direction = new Vector2(1, 0),
            Layer = 64,
            ColliderName = new string[]{"Enviroment_Obstacle","Solid_Collider"}}},
        {RayPoints.Right, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Left},//, RayPoints.LeftUp, RayPoints.LeftBottom},
            Direction = new Vector2(-1, 0),
            Layer = 64,
            ColliderName = new string[]{"Enviroment_Obstacle","Solid_Collider"}}},
        {RayPoints.Top, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Top, RayPoints.LeftUp, RayPoints.RightUp},
            Direction = new Vector2(0, 1),
            Layer = 64,
            ColliderName = new string[]{"Enviroment_Obstacle","Solid_Collider"}}},
        {RayPoints.Bottom, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Bottom, RayPoints.LeftBottom, RayPoints.RightBottom},
            Direction = new Vector2(0, -1),
            Layer = 64,
            ColliderName = new string[]{"Enviroment_Obstacle","Solid_Collider"}}},
    };

    private void ProcessInputs(){

        bool ResetX = false;

        if(GetFacingDirection() == ESM.AnimationSide.Left){
            if(_directions.x < 0) ResetX |= CheckHit(RayPoints.Right, 1);}

        if(GetFacingDirection() == ESM.AnimationSide.Right){
            if(_directions.x > 0) ResetX |= CheckHit(RayPoints.Left, 1);}

        if(ResetX) _directions.x = 0;

        bool ResetY = false;
        if(_directions.y > 0) ResetY = CheckHit(RayPoints.Top,    0.3f);
        if(_directions.y < 0) ResetY = CheckHit(RayPoints.Bottom, 0.3f);
        if(ResetY) _directions.y = 0;
    }

    public abstract int GetDamage();
    public virtual void TakeDamage(int amount, MonoBehaviour source = null){}

    protected virtual void ProcessMoveRequirements(Vector3 target){
        Vector3 distance = target - transform.position;
        _directions = distance.normalized;
        ProcessInputs();

        if(_directions.magnitude > 0.1f) _canMove = true;
    }

    private bool CheckHit(RayPoints point, float distance){
        bool returnValue = false;
        HitParameters parameters = _hitParameters[point];
        for(int i = 0; i < parameters.RelatedDirections.Length; i++) {
            returnValue |= IsHit(
                parameters.RelatedDirections[i], 
                parameters.Direction,
                parameters.Layer, 
                parameters.ColliderName,
                distance) != null;
        }

        return returnValue;
    }

    protected GameObject IsHit(
        RayPoints point, 
        Vector2 direction, 
        LayerMask layer, 
        string[] colliderName, 
        float distance = 1){
            
        RaycastHit2D hit1 = Physics2D.Raycast(
            _points[(int)point].transform.position, direction, distance, layer);
        Debug.DrawLine(
            (Vector2)_points[(int)point].transform.position,
            (Vector2)_points[(int)point].transform.position + (direction * distance),
            Color.yellow
        );

        if(hit1){
            for(int i = 0; i < colliderName.Length; i++) {
                if(hit1.transform.name.Contains(colliderName[i])) return hit1.transform.gameObject;
                GameObject t1 = CUtils.FindObjectByName(hit1.transform.gameObject, ref colliderName[i]);
                if(Guard.IsValid(t1) ) return t1;
            }
        }

        return null;
    }
}
