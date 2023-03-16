using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class T_Segment : MonoBehaviour
{

    [SerializeField] Vector3 scalePerTime = new Vector3(0.1f, 0.1f, 0);
    [SerializeField] float time = 1f;
    [SerializeField] float timeToCenter = 5f;
    [SerializeField] AnimationCurve _curve;
    bool _isScaling = false;
    Vector3 savedVector;

    float movingTimer = 0f;
    float repetition = 0f;

    float elapsedTime = 0;

    void Awake(){
        savedVector = scalePerTime;
    }

    void Update()
    {

        elapsedTime += Time.deltaTime;
        movingTimer -= Time.deltaTime;

        if(!_isScaling){
            ScaleManager.Instance.ScaleBy(transform, scalePerTime, time, ReFire);

            if(movingTimer > 0){

                float numberOfRepetition = timeToCenter/time;
                Vector3 move = - transform.position/numberOfRepetition;
                if(time >= movingTimer) move = - transform.position;

                TweenManager.Instance.TweenBy(transform, move, time);
            }else{
                transform.position = new Vector3();
            }

            _isScaling = true;
        }

        if(transform.localScale.x > 100) {
            enabled = false;
            Debug.Log(elapsedTime);
        }
    }

    private void ReFire(){
        _isScaling = false;

        float a = _curve.Evaluate(elapsedTime);

        scalePerTime = new Vector3(a,a, 1);

    //    scalePerTime += new Vector3(0.7f + repetition, 0.7f + repetition, 0);
    //    repetition += 0.2f;
    }

    public void Setup(Vector3 newPosition){
        transform.localScale = new Vector3(1,1,1);
        movingTimer = timeToCenter;
        transform.position = newPosition;
        scalePerTime = savedVector;
        repetition = 0.0f;
        elapsedTime = 0;
        enabled = true;

        GetComponent<Image>().color = new Color( 
            Random.Range(0.0f, 1.0f), 
            Random.Range(0.0f, 1.0f), 
            Random.Range(0.0f, 1.0f), 
            1.0f);
    }
}
