using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameChecker : CUpdateMonoBehaviour
{
    [SerializeField] GameObject[] objects;
    [SerializeField] Transform QRCode;
    [SerializeField] Transform QRCode2;
    [SerializeField] Button _mainMenuButton;
    [SerializeField] Transform centerPoint;
    [SerializeField] Transform bottomPoint;
    [SerializeField] Frogger _frogger;


    bool continued = false;


    void Awake() {
        AudioSystem.Instance.PlayMusic("BG", 1);
    }

    public override void CUpdate()
    {
        if(continued) {

            #if UNITY_ANDROID

            #else
                _mainMenuButton.Select();
            #endif
            
            return;
        }

        for(int i = 0; i < objects.Length; i++) {
            if(!objects[i].activeSelf && objects[i].transform.parent.gameObject.activeSelf) return;
        }

        enabled = false;
        continued = true;
        _frogger.DoNotRespawn = true;
        TweenManager.Instance.TweenTo(QRCode, centerPoint, 1f, () => {
            AudioSystem.Instance.PlayMusic("BG", 0.2f);
            AudioSystem.Instance.PlayEffect("Victory", 1);

            TimersManager.Instance.FireAfter(10, ()=>{
                TweenManager.Instance.TweenTo(QRCode, bottomPoint, 1f);
                
                TweenManager.Instance.TweenTo(QRCode2, centerPoint, 1f, () => {
                _mainMenuButton.Select();
                });
            });

            
        });
        base.CUpdate();
    }
}
