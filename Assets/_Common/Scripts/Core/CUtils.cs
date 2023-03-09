using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CUtils
{

    public static Vector3[] GetWorldCorners(RectTransform rt)
    {
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);
        return v;
    }

    public static Vector3 GetPointInsideRectTransform(RectTransform rt){
        Vector3[] worldCorners = GetWorldCorners(rt);
        return worldCorners[0] - new Vector3(Random.Range(0, rt.rect.x), Random.Range(0, rt.rect.y), 0);
    }

    
    public static void Shuffle<T>(List<T> list){
        for(int i = 0; i < list.Count; i++) {
            int a = Random.Range(0, list.Count);
            int b = Random.Range(0, list.Count);

            T temp = list[a];
            list[a] = list[b];
            list[b] = temp;
        }
    }

}
