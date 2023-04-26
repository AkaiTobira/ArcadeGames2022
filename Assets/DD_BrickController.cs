using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DD_BrickController : CMonoBehaviour
{
    [SerializeField] GridLayoutGroup _layout;
    [SerializeField] GameObject[] _digPoints;


    void Start()
    {
        CallInTime( 1, () => {
            _layout.enabled = false;
        });
    }

    public Vector3 GetClosetDigPoint(){

        float distance = 999999;
        Vector3 closestPoint = new Vector3();

        for(int i = 0; i < _digPoints.Length; i++) {
            float distance2 = Vector3.Distance(closestPoint, _digPoints[i].transform.position);
            if(distance > distance2){
                distance = distance2;
                closestPoint = _digPoints[i].transform.position;
            }
        }

        return closestPoint;
    }

    public Vector2 GetMininingPoint(ESM.AnimationSide side){
        switch(side){
            case ESM.AnimationSide.Right  : return _digPoints[1].transform.position;
            case ESM.AnimationSide.Top    : return _digPoints[0].transform.position;
            case ESM.AnimationSide.Left   : return _digPoints[3].transform.position;
            case ESM.AnimationSide.Bottom : return _digPoints[2].transform.position;
        }

        return transform.position;
    }


}
