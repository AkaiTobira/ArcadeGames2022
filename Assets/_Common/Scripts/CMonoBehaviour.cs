using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CMonoBehaviour : MonoBehaviour
{
    public void SetActive(bool active){
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
