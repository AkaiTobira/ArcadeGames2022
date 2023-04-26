using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MatchCanvasSortingToScale : MonoBehaviour
{
    [SerializeField] int _relativeBegin;
    [SerializeField] Transform _scaleSource;

    Canvas _canvas;

    void Awake(){
        _canvas = GetComponent<Canvas>();
    }

    void Update()
    {
        _canvas.sortingOrder = _relativeBegin + (int)_scaleSource.localScale.x;
    }
}
