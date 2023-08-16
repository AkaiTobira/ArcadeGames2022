using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_Path : MonoBehaviour
{
    [SerializeField] Transform[] _horizontalPoints;
    [SerializeField] Transform[] _verticalPoints;

    private Transform[] _reversedHorizontalPoints;
    private Transform[] _reversedVerticalPoints;

    private static DD_Path _instance;
    private static IComparator _xBigger = new XBigger();
    private static IComparator _yBigger = new YBigger();
    private static IComparator _xSmaller = new XSmaller();
    private static IComparator _ySmaller = new YSmaller();


    interface IComparator{
        bool Compare(Vector2 x, Vector2 y);
    }

    class XBigger : IComparator{
        public bool Compare(Vector2 x, Vector2 y) { return x.x > y.x; }
    }

    class XSmaller : IComparator{
        public bool Compare(Vector2 x, Vector2 y) { return x.x < y.x; }
    }

    class YBigger : IComparator{
        public bool Compare(Vector2 x, Vector2 y) { return x.y > y.y; }
    }

    class YSmaller : IComparator{
        public bool Compare(Vector2 x, Vector2 y) { return x.y < y.y; }
    }

    private void Awake() {
        _instance = this;

        SortArrays();

        ReverseArray(_horizontalPoints, ref  _reversedHorizontalPoints);
        ReverseArray(_verticalPoints,   ref  _reversedVerticalPoints);
    }

    private void SortArrays(){
        for(int i = 0; i < _horizontalPoints.Length; i++) {
            for(int j = 0; j < _horizontalPoints.Length-1; j++) {
                if(_horizontalPoints[i].position.x < _horizontalPoints[j].position.x){
                    Transform t = _horizontalPoints[i];
                    _horizontalPoints[i] = _horizontalPoints[j];
                    _horizontalPoints[j] = t;
                }
            }
        }

        for(int i = 0; i < _verticalPoints.Length; i++) {
            for(int j = 0; j < _verticalPoints.Length-1; j++) {
                if(_verticalPoints[i].position.y > _verticalPoints[j].position.y){
                    Transform t = _verticalPoints[i];
                    _verticalPoints[i] = _verticalPoints[j];
                    _verticalPoints[j] = t;
                }
            }
        }
    }

    private static void ReverseArray(Transform[] from, ref Transform[] to){
        to = new Transform[from.Length];
        for(int i = 0; i < from.Length; i++) {
            to[from.Length - 1 - i] = from[i];
        }
    }

    public static Vector2 GetClosestHorizontalPoint(Vector2 currentPosition){
        if(Guard.IsValid(_instance)){
            return GetClosestPointX(_instance._horizontalPoints, currentPosition);
        }
        return new Vector2();
    }

    public static Vector2 GetClosestVerticalPoint(Vector2 currentPosition){
        if(Guard.IsValid(_instance)){
            return GetClosestPointY(_instance._verticalPoints, currentPosition);
        }
        return new Vector2();
    }

    private static Vector2 GetClosestPointX(Transform[] array, Vector2 point){
        int nextPointIndex = array.Length-1;
        float distance = 9999999;

        for(int i = 0; i < array.Length; i++){
            float change = Mathf.Abs(array[i].position.x - point.x);
            if(change < distance){
                distance = change;
                nextPointIndex = i;
            }else{
                break;
            }
        }

        return array[nextPointIndex].position;
    }

    private static Vector2 GetClosestPointY(Transform[] array, Vector2 point){
        int nextPointIndex = array.Length-1;
        float distance = 9999999;

        for(int i = 0; i < array.Length; i++){
            float change = Mathf.Abs(array[i].position.y - point.y);
            if(change < distance){
                distance = change;
                nextPointIndex = i;
            }else{
                break;
            }
        }

        return array[nextPointIndex].position;
    }


    public static Vector2[] GetNextHorizontalPoint(Vector2 currentPosition, ESM.AnimationSide direction){
        if(Guard.IsValid(_instance)){

        //    Debug.Log(_instance._reversedHorizontalPoints.Length + " " + _instance._horizontalPoints.Length);

            return GetNextPoint(
                (direction == ESM.AnimationSide.Left) ? 
                    _instance._reversedHorizontalPoints : 
                    _instance._horizontalPoints,
                (direction == ESM.AnimationSide.Left) ? 
                    _xSmaller : 
                    _xBigger, 
                currentPosition);
        }

        return new Vector2[]{ new Vector2(), new Vector2()};
    }

    private static Vector2[] GetNextPoint(Transform[] array, IComparator comparer, Vector2 point){
        int nextPointIndex = array.Length-1;

        for(int i = 0; i < array.Length; i++){
            if(!comparer.Compare(point, array[i].position)){
                nextPointIndex = i;
                break;
            }
        }

        if(nextPointIndex == 0)              return new Vector2[] {array[nextPointIndex].position, array[nextPointIndex].position};
        if(nextPointIndex == array.Length-1) return new Vector2[] {array[nextPointIndex].position, array[nextPointIndex].position};

        return new Vector2[] {array[nextPointIndex-1].position, array[nextPointIndex].position};
    }

    public static Vector2[] GetNextVerticalPoint(Vector2 currentPosition, ESM.AnimationSide direction){
        if(Guard.IsValid(_instance)){
            return GetNextPoint(
                (direction == ESM.AnimationSide.Top) ? 
                    _instance._reversedVerticalPoints : 
                    _instance._verticalPoints,
                (direction == ESM.AnimationSide.Top) ? 
                    _yBigger : 
                    _ySmaller, 
                currentPosition);
        }

        return new Vector2[]{ new Vector2(), new Vector2()};
    }

    public static Vector2[] GetPrevVerticalPoint(Vector2 currentPosition, ESM.AnimationSide direction){
        if(Guard.IsValid(_instance)){
            return GetNextPoint(
                (direction == ESM.AnimationSide.Top) ? 
                    _instance._verticalPoints : 
                    _instance._reversedVerticalPoints,
                (direction == ESM.AnimationSide.Top) ? 
                    _ySmaller : 
                    _yBigger, 
                currentPosition);
        }

        return new Vector2[]{ new Vector2(), new Vector2()};
    }





}
