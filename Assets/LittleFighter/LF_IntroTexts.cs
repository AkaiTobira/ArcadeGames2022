using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LF_IntroTexts : MonoBehaviour
{
    [SerializeField] RectTransform _getReadyText;
    [SerializeField] RectTransform _fightText;
    [SerializeField] RectTransform _powerText;
    [SerializeField] RectTransform _gameOverText;

    [SerializeField] Transform _beginPoint;
    [SerializeField] Transform _endPoint;
    [SerializeField] Transform _middlePoint;


    static private LF_IntroTexts instance;
    private void Awake() { instance = this; }

    void Start()
    {
        TweenManager.Instance.TweenTo(_getReadyText, transform, 1, 
        () => {
            AudioSystem.PlaySample("LittleFighter_GetReady", 1);
            TimersManager.Instance.FireAfter(4, ()=>{
                TweenManager.Instance.TweenTo(_getReadyText, _endPoint, 1);
                TweenManager.Instance.TweenTo(_fightText, transform, 1, 
                () => {
                    AudioSystem.PlaySample("LittleFighter_Fight", 1);
                    TimersManager.Instance.FireAfter(3, ()=>{
                        TweenManager.Instance.TweenTo(_fightText, _endPoint, 1);
                    });
                });
            });

        });
    }

    public static void ShowGameOver(){
        TweenManager.Instance.TweenTo( instance._gameOverText, instance._middlePoint, 0.3f);
    }

    public static void ShowPowerUp(){
        instance._powerText.transform.position = instance._beginPoint.transform.position;
        TweenManager.Instance.TweenTo(
            instance._powerText, instance.transform, 0.3f,
            () => {
                TimersManager.Instance.FireAfter(1, ()=> {
                    TweenManager.Instance.TweenTo(instance._powerText, instance._endPoint, 0.3f);
                });
            }
        );
    }
}
