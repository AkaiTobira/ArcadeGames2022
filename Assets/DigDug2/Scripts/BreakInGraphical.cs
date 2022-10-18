using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakInGraphical : CMonoBehaviour
{
    [SerializeField] bool Left;
    [SerializeField] bool Right;
    [SerializeField] bool Up;
    [SerializeField] bool Bottom;


    [SerializeField] public Floor BLeft;
    [SerializeField] public Floor BRight;
    [SerializeField] public Floor ULeft;
    [SerializeField] public Floor URight;


    [SerializeField] public RectTransform PLeft;
    [SerializeField] public RectTransform PRight;
    [SerializeField] public RectTransform PUp;
    [SerializeField] public RectTransform PDown;

    void Start()
    {
        PLeft.gameObject.SetActive(Left);
        PRight.gameObject.SetActive(Right);
        PUp.gameObject.SetActive(Up);
        PDown.gameObject.SetActive(Bottom);

        CallNextFrame(()=>{
            PostStart();
        });
    }

    void PostStart(){
        if(!BRight || !BLeft)  PDown.gameObject.SetActive(false);
        if(!ULeft  || !URight) PUp.gameObject.SetActive(false);
        if(!ULeft  || !BLeft)  PLeft.gameObject.SetActive(false);
        if(!URight || !BRight) PRight.gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
