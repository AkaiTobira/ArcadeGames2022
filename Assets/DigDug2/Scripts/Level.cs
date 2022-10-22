using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] float rectSize = 7.5f;
    [SerializeField] Floor example1;
    [SerializeField] Floor example2;


    void Start()
    {
        Vector3 transforms = example2.transform.position - example1.transform.position;
        transforms.x = Mathf.Abs(transforms.x);
        transforms.y = Mathf.Abs(transforms.y);
        rectSize = transforms.x;

        Floor[] floors = GetComponentsInChildren<Floor>();

        for(int i = 0; i < floors.Length; i ++){
            Floor flor = floors[i];
            for(int j = 0; j < floors.Length; j++){
                Floor neighbour = floors[j];

                if(neighbour._type != GTerrainType.GTT_Solid) continue;
                if(transforms.magnitude * 1.2f < Vector3.Distance(flor.transform.position, neighbour.transform.position)) continue;

                Debug.Log(
                    neighbour.transform.position + "\n" + 
                    flor.transform.position + "\n" +
                    transforms.x + "\n" +
                    Mathf.Abs((flor.transform.position.x - neighbour.transform.position.x) - transforms.x) + 
                    "   " + Mathf.Abs((flor.transform.position.x - neighbour.transform.position.x) + transforms.x) +
                    "   " + Mathf.Abs((flor.transform.position.y - neighbour.transform.position.y) - transforms.y) + 
                    "   " + Mathf.Abs((flor.transform.position.y - neighbour.transform.position.y) + transforms.y)
                    );

                bool right = ( Mathf.Abs((flor.transform.position.x - neighbour.transform.position.x) + transforms.x) < 0.01f);
                bool left  = ( Mathf.Abs((flor.transform.position.x - neighbour.transform.position.x) - transforms.x) < 0.01f);
                bool up    = ( Mathf.Abs((flor.transform.position.y - neighbour.transform.position.y) + transforms.y) < 0.01f);
                bool down  = ( Mathf.Abs((flor.transform.position.y - neighbour.transform.position.y) - transforms.y) < 0.01f);

                if(left  && !up    && !down) flor.Left  = neighbour;
                if(right && !up    && !down) flor.Right = neighbour;
                if(!left && !right && up   ) flor.Up    = neighbour;
                if(!left && !right && down ) flor.Down  = neighbour;
            }
        };

        for(int i = 0; i < floors.Length; i ++){



        }
    }






    //public  


}
