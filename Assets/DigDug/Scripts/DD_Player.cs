using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ESM;

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
        OnStateEnter(DD_PlayerStates.Idle);
    }

    protected override void OnStateEnter(DD_PlayerStates enteredState)
    {
        switch(ActiveState){
            case DD_PlayerStates.Idle:
                _verticalPoint = DD_Path.GetNextVerticalPoint(transform.position, AnimationSide.Bottom);
                _horizontalPoint = DD_Path.GetNextHorizontalPoint(transform.position, AnimationSide.Right);
            break;
            case DD_PlayerStates.Moving:
                _verticalPoint = DD_Path.GetNextVerticalPoint(transform.position, AnimationSide.Bottom);
                _horizontalPoint = DD_Path.GetNextHorizontalPoint(transform.position, AnimationSide.Right);
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


    protected override void UpdateState()
    {
        ProcessInputsMove();

        if(_fDirection == AnimationSide.Top || _fDirection == AnimationSide.Bottom){
            _verticalPoint = DD_Path.GetNextVerticalPoint(transform.position, _fDirection);

            Vector2 horizontalPoint = new Vector2();

            if(_sDirection == AnimationSide.Left || _sDirection == AnimationSide.Right){
                _horizontalPoint = DD_Path.GetNextHorizontalPoint(transform.position, _sDirection);
                horizontalPoint = _horizontalPoint[0];
            }else if(_sDirection == AnimationSide.Common){
                horizontalPoint = _horizontalPoint[1]; 
                //(_horizontalPoint[0] + _horizontalPoint[1])/2.0f;
            }

            _movePoint = new Vector2(horizontalPoint.x, _verticalPoint[1].y);
        }



        if(_fDirection == AnimationSide.Left || _fDirection == AnimationSide.Right){
            _horizontalPoint = DD_Path.GetNextHorizontalPoint(transform.position, _fDirection);

            Vector2 verticalPoint = new Vector2();
            if(_sDirection == AnimationSide.Top || _sDirection == AnimationSide.Bottom){
                _verticalPoint = DD_Path.GetNextVerticalPoint(transform.position, _sDirection);
                verticalPoint = _verticalPoint[1];
            }else if(_sDirection == AnimationSide.Common){
                verticalPoint = (_verticalPoint[0]);// + _verticalPoint[1])/2.0f;
            }

            _movePoint = new Vector2(_horizontalPoint[1].x, verticalPoint.y);
        }
    
        _debugPoints[0].position = _horizontalPoint[0];
        _debugPoints[1].position = _horizontalPoint[1];
        _debugPoints[2].position = _verticalPoint[0];
        _debugPoints[3].position = _verticalPoint[1];
        _debugPoints[4].position = _movePoint;

        ProcessMove(_inputs.normalized / _moveSpeed);
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


    AnimationSide _fDirection = AnimationSide.Common;
    AnimationSide _sDirection = AnimationSide.Common;

    private void ProcessInputsMove(){
        _inputs.x = Input.GetAxisRaw("Horizontal");
        _inputs.y = Input.GetAxisRaw("Vertical");
        _shoot    = Input.GetKeyDown(KeyCode.Space);


        if(_fDirection == AnimationSide.Left || _fDirection == AnimationSide.Right){
            if(_inputs.x == 0){
                _fDirection = _sDirection;
                _sDirection = AnimationSide.Common;
            }else{
                _fDirection = (_inputs.x > 0) ? AnimationSide.Right : AnimationSide.Left;
                if(_inputs.y != 0) _sDirection = (_inputs.y > 0) ? AnimationSide.Top : AnimationSide.Bottom;
            }
        }

        if(_fDirection == AnimationSide.Top || _fDirection == AnimationSide.Bottom){
            if(_inputs.y == 0){
                _fDirection = _sDirection;
                _sDirection = AnimationSide.Common;
            }else{
                _fDirection = (_inputs.y > 0) ? AnimationSide.Top : AnimationSide.Bottom;
                if(_inputs.x != 0) _sDirection = (_inputs.x > 0) ? AnimationSide.Right : AnimationSide.Left;
            }
        }

        if(_fDirection == AnimationSide.Common){
            if(_inputs.x != 0){
                _fDirection = (_inputs.x > 0) ? AnimationSide.Right : AnimationSide.Left;
                if(_inputs.y != 0) _sDirection = (_inputs.y > 0) ? AnimationSide.Top : AnimationSide.Bottom;
                else _sDirection = AnimationSide.Common;
            }else if(_inputs.y != 0){
                _fDirection = (_inputs.y > 0) ? AnimationSide.Top : AnimationSide.Bottom;
                if(_inputs.x != 0) _sDirection = (_inputs.x > 0) ? AnimationSide.Right : AnimationSide.Left;
                else _sDirection = AnimationSide.Common;
            }
        }
    }
}

/*
        switch(_leadingDirection){
            case LeadingDirection.None:

                if(_inputs.x != 0){
                    _horizontalPoint = DD_Path.GetNextHorizontalPoint(
                        transform.position, 
                        (_inputs.x < 0) ? ESM.AnimationSide.Left : ESM.AnimationSide.Right);
                    _verticalPoint = DD_Path.GetClosestVerticalPoint(transform.position);
                }

                else if(_inputs.y != 0){
                    _verticalPoint = DD_Path.GetNextVerticalPoint(
                        transform.position, 
                        (_inputs.y < 0) ? ESM.AnimationSide.Top : ESM.AnimationSide.Bottom);
                    _horizontalPoint = DD_Path.GetClosestHorizontalPoint(transform.position);
                }

                _movePoint = new Vector2(_horizontalPoint.x, _verticalPoint.y);
                _debugPoints[1].position = _movePoint;
                Vector2 moveDifference = _movePoint - (Vector2)transform.position;



                if(moveDifference.magnitude < 0.1f){
                    if(_inputs.x != 0)      _leadingDirection = LeadingDirection.Horizontal;
                    else if(_inputs.y != 0) _leadingDirection = LeadingDirection.Vertical;
                }

                break;
            case LeadingDirection.Horizontal:
                if(_inputs.x == 0) _leadingDirection = LeadingDirection.None;
                break;
            case LeadingDirection.Vertical:
                if(_inputs.y == 0) _leadingDirection = LeadingDirection.None;
                break;
        }


                if(_inputs.x != 0 && _leadingDirection == LeadingDirection.Horizontal){
            Vector3 inputs = new Vector3(_inputs.x * 0.2f, 0);
            _horizontalPoint = DD_Path.GetNextHorizontalPoint(
                transform.position + inputs, 
                (_inputs.x < 0) ? ESM.AnimationSide.Left : ESM.AnimationSide.Right);

        }else if( _leadingDirection == LeadingDirection.Vertical ){
            _horizontalPoint = DD_Path.GetClosestHorizontalPoint(transform.position);
        }


        if(_inputs.y != 0 && _leadingDirection != LeadingDirection.Horizontal){
            Vector3 inputs = new Vector3(0, _inputs.y * 0.2f);
            _verticalPoint = DD_Path.GetNextVerticalPoint(
                transform.position + inputs, 
                (_inputs.y > 0) ? ESM.AnimationSide.Top : ESM.AnimationSide.Bottom);

        }else if( _leadingDirection == LeadingDirection.Horizontal ){
            _verticalPoint = DD_Path.GetClosestVerticalPoint(transform.position);
        }

        _movePoint = new Vector2(_horizontalPoint.x, _verticalPoint.y);

        _debugPoints[0].position = _movePoint;

        Vector2 moveDifference = _movePoint - (Vector2)transform.position;

        /*

        switch(_leadingDirection){
            case LeadingDirection.None: break;
            case LeadingDirection.Horizontal: 
                if(Mathf.Abs(moveDifference.x) < 0.01f){
                    _leadingDirection = LeadingDirection.Vertical;
                }else{
                //    Vector2 pp = DD_Path.GetClosestVerticalPoint(transform.position);
                //    moveDifference.y = pp.y - transform.position.y;
                }
            break;
            case LeadingDirection.Vertical: 
                if(Mathf.Abs(moveDifference.y) < 0.01f){
                    _leadingDirection = LeadingDirection.Horizontal;
                }else{
                //    Vector2 pp = DD_Path.GetClosestVerticalPoint(transform.position);
                //    moveDifference.x = pp.x - transform.position.x;
                }
            break;
        }
        */

        //if(_inputs.magnitude > 0 && moveDifference.magnitude > 0.05f)
