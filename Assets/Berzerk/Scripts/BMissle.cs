using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShootable{
    void IncreaseMissleCounter();
    void DecreaseMissleCounter();
}


public abstract class ABMissle : CMonoBehaviour{

    [SerializeField] float _speed = 10.0f;
    [SerializeField] float _distance = 10.0f;
    [SerializeField] string _audioEffect = null;

    Vector3 _forwardDirection;
    IShootable _owner;
    public virtual void Setup(Vector3 direcion, IShootable owner){
        _forwardDirection = direcion;
        _owner = owner;
        _speed *= GetSpeedMultiplier();

        _owner.IncreaseMissleCounter();
    
        if(!string.IsNullOrEmpty(_audioEffect)){
            AudioSystem.Instance.PlayEffect(_audioEffect, 1, true);
        }
    }

    private void Update()
    {
        Vector3 speed = _forwardDirection * _speed * Time.deltaTime;
        _distance -= speed.magnitude;

        transform.position += speed;

        if(_distance < 0){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        OnMissleHit(other);
    }

    private void OnDestroy() {
        _owner?.DecreaseMissleCounter();
    }


    protected abstract void OnMissleHit(Collider2D other);
    protected abstract float GetSpeedMultiplier();
}

public class BMissle : ABMissle
{

    protected override float GetSpeedMultiplier() { return 1; }

    protected override void OnMissleHit(Collider2D other)
    {   
        if(other.CompareTag("Obstacle")){
            Destroy(gameObject);
        }else if(other.CompareTag("Missle")){
            Destroy(gameObject);
        }else if(other.CompareTag("Enemy")){
            Destroy(gameObject);
        }
    }


}
