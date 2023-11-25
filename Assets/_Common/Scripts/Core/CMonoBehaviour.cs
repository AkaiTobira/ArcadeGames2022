using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CMonoBehaviour : MonoBehaviour
{
    private string _nameWithParent;

    public string GetNameWithParent(){
        return _nameWithParent; 
    }

    protected virtual void Awake() {
        if(Guard.IsValid(transform.parent)){
            _nameWithParent = transform.parent.name + "/" + transform.name;
        }else{
            _nameWithParent = "root/" + transform.name;
        }
    }

    public void SetActive(bool active){
        if(!Guard.IsValid(gameObject)) return;
        if(active != gameObject.activeSelf) gameObject.SetActive(active);
    }

    protected void CallNextFrame(Func<IEnumerator> function){
        StartCoroutine(function());
    }

    protected void CallNextFrame( Action function ){
        StartCoroutine( Next( new WaitForEndOfFrame(), function) );
    }

    protected void CallAfterFixedUpdate( Action function ){
        StartCoroutine( Next( new WaitForFixedUpdate(), function) );
    }

    protected void CallInTime(float time, Action function){
        StartCoroutine( Next( new WaitForSeconds(time), function));
    }

    private IEnumerator Next( YieldInstruction step,  Action funtcion ){
        yield return step;
        funtcion?.Invoke();
    }
}
