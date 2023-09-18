using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSorter : MonoBehaviour
{
    [SerializeField] int startingIndex;
    [SerializeField] int maxIndex;

    private static List< KeyValuePair<Canvas, int>> _canvases = new List<KeyValuePair<Canvas, int>>();

    [SerializeField] List<Canvas> _canvases2 = new List<Canvas>();
    private static CanvasSorter Sorter;
    private static List<int> _indexesToRemove = new List<int>();
    private static HashSet<int> _forcedToRemove = new HashSet<int>();

    private void Awake() {
        if(!Guard.IsValid(Sorter)) {
            Sorter = this;
            DontDestroyOnLoad(this);
        }else Destroy(this);
    }

    public static void AddCanvas(Canvas canvas, int keepRelative = 0){
        _canvases.Add(new KeyValuePair<Canvas, int>(canvas, keepRelative));
    //    Sorter._canvases2.Add(canvas);
    }

    public static void RemoveCanvas(Canvas canvas){
        for(int i = 0; i < _canvases.Count; i++) {
            if(canvas == _canvases[i].Key) _forcedToRemove.Add(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_canvases?.Count > 0){
            ClearList();

            _canvases.Sort((x,y) => {
                return (int)((y.Key.transform.position.y - x.Key.transform.position.y) * 100); 
            });

            for(int i = 0; i < _canvases.Count; i++) {
                _canvases[i].Key.sortingOrder = Mathf.Min(startingIndex + i + _canvases[i].Value,maxIndex);
            }

        //    for(int i = 0; i < _canvases.Count; i++){
        //        _canvases2.Add(_canvases[i].Key);
        //    }
        }
    }

    private void ClearList(){

        for(int i = 0; i < _canvases.Count; i++){
            if(!Guard.IsValid(_canvases[i].Key) || _forcedToRemove.Contains(i)) _indexesToRemove.Add(i);
        }
        _indexesToRemove.Reverse();
        for(int i = 0; i < _indexesToRemove.Count; i++){
            _canvases.RemoveAt(_indexesToRemove[i]);
        }
        _indexesToRemove.Clear();
        _forcedToRemove.Clear();
    }
}
