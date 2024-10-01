using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AlphaManipolator : CUpdateMonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] float _speed = 2.0f;
    [SerializeField] protected Image image;

    protected AlphaStates currentState = AlphaStates.Waiting;
    private static bool showUp;
    private static bool hide;

    protected enum AlphaStates{
        Waiting,
        Show,
        Hide,
    }

    protected override void Awake(){
        base.Awake();
        currentState = AlphaStates.Waiting;
    }

    public static void Show(){
        showUp = true;
        hide = false;
    }

    public static void Hide(){
        showUp = false;
        hide = true;    
    }

    protected virtual void OnShow(){}
    protected virtual void OnHide(){}


    public override void CUpdate()
    {
        base.CUpdate();

        switch(currentState){
            case AlphaStates.Waiting:
                if(showUp) {currentState = AlphaStates.Show; OnShow();};
                if(hide)   {currentState = AlphaStates.Hide; OnHide();};
            break;
            case AlphaStates.Show:
                showUp = false;
                
            //    _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, Mathf.Min(_text.color.a + Time.deltaTime *2.0f, 1));
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Min(image.color.a + Time.deltaTime *_speed, 1));
        
                if(image.color.a >= 0.99f) {
                    currentState = AlphaStates.Waiting;
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
                }
            break;                
            case AlphaStates.Hide:
                hide = false;
        
                if(Guard.IsValid(_text)){
                    _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, Mathf.Max(_text.color.a - Time.deltaTime *_speed, 0));
                }

                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Max(image.color.a - Time.deltaTime *_speed, 0));
        
                if(image.color.a <= 0.01f) {
                    currentState = AlphaStates.Waiting;
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
                }
            break;
        }
    }
}
