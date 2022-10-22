using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FillOrigin{
    Left,
    Right,
}

[RequireComponent(typeof(Image))]
public class ImageFiller : MonoBehaviour
{

//    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _image;
    [SerializeField] private int _keepRelative; 
    [SerializeField] private float _time = 0.2f;

    private float _elapsedTime;
    private float _targetValue;
    private float _change;

    public void Setup(float targetValue, FillOrigin fillOrigin = FillOrigin.Left ){
        _targetValue = Mathf.Max(Mathf.Min(targetValue, 1.0f), 0.0f);
        _elapsedTime = _time;
        _image.fillOrigin = (int)fillOrigin;

        _change = (_targetValue - _image.fillAmount)/_time;
    }

    void Update()
    {
        _image.fillAmount = Mathf.Max(Mathf.Min(_image.fillAmount + _change * Time.deltaTime, 1.0f), 0.0f);
    }
}
