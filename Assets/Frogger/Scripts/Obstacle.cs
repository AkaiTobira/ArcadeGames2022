using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private int _size;
    [SerializeField] private float _speed;
    [SerializeField] private float _speedModifier = 0.8f;
    [SerializeField] private GameObject LeftBorder;
    [SerializeField] private GameObject RightBorder;

    private static float _internalSpeedModifier = 0.8f;

    private const float FRAME_DISTANCE = 2.5f;

    private Vector3 _calculetedSpeed;
    
    void Update()
    {
        _calculetedSpeed = FRAME_DISTANCE * _speed * _speedModifier * Vector3.right * Time.deltaTime * _internalSpeedModifier;

        if(_speed < 0  && transform.position.x < LeftBorder.transform.position.x){
            transform.position = new Vector3(RightBorder.transform.position.x, transform.position.y, transform.position.z);
        }
        else if(_speed > 0  && transform.position.x > RightBorder.transform.position.x){
            transform.position = new Vector3(LeftBorder.transform.position.x, transform.position.y, transform.position.z);
        }

        transform.Translate(_calculetedSpeed);
    }
}
