using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ESM;

public class DD_Player2 : ESM.SMC_4D<DD_PlayerStates>
{

    private enum LeadingDirection{
        None,
        Horizontal,
        Vertical
    }

    public static DD_Player2 Instance;
    protected Vector2 _inputs = new Vector2();
    protected bool _shoot;

    private LeadingDirection _leadingDirection = LeadingDirection.None;

    [SerializeField] Transform[] rayPoints;
    [SerializeField] Transform[] _debugPoints;

    private void Awake() {
        Instance = this;

        _verticalPoint = DD_Path.GetNextVerticalPoint(transform.position, AnimationSide.Bottom);
        _horizontalPoint = DD_Path.GetNextHorizontalPoint(transform.position, AnimationSide.Right);
    }

    protected override void OnStateEnter(DD_PlayerStates enteredState)
    {
        switch(ActiveState){
            case DD_PlayerStates.Idle:
            break;
            case DD_PlayerStates.Moving:
            break;
        }
    }

    protected override void OnStateExit(DD_PlayerStates exitedState){}

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


    Vector2[] _horizontalPoint;
    Vector2[] _verticalPoint;
    Vector2 _movePoint = new Vector2();
    Vector2 _direction = new Vector2();


    bool _canChangeDirection = true;

    private void GetPoint(){
            switch(_moveDirection){
                case AnimationSide.Left:
                case AnimationSide.Right:
                        _horizontalPoint = DD_Path.GetNextHorizontalPoint(transform.position, _moveDirection);
                        Vector2 temp = _horizontalPoint[0];
                        _horizontalPoint[0] = _horizontalPoint[1];
                        _horizontalPoint[1] = temp;

                        _movePoint = new Vector2(_horizontalPoint[1].x, _verticalPoint[1].y);
                    break;
                case AnimationSide.Bottom:
                case AnimationSide.Top:
                        _verticalPoint   = DD_Path.GetNextVerticalPoint(transform.position, _moveDirection);
                        _movePoint = new Vector2(_horizontalPoint[1].x, _verticalPoint[1].y);
                    break;
            }
    }

    private void ProcessMoveHorizontall(){
        if(!_canChangeDirection && 
            (   _lastMoveDirection == AnimationSide.Top || 
                _lastMoveDirection == AnimationSide.Bottom || 
                _lastMoveDirection == AnimationSide.Common)
        ){ ProcessMoveVertical(); return; }

        MoveHoriozontal();
        GetPoint();


        _canChangeDirection = (_movePoint - (Vector2)transform.position).magnitude < 0.5f;
        Debug.Log(_canChangeDirection);
    }

    private void ProcessMoveVertical(){
        if(!_canChangeDirection &&             
            (   _lastMoveDirection == AnimationSide.Left || 
                _lastMoveDirection == AnimationSide.Right ||
                _lastMoveDirection == AnimationSide.Common)
        ){ ProcessMoveHorizontall(); return; }

        MoveVerticall();
        GetPoint();

        _canChangeDirection = (_movePoint - (Vector2)transform.position).magnitude < 0.5f;
    }

    private void MoveHoriozontal(){
        _lastMoveDirection = _moveDirection;
        _direction = (_moveDirection == AnimationSide.Left) ? Vector2.left : Vector2.right;
    }

    private void MoveVerticall(){
        _lastMoveDirection = _moveDirection;
        _direction = (_moveDirection == AnimationSide.Bottom) ? Vector2.down : Vector2.up;
    }


    protected override void UpdateState()
    {
        ProcessInputsMove();

        if(_inputs.sqrMagnitude > 0){
            if( _moveDirection == AnimationSide.Left || 
                _moveDirection == AnimationSide.Right ) ProcessMoveHorizontall();
            else ProcessMoveVertical();
        }else{
            _direction = Vector2.zero;
        }


/*

        if(_inputs.sqrMagnitude > 0){
            switch(_moveDirection){
                case AnimationSide.Left   : _direction = Vector2.left;  break;
                case AnimationSide.Right  : _direction = Vector2.right; break;
                case AnimationSide.Bottom : _direction = Vector2.down;  break;
                case AnimationSide.Top    : _direction = Vector2.up;    break;
            }
        }
*/
        //if(_lastMoveDirection )








        _debugPoints[0].position = _horizontalPoint[1];
        _debugPoints[1].position = _horizontalPoint[0];
        _debugPoints[2].position = _verticalPoint[1];
        _debugPoints[3].position = _verticalPoint[0];
        _debugPoints[4].position = _movePoint;

        ProcessMove(_direction / _moveSpeed);
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

    AnimationSide _moveDirection     = AnimationSide.Common;
    AnimationSide _lastMoveDirection = AnimationSide.Common;

    private void ProcessInputsMove(){
        _inputs.x = Input.GetAxisRaw("Horizontal");
        _inputs.y = Input.GetAxisRaw("Vertical");
        _shoot    = Input.GetKeyDown(KeyCode.N);

        if(_moveDirection == AnimationSide.Common){
            if(_inputs.x != 0) _moveDirection = (_inputs.x < 0) ? AnimationSide.Left : AnimationSide.Right;
            if(_inputs.y != 0) _moveDirection = (_inputs.y > 0) ? AnimationSide.Top : AnimationSide.Bottom;
        }else if(_moveDirection == AnimationSide.Left || _moveDirection == AnimationSide.Right){
            if(_inputs.x == 0) _moveDirection = AnimationSide.Common;
        }else if(_moveDirection == AnimationSide.Top || _moveDirection == AnimationSide.Bottom){
            if(_inputs.y == 0) _moveDirection = AnimationSide.Common;
        }
    }
}
