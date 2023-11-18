using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_BrickPart : MonoBehaviour
{
    [SerializeField] public DD_BrickController _mainBrick;
    [SerializeField] private int _points;


    private void OnTriggerEnter2D(Collider2D other) {

//        print(other.gameObject.name);

        if(other.name.Contains("Box")) return;

        gameObject.SetActive(false);
        PointsCounter.Score += _points;
    }

}
