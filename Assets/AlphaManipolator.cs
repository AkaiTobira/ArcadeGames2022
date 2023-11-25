using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AlphaManipolator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Image image;

    private AlphaStates currentState = AlphaStates.Waiting;
    private static bool showUp;
    private static bool hide;

    enum AlphaStates{
        Waiting,
        Show,
        Hide,
    }

    void Awake(){
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

    void Update()
    {
        switch(currentState){
            case AlphaStates.Waiting:
                if(showUp) currentState = AlphaStates.Show;
                if(hide) currentState = AlphaStates.Hide;
            break;
            case AlphaStates.Show:
                showUp = false;
                
            //    _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, Mathf.Min(_text.color.a + Time.deltaTime *2.0f, 1));
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Min(image.color.a + Time.deltaTime *2.0f, 1));
        
                if(image.color.a >= 0.99f) currentState = AlphaStates.Waiting;
            break;                
            case AlphaStates.Hide:
                hide = false;
        
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, Mathf.Max(_text.color.a - Time.deltaTime *2.0f, 0));
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Max(image.color.a - Time.deltaTime *2.0f, 0));
        
                if(image.color.a <= 0.01f) currentState = AlphaStates.Waiting;
            break;
        }
    }
}
