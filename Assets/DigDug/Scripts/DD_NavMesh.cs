using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Queue<T>{
    
    public class QueueElement<P>{
        public float Order;
        public P Element;

        public QueueElement<P> Next;
        public QueueElement<P> Prev;
    }

    QueueElement<T> _head = null;
    int count = 0;
    
    public int Count(){ return count;}

    public void Push(float priority, T element){
        count++;
        QueueElement<T> toInsert = new QueueElement<T>(){Order = priority, Element = element, Next = null, Prev = null};

        if(_head == null){
            _head = toInsert;
            return;
        }

        if(_head.Order > priority){
            toInsert.Next = _head;
            _head.Prev = toInsert;
            _head = toInsert;
            return;
        }


        QueueElement<T> head = _head;
        while(head != null){

            QueueElement<T> temp;
            if(priority < head.Order){
                temp = head.Prev;
                head.Prev = toInsert;
                toInsert.Next = head;
                if(temp != null) temp.Next = toInsert;
                toInsert.Prev = temp;
                return;
            }




        //    Debug.Log(head.Order);
        //    if(head.Next.Order > priority){
        //        head.Next.Next = new QueueElement<T>(){Order = priority, Element = element, Next = head.Next.Next};
        //        return;
        //    }


            temp = head;
            head = head.Next;
            if(head == null){
                temp.Next = toInsert;
                toInsert.Prev = temp;
            }
        
        }

//        head.Next = toInsert;
    }

    public T Pop(){
        count --;

        QueueElement<T> head = _head;
        _head = _head.Next;
        return head.Element;
    }

    public T Peek(){
        return _head.Element;
    }

    public void Print(){
        string ss = "";

        QueueElement<T> head = _head;
        while(head != null){
            ss += "[" + head.Order + ", " + head.Element + "]";
            head = head.Next;
        }

        Debug.Log(ss);
    }
}



namespace DigDug {
    public class DD_NavMesh : CMonoBehaviour
    {
        //List<Vector3> points;

        [Serializable]
        class AdditionalNavPoint{
            [SerializeField] public DD_NavPoint dD_NavPoint;
            [SerializeField] public DD_NavPoint leftPoint;
            [SerializeField] public DD_NavPoint topPoint;
            [SerializeField] public DD_NavPoint rightPoint;
            [SerializeField] public DD_NavPoint bottomPoint;
        }

        List<List<int>> neighbourMatrix;
        List<DD_NavPoint> bricks;

        [SerializeField] Transform bricksParent;
        [SerializeField] List<AdditionalNavPoint> additionalNavPoints;

        private static DD_NavMesh instance;

        private void Start() {
            instance = this;

            int verticalSize   = transform.GetChild(0).childCount;
            int horizontalSize = transform.GetChild(1).childCount; 

            int size = verticalSize * horizontalSize;
          //  points = new List<Vector3>();
            neighbourMatrix = new List<List<int>>();
/*
            for(int i = 0; i < verticalSize; i++) {
                Vector3 point = new Vector3();
                for(int j = 0; j < horizontalSize; j++){
                    point.x =  transform.GetChild(0).GetChild(i).position.x;
                    point.y =  transform.GetChild(1).GetChild(j).position.y;
                }
                points.Add(point);
            }
*/
            for(int i = 0; i< size; i++){
                neighbourMatrix.Add( new List<int>{
                    (i - horizontalSize < 0)                 ? -1 : i - horizontalSize,
                    (i + horizontalSize >= size)             ? -1 : i + horizontalSize,
                    (i % horizontalSize == 0)                ? -1 : i - 1,
                    (i % horizontalSize == horizontalSize-1) ? -1 : i + 1,
                });
            }

            bricks = bricksParent.GetComponentsInChildren<DD_NavPoint>(true).ToList();

            for(int i = 0; i < bricks.Count; i++){
                for( int j = 0; j < bricks.Count-1; j++){
                    if(Mathf.Abs(bricks[i].transform.position.x - bricks[j].transform.position.x) < 0.01f){
                        if(Mathf.Abs(bricks[i].transform.position.y - bricks[j].transform.position.y) < 0.01f){
                            continue;
                        }else if(bricks[i].transform.position.y > bricks[j].transform.position.y){
                            DD_NavPoint temp = bricks[i];
                            bricks[i] = bricks[j];
                            bricks[j] = temp;
                        }
                    }else if(bricks[i].transform.position.x > bricks[j].transform.position.x){
                        DD_NavPoint temp = bricks[i];
                        bricks[i] = bricks[j];
                        bricks[j] = temp;
                    }
                }
            }

            for(int i = 0; i < bricks.Count; i++){
                DD_BrickController brick = bricks[i] as DD_BrickController;
                if(Guard.IsValid(brick)){
                    brick._uiGui.text = i.ToString();
                    brick.Recolor(i, verticalSize, horizontalSize);
                }
            }

            for(int i = 0; i < additionalNavPoints.Count; i++){
                AdditionalNavPoint navPoint = additionalNavPoints[i];
                List<int> neigbours = new List<int>{-1,-1,-1,-1};
                for(int j = 0; j < bricks.Count; j++){
                    if(navPoint.rightPoint  == bricks[j]) {
                        neigbours[3] = j;
                        neighbourMatrix[j][2] = bricks.Count;
                    }
                    if(navPoint.leftPoint   == bricks[j]) {
                        neigbours[2] = j;
                        neighbourMatrix[j][3] = bricks.Count;
                    }
                    if(navPoint.topPoint    == bricks[j]){
                        neigbours[0] = j;
                        neighbourMatrix[j][1] = bricks.Count;
                    }
                    if(navPoint.bottomPoint == bricks[j]) {
                        neigbours[1] = j; 
                        neighbourMatrix[j][0] = bricks.Count;
                    }
                }
                bricks.Add(navPoint.dD_NavPoint);
                neighbourMatrix.Add(neigbours);
            }

            CallNextFrame( ()=>{
                Debug.Log(bricks[0].transform.position + " " + bricks[1].transform.position + " " + bricks[neighbourMatrix[0][1]].transform.position);

            });

    //        string bassed = "";
    //        for(int i = 0; i <size; i++){
    //            bassed += i + " : ";
    //            for(int j = 0; j < 4; j++){
    //                bassed += neighbourMatrix[i, j] + " ,";
    //            }
    //            bassed += "\n";
    //        }

    //        Debug.Log(bassed);
            GetNextPathPoint_internal(Vector3.down, Vector3.down);
        }

        public static Vector3 GetClosestPointExt(Vector3 point){
            if(Guard.IsValid(instance)){
                return instance.bricks[instance.GetClosestPoint(point)].transform.position;
            }

            return point;
        }


        private int GetClosestPoint(Vector3 point){
            
            int found = -1;
            float distance = 9999999;
            for(int i = 0; i < bricks.Count; i++) {
                float dis = Vector3.Distance(point, bricks[i].transform.position);
                if(distance > dis){
                    found = i;
                    distance = dis;
                }
            }

            return found;
        }

        public Vector3 GetNextPathPoint_internal(Vector3 start, Vector3 target){

            Queue<int> queue = new Queue<int>();
            int startingPoint = GetClosestPoint(start);
            int endingPoint   = GetClosestPoint(target);

            Dictionary<int, int>   _pathConections = new Dictionary<int, int>();
            Dictionary<int, float> _pathCost = new Dictionary<int, float>();
            Dictionary<int, float> _someOtherDict = new Dictionary<int, float>();

            _pathCost[startingPoint] = 0;
            _someOtherDict[startingPoint] = 0;
            
            queue.Push(0, startingPoint);

            while(queue.Count() > 0){

                int point = queue.Pop();

                if(point == endingPoint) return ReconstructPath(_pathConections, endingPoint, startingPoint);

                for(int i = 0; i < 4; i++){
    //                Debug.Log(point);
                    int n_index = neighbourMatrix[point][i];
                    if( n_index != -1){

                        float cost = _pathCost[point] + bricks[n_index].GetWalkWeight() + (Vector3.Distance(target, bricks[n_index].transform.position) * 0.2f);
                        if(!_pathCost.ContainsKey(n_index) || _pathCost[n_index] > cost){

                            _pathConections[n_index] = point;
                            _pathCost[n_index] = cost;


                            queue.Push(cost, neighbourMatrix[point][i]);
                        }
                    };
                }
            }
            return new Vector3();
        }   

        Vector3 ReconstructPath(Dictionary<int, int> _pathConections, int target, int source){
            int whileCounter = 30;
            int last = target;

    //        string ss = "";
            while(whileCounter > 0){
                whileCounter--;

    //            ss += last + ", ";

                if(_pathConections.TryGetValue(last, out int value)){
                    if(value == source){
                        
    //                    Debug.Log(ss);
                        return bricks[last].transform.position;
                    }
                    
                    last = value;
                }
            }

            return bricks[source].transform.position;
        }


        public static Vector3 GetNextPathPoint(Vector3 start, Vector3 target){
            if(Guard.IsValid(instance)){
                return instance.GetNextPathPoint_internal(start, target);
            }
            return start;
        }
    }
}
