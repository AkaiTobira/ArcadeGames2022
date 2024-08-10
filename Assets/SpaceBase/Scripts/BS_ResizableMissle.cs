using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_ResizableMissle : BS_Missle
{
    [SerializeField] float _slowDown;
    [SerializeField] GameObject[] _steps;

    private float _maxDistance;

    public void SetParent(MonoBehaviour behaviour){
        for(int i = 0; i < _steps.Length; i++){
            LF_ColliderSide side = _steps[i].GetComponent<LF_ColliderSide>();
            if(Guard.IsValid(side)) side.SetParent(behaviour);
        }
    }

    public override void Setup(Vector3 direcion)
    {
        _maxDistance = _distance;
        base.Setup(direcion);
    }

    protected override void Update(){
        if(!Guard.IsValid(this)) return;


        _speed -= _slowDown * Time.deltaTime;

        float percent = _distance/_maxDistance;
        int index = (int)(_steps.Length * percent);
        
        for(int i = 0; i < _steps.Length; i++) _steps[i].SetActive(index == i); 

        base.Update();
    }
}
