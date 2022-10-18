using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSorter : MonoBehaviour
{
    [SerializeField] int startingIndex;
    [SerializeField] int maxIndex;

    List< KeyValuePair<Canvas, int>> _canvases = new List<KeyValuePair<Canvas, int>>();

    [SerializeField] List<Canvas> _canvases2 = new List<Canvas>();
    private static CanvasSorter Sorter;

    private void Awake() {
        if(!Guard.IsValid(Sorter)) {
            Sorter = this;
        }
    }

    public static void AddCanvas(Canvas canvas, int keepRelative = 0){
        Sorter._canvases.Add(new KeyValuePair<Canvas, int>(canvas, keepRelative));
    //    Sorter._canvases2.Add(canvas);
    }

    // Update is called once per frame
    void Update()
    {
        if(_canvases?.Count > 0){
        //    _canvases2.Clear();
            List<int> indexesToRemove = new List<int>();
            for(int i = 0; i < _canvases.Count; i++){
                if(!Guard.IsValid(_canvases[i].Key)) indexesToRemove.Add(i);
            }
            indexesToRemove.Reverse();
            for(int i = 0; i < indexesToRemove.Count; i++){
                _canvases.RemoveAt(indexesToRemove[i]);
            }

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
}
