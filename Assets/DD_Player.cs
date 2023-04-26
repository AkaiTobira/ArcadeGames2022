using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DD_PlayerStates{
    Idle,
    Moving,
    Shooting,
    Digging,
    Dead,
}

public class DD_Player : ESM.SMC_4D<DD_PlayerStates>
{

    private enum LeadingDirection{
        None,
        Horizontal,
        Vertical
    }

    public static DD_Player Instance;
    protected Vector2 _inputs = new Vector2();
    protected bool _shoot;

    private LeadingDirection _leadingDirection = LeadingDirection.None;

    [SerializeField] Transform[] rayPoints;
    [SerializeField] Transform[] _debugPoints;

    private void Awake() {
        Instance = this;
    }


    protected override void OnStateEnter(DD_PlayerStates enteredState)
    {

        switch(ActiveState){
            case DD_PlayerStates.Idle:
                _verticalPoint = DD_Path.GetClosestVerticalPoint(transform.position);
            break;
            case DD_PlayerStates.Moving:
                _verticalPoint = DD_Path.GetClosestVerticalPoint(transform.position);
            break;
        }

    }

    protected override void OnStateExit(DD_PlayerStates exitedState)
    {

    }



    private (RaycastHit2D, RaycastHit2D) GetHits(int rayIndex1, int rayIndex2, Vector2 direction){
        Debug.DrawLine(rayPoints[rayIndex1].position, rayPoints[rayIndex1].position + (Vector3)direction);
        Debug.DrawLine(rayPoints[rayIndex2].position, rayPoints[rayIndex2].position + (Vector3)direction);

        return (Physics2D.Raycast(
                    rayPoints[rayIndex1].position,
                    direction, 
                    1), 
                Physics2D.Raycast(
                    rayPoints[rayIndex2].position,
                    direction, 
                    1));
    }


    Vector2 _horizontalPoint = new Vector2();
    Vector2 _verticalPoint   = new Vector2();
    Vector2 _movePoint       = new Vector2();


    protected override void UpdateState()
    {

        ProcessInputsMove();

        if(_inputs.x != 0){
            Vector3 inputs = new Vector3(_inputs.x, 0);
            _horizontalPoint = DD_Path.GetNextHorizontalPoint(
                transform.position + inputs, 
                (_inputs.x < 0) ? ESM.AnimationSide.Left : ESM.AnimationSide.Right);

        //    if(_leadingDirection == LeadingDirection.Vertical){
        //        _horizontalPoint = DD_Path.GetClosestHorizontalPoint(transform.position);
        //    }
        }else{
            _horizontalPoint = DD_Path.GetClosestHorizontalPoint(transform.position);
        }


        if(_inputs.y != 0){
            Vector3 inputs = new Vector3(0, _inputs.y);
            _verticalPoint = DD_Path.GetNextVerticalPoint(
                transform.position + inputs, 
                (_inputs.y > 0) ? ESM.AnimationSide.Top : ESM.AnimationSide.Bottom );

        //    if(_leadingDirection == LeadingDirection.Horizontal){
        //        _verticalPoint = DD_Path.GetClosestVerticalPoint(transform.position);
        //    }
        }else{
            _verticalPoint = DD_Path.GetClosestVerticalPoint(transform.position);
        }

    //    if(_inputs.x == 0){
    //        _movePoint = new Vector2(0, _verticalPoint.y);
    //    }else if(_inputs.y == 0){
    //        _movePoint = new Vector2(_horizontalPoint.x, 0);
    //    }else if(_inputs.x == 0 && _inputs.y == 0){
    //        _movePoint = new Vector2(_horizontalPoint.x, _verticalPoint.y);
    //    }else{
   //         _movePoint = transform.position;
    //    }

        _movePoint = new Vector2(_horizontalPoint.x, _verticalPoint.y);

        _debugPoints[0].position = _movePoint;

        Vector2 moveDifference = _movePoint - (Vector2)transform.position;


        switch(_leadingDirection){
            case LeadingDirection.None: break;
            case LeadingDirection.Horizontal: 
                if(Mathf.Abs(moveDifference.x) < 0.01f){
                    _leadingDirection = LeadingDirection.Vertical;
                }else{
                    moveDifference.y = 0;
                }
            break;
            case LeadingDirection.Vertical: 
                if(Mathf.Abs(moveDifference.y) < 0.01f){
                    _leadingDirection = LeadingDirection.Horizontal;
                }else{
                    moveDifference.x = 0;
                }
            break;
        }

        if(_inputs.magnitude > 0 && moveDifference.magnitude > 0.05f) ProcessMove(moveDifference.normalized / _moveSpeed);
    }

    protected override DD_PlayerStates CheckStateTransitions()
    {
        switch(ActiveState){
            case DD_PlayerStates.Idle:
                if(_inputs.magnitude > 0) return DD_PlayerStates.Moving;
            break;
            case DD_PlayerStates.Moving:
                if(_inputs.magnitude <= 0) return DD_PlayerStates.Idle;
            break;
        }


        return ActiveState;
    }

    private void ProcessInputsMove(){

        _inputs.x = Input.GetAxisRaw("Horizontal");
        _inputs.y = Input.GetAxisRaw("Vertical");
        _shoot    = Input.GetKeyDown(KeyCode.Space);


        switch(_leadingDirection){
            case LeadingDirection.None:
                if(_inputs.x != 0) _leadingDirection = LeadingDirection.Horizontal;
                else if(_inputs.y != 0) _leadingDirection = LeadingDirection.Vertical;
                break;
            case LeadingDirection.Horizontal:
                if(_inputs.x == 0) _leadingDirection = LeadingDirection.None;
                break;
            case LeadingDirection.Vertical:
                if(_inputs.y == 0) _leadingDirection = LeadingDirection.None;
                break;
        }
    }
}
