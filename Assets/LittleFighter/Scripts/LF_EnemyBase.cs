using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHiveMinded{}
public interface ITakeDamage{
    void TakeDamage(int amount);
}
public interface IDealDamage{
    int GetDamage();
}

public abstract class LF_EnemyBase<StateType> : 
    ESM.SMC_2D<StateType>, 
    IHiveMinded, 
    ITakeDamage,
    IDealDamage
where StateType : System.Enum
{
    private const float VERTICAL_SLOW = 0.5f;

    [SerializeField] private LayerMask _fieldOfView;
    [SerializeField] private string _obstacleTag;
    [SerializeField] private GameObject[] _points;

    class HitParameters{
        public RayPoints[] RelatedDirections;
        public Vector2     Direction;
        public LayerMask   Layer;
        public string      Tag;
        public string      ColliderName;
    }


    protected virtual void Awake() {
//        LF_HiveMind.Register(this);
    }


    protected float _idleTimer;
    protected Vector2 _directions;

    protected void ProcessIdle(){
        _idleTimer -= Time.deltaTime;
    }


    enum RayPoints{
        Right,
        Left,
        Bottom,
        Top,
        RightBottom,
        RightUp,
        LeftUp,
        LeftBottom,
    }


    Dictionary<RayPoints, HitParameters> _hitParameters = new Dictionary<RayPoints, HitParameters>{
        {RayPoints.Left, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Left, RayPoints.LeftUp, RayPoints.LeftBottom},
            Direction = new Vector2(-1, 0),
            Layer = 64,
            Tag = "Obstacle",
            ColliderName = "Solid_Collider"}},
        {RayPoints.Right, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Right, RayPoints.RightUp, RayPoints.RightBottom},
            Direction = new Vector2(1, 0),
            Layer = 64,
            Tag = "Obstacle",
            ColliderName = "Solid_Collider"}},
        {RayPoints.Top, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Top, RayPoints.LeftUp, RayPoints.RightUp},
            Direction = new Vector2(0, 1),
            Layer = 64,
            Tag = "Obstacle",
            ColliderName = "Solid_Collider"}},
        {RayPoints.Bottom, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Bottom, RayPoints.LeftBottom, RayPoints.RightBottom},
            Direction = new Vector2(0, -1),
            Layer = 64,
            Tag = "Obstacle",
            ColliderName = "Solid_Collider"}},
    };

    private void ProcessInputs(){

        bool ResetX = false;
        if(_directions.x > 0) ResetX = CheckHit(RayPoints.Right, 1);
        if(_directions.x < 0) ResetX = CheckHit(RayPoints.Left,  1);
        if(ResetX) _directions.x = 0;

        bool ResetY = false;
        if(_directions.y > 0) ResetX = CheckHit(RayPoints.Top,    0.3f);
        if(_directions.y < 0) ResetX = CheckHit(RayPoints.Bottom, 0.3f);
        if(ResetY) _directions.y = 0;
    }

    public virtual int GetDamage(){ return 0; }
    public virtual void TakeDamage(int amount){}

    private bool CheckHit(RayPoints point, float distance){
        bool returnValue = false;
        HitParameters parameters = _hitParameters[point];
        for(int i = 0; i < parameters.RelatedDirections.Length; i++) {
            returnValue |= IsHit(
                parameters.RelatedDirections[i], 
                parameters.Direction,
                parameters.Layer, 
                parameters.ColliderName,
                parameters.Tag, 
                distance) != null;
        }

        return returnValue;
    }

    private GameObject IsHit(RayPoints point, Vector2 direction, LayerMask layer, string colliderName, string tag, float distance = 1){
        RaycastHit2D hit1 = Physics2D.Raycast(
            _points[(int)point].transform.position, direction, distance, layer);
        Debug.DrawLine(
            (Vector2)_points[(int)point].transform.position,
            (Vector2)_points[(int)point].transform.position + (direction * distance),
            Color.red
        );

        if(hit1){
            if(hit1.transform.CompareTag(tag)) return hit1.transform.gameObject;
            Transform t1 = hit1.transform.GetChild(0).Find(colliderName);
            if(Guard.IsValid(t1) ) return hit1.transform.gameObject;
        }

        return null;
    }
}
