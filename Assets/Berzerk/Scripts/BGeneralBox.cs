using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGeneralBox : MonoBehaviour
{
    [SerializeField] Image _fillBar;
    [SerializeField] GameObject[] _texts;
    [SerializeField] GameObject   _getHimText;

    private float _maxFillValue = 30f;
    private float _perLevelLost = 0.9f;
    private float _minFillValue = 7.5f;

    private static int ActiveText = -2;
    private static float ElapsedTimeToChangeText = 0;

    public void Setup(){
        ActivateText();
        ElapsedTimeToChangeText = 5.0f;
    }

    public void Adjust() {
        if(ActiveText == -2) return;
        if(ActiveText == -1) _getHimText.SetActive(true);
        else{
            DeactivateText();
            _texts[ActiveText].SetActive(true);
        }
    }


    private void Update() {
        
        float maxValue = Mathf.Max( _maxFillValue - (BLevelsManager.CurrentLevel * _perLevelLost ), _minFillValue);
        float percent  = Mathf.Min( (BLevelsManager.Timer - BGeneralBoxController.Retributed)/maxValue, 1.0f);

//        Debug.Log((1.0f - percent) +  " " + maxValue );

        _fillBar.fillAmount = Mathf.Max(0, (1.0f - percent));

        if(BGeneralBoxController.TankSend) {
            DeactivateText();
            ActiveText = -1;
            _getHimText.SetActive(true);
            return;
        } 

        if(_fillBar.fillAmount <= 0.1f){
            if(!BGeneralBoxController.TankSend){
                BGeneralBoxController.TankSend = true;
                Events.Gameplay.RiseEvent(GameplayEventType.SpawnFollower);
                ActiveText = -1;
            }
            return;
        }


        ElapsedTimeToChangeText -= Time.deltaTime;
        if(ElapsedTimeToChangeText < 0){
            ElapsedTimeToChangeText += 5f;
            ActivateText();
        }
    }

    private void DeactivateText(){
        for(int i = 0; i < _texts.Length; i++) {
            _texts[i].SetActive(false);
        }
    }

    private void ActivateText(){
        DeactivateText();
        _getHimText.SetActive(false);
        ActiveText = Random.Range(0, _texts.Length);
        _texts[ActiveText].SetActive(true);
    }
}
