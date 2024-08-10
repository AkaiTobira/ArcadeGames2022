using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CUtils
{

    public static Vector2 RotateVector_Degree(Vector2 v, float angle)
    {
        return RotateVector(v, angle * Mathf.Deg2Rad);
    }


    // angle is in Radian 2 Pi
    public static Vector2 RotateVector(Vector2 v, float angle)
    {
        float _sin = Mathf.Sin(angle);
        float _cos = Mathf.Cos(angle);

        float _x = v.x*_cos - v.y*_sin;
        float _y = v.x*_sin + v.y*_cos;
        return new Vector2(_x,_y);  
    }

    public static Vector3[] GetWorldCorners(RectTransform rt)
    {
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);
        return v;
    }

    public static Vector3 GetPointInsideRectTransform(RectTransform rt){
        Vector3[] worldCorners = GetWorldCorners(rt);
        return worldCorners[0] - new Vector3(Rand(0, rt.rect.x), Rand(0, rt.rect.y), 0);
    }

    public static int Rand(int max){ return Rand(0, max); }
    public static int Rand(int min, int max){ return Random.Range(min, max); }
    public static float Rand(float max){ return Rand(0, max); }
    public static float Rand(float min, float max){ return Random.Range(min, max); }



    public static void Shuffle<T>(List<T> list){
        for(int i = 0; i < list.Count; i++) {
            int a = Rand(list.Count);
            int b = Rand(list.Count);

            T temp = list[a];
            list[a] = list[b];
            list[b] = temp;
        }
    }

    public static GameObject FindObjectByName(GameObject root, ref string toFind){
        if(root.gameObject.name.Contains(toFind)) return root;

        for(int i = 0; i < root.transform.childCount; i++) {
            GameObject child2 = FindObjectByName(root.transform.GetChild(i).gameObject,ref toFind);
            if(Guard.IsValid(child2)) return child2;
        }

        return null;
    }

    public static void PrintContainer<T>(List<T> container){
        string ss = "";
        for(int i = 0; i < container.Count; i++) {
            ss += i + ": " + container[i].ToString() + "\n";
        }
        Debug.Log(ss);
    }

    public static Color GetRandomColor(float minR, float minG, float minB){
        return new Color(
            Rand(minR, 1),
            Rand(minG, 1),
            Rand(minB, 1)
        );
    }

    public static Color GetRandomColor(float min, float max){
        return new Color(
            Rand(min, max),
            Rand(min, max),
            Rand(min, max)
        );
    }

}
