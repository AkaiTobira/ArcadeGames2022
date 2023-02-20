using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGeneralBox : MonoBehaviour
{
    [SerializeField] Image _fillBar;
    [SerializeField] GameObject[] _texts;
    [SerializeField] GameObject   _getHimText;

    float changeText = 5f;


    private void Update() {
        
        float maxValue = Mathf.Max( 17.0f - BLevelsManager.CurrentLevel/2.0f, 5f );
        float percent  = Mathf.Min( BLevelsManager.Timer/maxValue, 1.0f);

//        Debug.Log((1.0f - percent) +  " " + maxValue );

        _fillBar.fillAmount = Mathf.Max(0, (1.0f - percent));
        

        if(_fillBar.fillAmount <= 0.1f){
            DeactivateText();
            _getHimText.SetActive(true);
            return;
        }


        changeText -= Time.deltaTime;
        if(changeText < 0){
            changeText += 5f;
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
        _texts[Random.Range(0, _texts.Length)].SetActive(true);
    }
}
