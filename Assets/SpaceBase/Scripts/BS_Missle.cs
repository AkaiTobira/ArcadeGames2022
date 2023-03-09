using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_Missle : MonoBehaviour
{
    [SerializeField] float _speed = 10.0f;
    [SerializeField] float _distance = 10.0f;
    [SerializeField] string _shotSound = "Asteroid_Shoot";

    Vector3 _forwardDirection;
    public virtual void Setup(Vector3 direcion){
        _forwardDirection = direcion;
        AudioSystem.Instance.PlayEffect(_shotSound, 1, true);
    }

    void Update(){
        Vector3 speed = _forwardDirection * _speed * Time.deltaTime;
        _distance -= speed.magnitude;

        transform.position += speed;

        if(_distance < 0){
            Destroy(gameObject);
        }
    }

}
