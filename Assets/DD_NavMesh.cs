using System.Collections;
using System.Collections.Generic;
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
    public class DD_NavMesh : MonoBehaviour
    {
        Vector3[] points;
        int[,] neighbourMatrix;
        DD_BrickController[] bricks;

        [SerializeField] Transform bricksParent;

        private static DD_NavMesh instance;

        private void Start() {
            instance = this;

            int verticalSize   = transform.GetChild(0).childCount;
            int horizontalSize = transform.GetChild(1).childCount; 

            int size = verticalSize * horizontalSize;
            points = new Vector3[size];
            neighbourMatrix = new int[size, 4];

            for(int i = 0; i < verticalSize; i++) {
                for(int j = 0; j < horizontalSize; j++){
                    points[i* horizontalSize + j].x =  transform.GetChild(0).GetChild(i).position.x;
                    points[i* horizontalSize + j].y =  transform.GetChild(1).GetChild(j).position.y;
                }
            }

            for(int i = 0; i< size; i++){
                neighbourMatrix[i, 0] = (i - horizontalSize < 0)                 ? -1 : i - horizontalSize;
                neighbourMatrix[i, 1] = (i + horizontalSize >= size)             ? -1 : i + horizontalSize;
                neighbourMatrix[i, 2] = (i % horizontalSize == 0)                ? -1 : i - 1;
                neighbourMatrix[i, 3] = (i % horizontalSize == horizontalSize-1) ? -1 : i + 1;
            }

            bricks = bricksParent.GetComponentsInChildren<DD_BrickController>(true);

            for(int i = 0; i < bricks.Length; i++){
                for( int j = 0; j < bricks.Length-1; j++){
                    if(Mathf.Abs(bricks[i].transform.position.x - bricks[j].transform.position.x) < 0.01f){
                        if(Mathf.Abs(bricks[i].transform.position.y - bricks[j].transform.position.y) < 0.01f){
                            continue;
                        }else if(bricks[i].transform.position.y > bricks[j].transform.position.y){
                            DD_BrickController temp = bricks[i];
                            bricks[i] = bricks[j];
                            bricks[j] = temp;
                        }
                    }else if(bricks[i].transform.position.x > bricks[j].transform.position.x){
                        DD_BrickController temp = bricks[i];
                        bricks[i] = bricks[j];
                        bricks[j] = temp;
                    }
                }
            }

            for(int i = 0; i < bricks.Length; i++){
                bricks[i]._uiGui.text = i.ToString();
            }


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

        private int GetClosestPoint(Vector3 point){
            
            int found = -1;
            float distance = 9999999;
            for(int i = 0; i < bricks.Length; i++) {
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
                    int n_index = neighbourMatrix[point, i];
                    if( n_index != -1){

                        float cost = _pathCost[point] + bricks[n_index].GetWalkWeight() + (Vector3.Distance(target, bricks[n_index].transform.position) * 0.2f);
                        if(!_pathCost.ContainsKey(n_index) || _pathCost[n_index] > cost){

                            _pathConections[n_index] = point;
                            _pathCost[n_index] = cost;


                            queue.Push(cost, neighbourMatrix[point, i]);
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
