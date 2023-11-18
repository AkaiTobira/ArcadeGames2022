using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ESM;

namespace DigDug{
    public abstract class DD_MoveCore<T> : ESM.SMC_4D<T>
    where T : System.Enum
    {
        [SerializeField] Transform[] rayPoints;
        [SerializeField] LayerMask   _blockslayerMask;
        [SerializeField] Transform[] _debugPoints;

        protected Vector2 _direction = new Vector2();
        bool _keepDirection = false;
        Vector2[,] _points = new Vector2[3,3];
        protected Vector2 _inputs = new Vector2();
        protected AnimationSide _pressedDirection  = AnimationSide.Common;
        protected AnimationSide _lastMoveDirection = AnimationSide.Common;

        private AnimationSide _lastHorizontalDirection = AnimationSide.Common;

        protected override void UpdateState(){
            if(_debugPoints.Length == 0) return; 
            for(int i = 0; i < 3; i++) {
                for(int j = 0; j < 3; j++) {
                    _debugPoints[i * 3 + j].position = _points[i,j];
                }
            }
        }

        protected void CalculateDirections(){

            if(_lastMoveDirection != _pressedDirection){
                if(AreAligned(_pressedDirection, _lastMoveDirection)){
                    _lastMoveDirection = _pressedDirection;
                }else{
                    _keepDirection = true;
                }
            }

            if(_keepDirection){
                if(Vector2.SqrMagnitude(_points[1,1] - (Vector2)transform.position) < 0.05f ){
                    _keepDirection = false;
                    _lastMoveDirection = _pressedDirection;
                }
            }

            Vector2 movePoint = GetPoint(_lastMoveDirection);
            Vector2 moveDistance = movePoint - (Vector2)transform.position;
            if(Vector2.SqrMagnitude(moveDistance) < 0.01f) _direction = Vector2.zero;
            else{ _direction = moveDistance.normalized; }
        }

        protected (RaycastHit2D, RaycastHit2D) GetHits(int rayIndex1, int rayIndex2, Vector2 direction){
            float distance = 0.075f;
            Debug.DrawLine(rayPoints[rayIndex1].position, rayPoints[rayIndex1].position + ((Vector3)direction * distance), Color.magenta);
            Debug.DrawLine(rayPoints[rayIndex2].position, rayPoints[rayIndex2].position + ((Vector3)direction * distance), Color.magenta);

            return (Physics2D.Raycast(
                        rayPoints[rayIndex1].position,
                        direction, 
                        distance,
                        _blockslayerMask), 
                    Physics2D.Raycast(
                        rayPoints[rayIndex2].position,
                        direction, 
                        distance,
                        _blockslayerMask));
        }


        protected virtual float GetMoveModifier(){
            return 1f;
        }

        private void FillPoints(){
            const float distance = 1.3f;

            Vector2[] horizonatl = new Vector2[]{
                DD_Path.GetClosestHorizontalPoint(transform.position - new Vector3(distance,0)),
                DD_Path.GetClosestHorizontalPoint(transform.position),
                DD_Path.GetClosestHorizontalPoint(transform.position + new Vector3(distance,0)),
            };

            Vector2[] vertialcs = new Vector2[]{
                DD_Path.GetClosestVerticalPoint(transform.position - new Vector3(0,distance)),
                DD_Path.GetClosestVerticalPoint(transform.position),
                DD_Path.GetClosestVerticalPoint(transform.position + new Vector3(0,distance)),
            };



            for(int i = 0; i < horizonatl.Length; i++) {
                for(int j = 0; j < vertialcs.Length; j++){
                    _points[i,j].x = horizonatl[i].x;
                    _points[i,j].y = vertialcs[j].y;
                }
            }
        }

        private bool AreAligned(ESM.AnimationSide _pressed, ESM.AnimationSide _last){
            switch(_pressed){
                case ESM.AnimationSide.Common:
                    switch(_last){
                        case ESM.AnimationSide.Common: return true;
                        default: return false;
                    }
                case ESM.AnimationSide.Bottom:
                    switch(_last){
                        case ESM.AnimationSide.Top: return true;
                        default: return false;
                    }
                case ESM.AnimationSide.Top:
                    switch(_last){
                        case ESM.AnimationSide.Bottom: return true;
                        default: return false;
                    }
                case ESM.AnimationSide.Left:
                    switch(_last){
                        case ESM.AnimationSide.Right: return true;
                        default: return false;
                    }
                case ESM.AnimationSide.Right:
                    switch(_last){
                        case ESM.AnimationSide.Left: return true;
                        default: return false;
                    }
            }
            return false;
        }

        protected override void Update()
        {
            base.Update();

            if(_inputs.magnitude > 0){
                if(Mathf.Abs(_inputs.x) > 0 && Mathf.Abs(_direction.y) < 0.25)
                    _lastHorizontalDirection = _inputs.x < 0 ? AnimationSide.Left : AnimationSide.Right;

                float zRotationChange = 0;
                if(_direction.y > 0.25f)      zRotationChange = _lastHorizontalDirection == AnimationSide.Left ?  -90: 90;
                else if(_direction.y < -0.25f) zRotationChange = _lastHorizontalDirection == AnimationSide.Left ?  90:-90;
                if(Vector2.SqrMagnitude(_direction) < 0.01f) zRotationChange = 0;

                Vector3 beforeRotationPositon = Graphicals.transform.position;
                Vector3 rotation2 = new Vector3(0,0,zRotationChange);
                Graphicals.transform.rotation = Quaternion.Euler(rotation2);
                Graphicals.transform.position = beforeRotationPositon;
            }
        }

        protected void UpdateMove(){
            FillPoints();
            if(_inputs.sqrMagnitude > 0){
                CalculateDirections();

                ProcessMove( (_direction.normalized / _moveSpeed) * GetMoveModifier());
            }
        }

        private Vector2 GetPoint(ESM.AnimationSide side){
            switch(side){
                case ESM.AnimationSide.Common: return _points[1,1];
                case ESM.AnimationSide.Bottom: return _points[1,0];
                case ESM.AnimationSide.Top:    return _points[1,2];
                case ESM.AnimationSide.Left:   return _points[0,1];
                case ESM.AnimationSide.Right:  return _points[2,1];
            }

            return _points[1,1];
        }
    }
}