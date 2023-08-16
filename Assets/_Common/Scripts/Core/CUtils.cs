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
            Random.Range(minR, 1),
            Random.Range(minG, 1),
            Random.Range(minB, 1)
        );
    }

    public static Color GetRandomColor(float min, float max){
        return new Color(
            Random.Range(min, max),
            Random.Range(min, max),
            Random.Range(min, max)
        );
    }

}
