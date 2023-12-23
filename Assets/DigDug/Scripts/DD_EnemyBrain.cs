using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.FSM;
using DigDug;
using ESM;
using UnityEngine.AI;
using TMPro;

public enum DD_EnemyActions{
    Idle,
    Walking,
    AfterPlayer,
    ToExit,
    ShadowMoving,
    RanAway,
}

public class DD_EnemyBrain : FSM<DD_EnemyActions>
{
    public DD_EnemyBrain(
        Dictionary<DD_EnemyActions, IState<DD_EnemyActions>> states, 
        DD_EnemyActions startingState) : base(states, startingState){
    }

    public Vector3 GetNextPoint(){
        DD_EnemyBaseState state = _activeState as DD_EnemyBaseState;
        if(Guard.IsValid(state)) return state.GetNextPoint();
        return new Vector2();
    }

    public IState<DD_EnemyActions> GetState(){
        return _activeState;
    }
}

public abstract class DD_EnemyBaseState : IState<DD_EnemyActions>{
    
    protected string Name;
    public abstract Vector3 GetNextPoint();
    protected static Dictionary<AnimationSide, AnimationSide[]> lookUpDirections = 
        new Dictionary<AnimationSide, AnimationSide[]>
        {
            {AnimationSide.Top,     new AnimationSide[] { AnimationSide.Top,    AnimationSide.Left,   AnimationSide.Right}},
            {AnimationSide.Bottom,  new AnimationSide[] { AnimationSide.Bottom, AnimationSide.Left,   AnimationSide.Right}},
            {AnimationSide.Left,    new AnimationSide[] { AnimationSide.Left,   AnimationSide.Top,    AnimationSide.Bottom}},
            {AnimationSide.Right,   new AnimationSide[] { AnimationSide.Right,  AnimationSide.Bottom, AnimationSide.Top}},  
        };

    public DD_EnemyBaseState(DD_Enemy1 obj){
        owner = obj;
    }

    protected DD_Enemy1 owner;
    public override void OnEnter()
    {
        base.OnEnter();

    //    owner.AddToDebugLog(Name, true);
    }

    public override void OnUpdate()
    {
        //Debug.Log(Name + " COŚ ");     
    }

    protected Transform ShootRaycast(AnimationSide side, float raycastLenght, LayerMask layerMask, string[] colliderNames){
        Vector2 shootDirection = SMC.DirectionToVector2(side);
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            owner.transform.position, 
            shootDirection, 
            raycastLenght, 
            layerMask);

//        Debug.Log("Number of hits" + hits.Length + " " + owner.transform.position + " " + shootDirection + " " + raycastLenght + " " + layerMask);
        //string b = "";
        for(int i = 0; i < hits.Length; i++){
            RaycastHit2D hit = hits[i];

       //     if(Guard.IsValid(hit)){
       //         b += i + hit.collider.transform.parent.name + "/" + hit.collider.transform.name + ":" + hit.collider.transform.tag + ", ";
       //     }

            for(int j = 0; j < colliderNames.Length; j++){
                if( hit.collider.name.Contains(colliderNames[j]) || 
                    hit.collider.tag.Contains(colliderNames[j])) {
         //               Debug.Log(b);
                        return hit.collider.transform;
                }
            }
        }
      //  Debug.Log(b);
        return null;
    }


    protected bool IsFacingWall(
        AnimationSide side, 
        float wallCheckRay,
        LayerMask wallMask,
        float wallCheckDistance,
        string[] colliderNames
        ){
            Vector3 center = owner.transform.position;

        //    Debug.DrawLine(center, center + (Vector3)DirectionToVector2(side) * _wallRayLenght, UnityEngine.Color.green);
            RaycastHit2D[] hit2D2 = Physics2D.RaycastAll(center, SMC_4D<DD_PlayerStates>.DirectionToVector2(side), wallCheckRay, wallMask);

         //   string a = hit2D2.Length.ToString()+ " : " + transform.parent.name+"/"+name +" : " + center + " " + side + "\n";
         //   for(int o = 0; o < hit2D2.Length; o++){
         //       RaycastHit2D raycast = hit2D2[o];
          //      a += raycast.collider.transform.parent.name + "/" + raycast.collider.name + ":" + raycast.point +  " " + DirectionToVector2(side) + " " + Vector2.Distance(raycast.point, center) + "\n";
           // }
           // Debug.Log(a);

            string raycastHits = hit2D2.Length.ToString() + " " + center+ "\n";
            for(int i = 0; i < hit2D2.Length; i++){
                RaycastHit2D raycast = hit2D2[i];

                //raycastHits += raycast.collider.transform.parent.name+"/"+raycast.collider.name + ":" + raycast.point + "::" + raycast.collider.transform.position +"\n ";
                for(int j = 0; j < colliderNames.Length; j++){
                 //   raycastHits += transform.parent.name+"//"+name + " :" +side + "::" + raycast.point + " " + center + " "  + raycast.collider.transform.parent.name + "//" + raycast.collider.name + DirectionToVector2(side) + " " + Vector2.Distance(raycast.point, center);
                 //   Debug.Log(raycastHits);
                    if(raycast.collider.name.Contains(colliderNames[j]) && 
                    Vector2.Distance(raycast.point, center) < wallCheckDistance) return true;
                }
            }

          //  Debug.Log(raycastHits);
            return false;
        }

}

public class DD_EnemyToExit : DD_EnemyBaseState
{
    Transform _exit;
    LayerMask _layer;
    Vector2 tempPoint;
    bool onlyOnce = true;

    public DD_EnemyToExit(DD_Enemy1 obj, LayerMask layerMask) : base(obj){ _layer = layerMask; Name = "EXIT"; }

    public override void OnEnter()
    {
        base.OnEnter();
        onlyOnce = true;

        _exit = ShootRaycast(owner.GetFacingDirection(), 20, _layer, new string[]{"Exit"});
        if(!Guard.IsValid(_exit)){
            _exit = ShootRaycast(SMC.ReverseDirection(owner.GetFacingDirection()), 20, _layer, new string[]{"Exit"});
        }

        tempPoint = DD_NavMesh.GetClosestPointExt(owner.transform.position);
    }

    public override Vector3 GetNextPoint()
    {
        return tempPoint;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if(Vector2.Distance(tempPoint, owner.transform.position) < 0.2f && onlyOnce){
            onlyOnce = false;
            if(Guard.IsValid(_exit)){ 
                tempPoint = _exit.position; 
                return;
            }
            tempPoint = owner.transform.position;
        }
    }

    public override DD_EnemyActions Transite(ref bool IsChanged)
    {
        if(Guard.IsValid(_exit)){
            IsChanged = true;
            if(Vector2.Distance(_exit.position, owner.transform.position) < 1) return DD_EnemyActions.RanAway;
        }
        
        if(!Guard.IsValid(_exit)){
            IsChanged = true;
            return DD_EnemyActions.Walking;
        }

        IsChanged = false;
        return DD_EnemyActions.ToExit;
    }
}

public class DD_EnemyPlayerPursit : DD_EnemyIdle
{
    static string[] objectToCollide = new string[]{"GameCon", "Brick"};

    public DD_EnemyPlayerPursit(DD_Enemy1 obj, float stateDuration, LayerMask playerLayer) : base(obj, stateDuration){
        _layer = playerLayer;
        Name = "PURSIT";
    }

    private bool _seePlayer;
    LayerMask _layer;

    Vector2 nextPoint;

    public override void OnEnter()
    {
        base.OnEnter();
        _seePlayer = true;
        AudioSystem.PlaySample("DigDug_Danger");

        nextPoint = DD_NavMesh.GetNextPathPoint(owner.transform.position, DD_Player3.Instance.transform.position);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(Vector2.Distance(owner.transform.position, nextPoint) < 0.05f){
            nextPoint = DD_NavMesh.GetNextPathPoint(owner.transform.position, DD_Player3.Instance.transform.position);    
        }

        _seePlayer = false;
        AnimationSide[] animationSides = lookUpDirections[owner.GetFacingDirection()];
        for(int i = 0; i < animationSides.Length; i++){

            Transform firstHit = ShootRaycast(animationSides[i], 20, _layer, objectToCollide);
            if(Guard.IsValid(firstHit)) _seePlayer = firstHit.tag.Contains("GameCon");
            if(_seePlayer) {
                ResetTimer();
                return;
            }
        }
    }

    public override DD_EnemyActions Transite(ref bool IsChanged){
        if(_elapsedTime <= 0){
            IsChanged = true;
            return DD_EnemyActions.Walking;
        }

        IsChanged = false;
        return DD_EnemyActions.AfterPlayer;
    }

    public override Vector3 GetNextPoint(){ 
        return nextPoint;
    }
}

public class DD_EnemyShadowWalking : DD_EnemyIdle
{
    public DD_EnemyShadowWalking(DD_Enemy1 obj, float stateDuration) : base(obj, stateDuration){}
    bool _canStopThere;
    Vector2 targetPositon;

    public override void OnEnter()
    {
        base.OnEnter();
        Name = "SHADOW";

        targetPositon = DD_NavMesh.GetClosestPointExt(DD_Player3.Instance.transform.position);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _canStopThere = DD_NavMesh.CanStopThere(owner.transform.position);
    }

    public override DD_EnemyActions Transite(ref bool IsChanged){
        if(_elapsedTime <= 0 && _canStopThere){
            IsChanged = true;
            return DD_EnemyActions.Walking;
        }

        if(Vector2.Distance(targetPositon, owner.transform.position) < 0.5f){
            IsChanged = true;
            return DD_EnemyActions.Walking;
        }

        IsChanged = false;
        return DD_EnemyActions.ShadowMoving;
    }

    public override Vector3 GetNextPoint(){  return targetPositon; }
}

public class DD_EnemyIdle : DD_EnemyBaseState
{
    float _stateDuration = 0;
    protected float _elapsedTime   = 0;

    public DD_EnemyIdle(DD_Enemy1 obj, float stateDuration) : base(obj){
        _stateDuration = stateDuration;
    }
    protected void ResetTimer(){ _elapsedTime = _stateDuration; }
    public override void OnEnter()
    {
        base.OnEnter();
        Name = "IDLE";
        _elapsedTime = _stateDuration;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _elapsedTime -= Time.deltaTime;
    }

    public override DD_EnemyActions Transite(ref bool IsChanged){
        if(_elapsedTime <= 0){
            IsChanged = true;
            return DD_EnemyActions.Walking;
        }

        IsChanged = false;
        return DD_EnemyActions.Idle;
    }

    public override Vector3 GetNextPoint(){ return owner.transform.position; }
}

public class DD_EnemyRunedAway : DD_EnemyBaseState
{
    public DD_EnemyRunedAway(DD_Enemy1 obj) : base(obj){}

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override Vector3 GetNextPoint()
    {
        return owner.transform.position;
    }

    public override DD_EnemyActions Transite(ref bool IsChanged)
    {
        IsChanged = false;
        return DD_EnemyActions.RanAway;
    }
}

public class DD_EnemyWalking : DD_EnemyBaseState
{
    ESM.AnimationSide _lookDirection;

    LayerMask _solidMaskLayer;
    LayerMask _uiMaskLayer;

    bool _playerSeen;
    bool _exitSeen;
    bool _canShadowMove;

    AnimationSide _movingDirection = AnimationSide.Common;
    Vector2 nexPosition = new Vector2();
    float _pursitProb = 0;

    public override void OnEnter()
    {
        base.OnEnter();

        _playerSeen    = false;
        _exitSeen      = false;
        _canShadowMove = false;
        _pursitProb    = 0;

        _movingDirection = owner.GetFacingDirection();

    //    SelectDirection();

        nexPosition = DD_NavMesh.GetClosestPointExt(owner.transform.position);
        Vector3 differene = (Vector3)nexPosition - owner.transform.position;

        _movingDirection = 
            Mathf.Abs(differene.x) > Mathf.Abs(differene.y) ?
                differene.x > 0 ? AnimationSide.Left : AnimationSide.Right :
                differene.y > 0 ? AnimationSide.Bottom : AnimationSide.Top;
                 

        Name = "WALKING";
    }

    private void SelectDirection(){
        List<AnimationSide> directions = GetDirectionList();

        nexPosition = DD_NavMesh.GetClosestPointExt(owner.transform.position);
        for(int i = 0; i < directions.Count; i++){
            Vector2 vector2 = DD_NavMesh.GetFarestPointInDirection(nexPosition, directions[i]);
            if(Vector2.Distance(nexPosition, vector2) < 0.4f) continue;

            _movingDirection = directions[i];
            nexPosition = vector2;
            break;
        }
    }

    public DD_EnemyWalking(DD_Enemy1 obj, LayerMask uiLayerMask, LayerMask brickLayerMask) : base(obj)
    {
        _uiMaskLayer    = uiLayerMask;
        _solidMaskLayer = brickLayerMask;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        List<AnimationSide> directions = GetLookingDirectionList();        
        for(int i = 0; i < directions.Count; i++){
            Transform hit = ShootRaycast(directions[i], 20, _uiMaskLayer, new string[]{"Exit", "GameCon", "Brick"});
            if(Guard.IsValid(hit)){
                _playerSeen = hit.tag.Contains("GameCon");
                if(directions[i] == owner.GetFacingDirection()) _exitSeen   = hit.name.Contains("Exit");
            }
        }

        if(Vector2.Distance(nexPosition, owner.transform.position) < 0.1f){//owner.GetDistanceToWall()){
            if(Random.Range(0, 1.0f) < _pursitProb ){
                _canShadowMove = true;
                return;
            }

            _pursitProb += 0.009f;
            SelectDirection();
        }
    }

    public override Vector3 GetNextPoint()
    {
        return nexPosition;
    }

    public override DD_EnemyActions Transite(ref bool IsChanged){
    
        IsChanged = _canShadowMove || _exitSeen || _playerSeen;

        if(_canShadowMove) return DD_EnemyActions.ShadowMoving;
        if(_exitSeen)      return DD_EnemyActions.ToExit;
        if(_playerSeen)    return DD_EnemyActions.AfterPlayer;

        return DD_EnemyActions.Walking;
    }

    private List<AnimationSide> GetLookingDirectionList(){
        switch(_movingDirection){
            case AnimationSide.Common:
                return new List<AnimationSide>{
                    AnimationSide.Bottom, AnimationSide.Top, AnimationSide.Right, AnimationSide.Left
                };
            case AnimationSide.Right:
            case AnimationSide.Left:
                return new List<AnimationSide>{
                    AnimationSide.Bottom, AnimationSide.Top, _movingDirection
                };
            case AnimationSide.Top:
            case AnimationSide.Bottom:
                return new List<AnimationSide>{
                    AnimationSide.Right, AnimationSide.Left, _movingDirection
                };
        }
        return new List<AnimationSide> {_movingDirection};
    }

    private List<AnimationSide> GetDirectionList(){
        switch(_movingDirection){
            case AnimationSide.Common:
                return new List<AnimationSide>{
                    AnimationSide.Bottom, AnimationSide.Top, AnimationSide.Right, AnimationSide.Left
                };
            case AnimationSide.Right:
            case AnimationSide.Left:
                return new List<AnimationSide>{
                    AnimationSide.Bottom, AnimationSide.Top, SMC.ReverseDirection(_movingDirection)
                };
            case AnimationSide.Top:
            case AnimationSide.Bottom:
                return new List<AnimationSide>{
                    AnimationSide.Right, AnimationSide.Left, SMC.ReverseDirection(_movingDirection)
                };
        }

        return new List<AnimationSide> {_movingDirection};
    }
}
