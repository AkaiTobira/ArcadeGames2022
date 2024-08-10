using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LF_PlayerHPBar : MonoBehaviour
{
    [SerializeField] Image _fillMainImage;
    [SerializeField] Image _fillRedImage;
    [SerializeField] float _speed;


    private float _targetValue;
    private float _calculatedSpeed;
    private bool _isStoped;

    public void SetupHp(float percent){
        _fillMainImage.fillAmount = percent;
        float change = _fillRedImage.fillAmount - percent;

        _calculatedSpeed = _speed * Mathf.Sign(change);
        _isStoped = false;
    }

    public void SetupHp(float percent, float startingPercent){
        _fillRedImage.fillAmount  = startingPercent;
        SetupHp(percent);
    }

    private void Update() {
        if(!_isStoped){
            float newFillValue = _fillRedImage.fillAmount - (_calculatedSpeed * Time.deltaTime);
            if(_calculatedSpeed > 0){
                if(newFillValue < _fillMainImage.fillAmount){
                    _fillRedImage.fillAmount = _fillMainImage.fillAmount;
                    _isStoped = true;
                }else{
                    _fillRedImage.fillAmount = newFillValue;
                }
            }else{
                if(newFillValue > _fillMainImage.fillAmount){
                    _fillRedImage.fillAmount = _fillMainImage.fillAmount;
                    _isStoped = true;
                }else{
                    _fillRedImage.fillAmount = newFillValue;
                }
            }
        }
    }
}
