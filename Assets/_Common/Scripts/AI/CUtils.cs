using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace AI{

        public static class Guard
        {
            public static bool IsValid( GameObject obj ){
                return obj != null && !ReferenceEquals(obj, null);
            }
            public static bool IsValid( Component obj ){
                return obj != null && !ReferenceEquals(obj, null);
            }
        }


        public static class Algotithms{
            
            public interface IComparator<T>{
                bool IsBiggerThan(T a, T b);
            }

            public static void InsertSort<T>(this IList<T> list, IComparator<T> compare){
                for (int i = 0; i < list.Count - 1; i++){
                    for (int j = i + 1; j > 0; j--){
                        T a = list[j];
                        T b = list[j - 1];
                        if(!compare.IsBiggerThan(a, b)){
                            list[j] = b;
                            list[j - 1] = a;
                        }
                    }
                }
            }

            public static void BubbleSort<T>(this IList<T> list, IComparator<T> compare){
                for(int i = 0; i < list.Count; i++) {
                    bool swaped = false;
                    for(int j = 0; j < list.Count - 1; j++){
                        T a = list[j];
                        T b = list[j + 1];
                        if(compare.IsBiggerThan(a, b)){
                            list[j] = b;
                            list[j + 1] = a;
                            swaped = true;
                        }
                    }
                    if(!swaped) break;
                }
            }

            public static void Shuffle<T>(this IList<T> list){
                for(int i = 0; i < list.Count; i++) {
                    int a = UnityEngine.Random.Range(0, list.Count);
                    int b = UnityEngine.Random.Range(0, list.Count);

                    T temp = list[a];
                    list[a] = list[b];
                    list[b] = temp;
                }
            }
        }

        public static class CanvasExtensions {
            public static Vector2 SizeToParent(this RawImage image, float padding = 0) {
                float w = 0, h = 0;
                    var parent = image.transform.parent.GetComponent<RectTransform>();

                var imageTransform = image.GetComponent<RectTransform>();

                // check if there is something to do
                if (image.texture != null) {
                    if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
                    padding = 1 - padding;
                    float ratio = image.texture.width / (float)image.texture.height;
                    var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
                    if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90) {
                        //Invert the bounds if the image is rotated
                        bounds.size = new Vector2(bounds.height, bounds.width);
                    }
                    //Size by height first
                    h = bounds.height * padding;
                    w = h * ratio;
                    if (w > bounds.width * padding) { //If it doesn't fit, fallback to width;
                        w = bounds.width * padding;
                        h = w / ratio;
                    }
                }
                imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
                imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
                return imageTransform.sizeDelta;
            }

            [Obsolete]
            public static IEnumerator LoadAnyImage(string path){
                Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
                WWW www = new WWW("file://" + path);    
                yield return www;
                www.LoadImageIntoTexture(tex);

                Debug.Log("Loaded file://" + path);
            }
        }

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
                return worldCorners[0] - new Vector3(UnityEngine.Random.Range(0, rt.rect.x), UnityEngine.Random.Range(0, rt.rect.y), 0);
            }

            


            public static GameObject FindObjectByName(GameObject root, ref string toFind){
                if(root.gameObject.name.Contains(toFind)) return root;

                for(int i = 0; i < root.transform.childCount; i++) {
                    GameObject child2 = FindObjectByName(root.transform.GetChild(i).gameObject,ref toFind);
                    if(Guard.IsValid(child2)) return child2;
                }

                return null;
            }

            public static void PrintContainer<T>(this IList<T> container){
                string ss = "";
                for(int i = 0; i < container.Count; i++) {
                    ss += i + ": " + container[i].ToString() + "\n";
                }
                Debug.Log(ss);
            }

            public static void GetEnumToIntValues<T>(ref Dictionary<T, int> toSave) where T : System.Enum{
                toSave = new Dictionary<T, int>();
                
                Array values = Enum.GetValues(typeof(T));
                foreach(T type in values){
                    toSave[type] = Convert.ToInt32(type);
                }
            }

            public static void GetIntToEnumValues<T>(ref Dictionary<int, T> toSave) where T : System.Enum{
                toSave = new Dictionary<int, T>();
                
                Array values = Enum.GetValues(typeof(T));
                foreach(T type in values){
                    toSave[Convert.ToInt32(type)] = type;
                }
            }

            public static Color RandomColor(float min = 0.0f, float max = 1.0f){
                return new Color(
                    UnityEngine.Random.Range(min, max), 
                    UnityEngine.Random.Range(min, max), 
                    UnityEngine.Random.Range(min, max));
            }

            public static Color RandomColorGreen(float min = 0.0f, float max = 1.0f){
                return new Color(
                    0, 
                    UnityEngine.Random.Range(min, max), 
                    0);
            }


            public static string FormatTime_MS(int time){
                return  ((int)(time/60)).ToString() + ":" + 
                        ((int)(time%60)).ToString().PadLeft(2,'0');
            }

            public static string FormatTime_HMS(int time){
                return  ((int)(time/3600)).ToString() + ":" + 
                        ((int)(time/60)).ToString().PadLeft(2,'0') + ":" + 
                        ((int)(time%60)).ToString().PadLeft(2,'0');
            }
        }
    

}