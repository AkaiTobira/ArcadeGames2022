using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle : WallThrought
{

    [SerializeField] float _speed = 10.0f;
    [SerializeField] float _distance = 10.0f;

    Vector3 _forwardDirection;
    public void Setup(Vector3 direcion){
        _forwardDirection = direcion;
        Asteroids.NumberOfMissles += 1;
        AudioSystem.Instance.PlayEffect("Asteroid_Shoot", 1, true);
    }

    override protected void Update()
    {
        base.Update();
        
        Vector3 speed = _forwardDirection * _speed * Time.deltaTime;
        _distance -= speed.magnitude;


        transform.position += speed;



        if(_distance < 0){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag.Contains("cle")){
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        Asteroids.NumberOfMissles -= 1;
    }

}
