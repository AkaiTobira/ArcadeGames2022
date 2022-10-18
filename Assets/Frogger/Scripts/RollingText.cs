using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RollingText : MonoBehaviour
{
    TextMeshProUGUI _text;
    [SerializeField] string _translationTag;
    [SerializeField] float _speed = 0.3f;


    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        text = AutoTranslator.Translate(_translationTag);
    }

    float elapsed = 0;
    [SerializeField] string text = "Wcale nie chcemy zdobywac kosmosu, chcemy tylko rozszerzyc Ziemie do jego granic *** ";
    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed > _speed){
            elapsed -= _speed;

            _text.text = text;

            text += text[0];
            text = text.Substring(1);
        }
    }
}
