using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TimerCount : MonoBehaviour
{
    [SerializeField] public int time = 140;
    [SerializeField] int hurryUpLevel = 140;
    [SerializeField] Color hurryUpColor = Color.red;
    [SerializeField] string _translationTag = "Timer";

    float _elapsedTime = 0;

    TextMeshProUGUI _text;
    private static TimerCount _instance;
    
    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        if(!Guard.IsValid(_instance)) _instance = this;
    }

    public static void Disable(){
        if(Guard.IsValid(_instance)) _instance.enabled = false;
    }


    void Update()
    {
        _elapsedTime -= Time.deltaTime;
        if(_elapsedTime <= 0){
            _elapsedTime += 1.0f;
            time -= 1;

            if(time < 0){
                Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.GameOver, GameOver.TimesUp));
                enabled = false;
                return;
            }

            if(hurryUpLevel > time){
                _text.color = (time % 2 == 1) ? hurryUpColor : Color.white;
                AudioSystem.Instance.PlayEffect("DigDug_Tick", 1);
            }

            string leftTime = ((int)(time / 60)).ToString().PadLeft(2, '0') + ":" + ((int)(time % 60)).ToString().PadLeft(2, '0');
            _text.text = AutoTranslator.Translate(_translationTag, leftTime);// + " : " + leftTime;

        }
    }
}
