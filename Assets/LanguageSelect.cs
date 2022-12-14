using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSelect : MonoBehaviour
{
    [SerializeField] Sprite[] _sprites;
    [SerializeField] Image _image;

    float _readBreak = 0.2f;
    float _elapsedTime;

    void Start()
    {
        _image.sprite = _sprites[(int)AutoTranslator.Language];
    }

    void Update()
    {
        float horizontalValue = Input.GetAxisRaw("Horizontal");

        _elapsedTime -= Time.deltaTime;
        if(_elapsedTime <= 0 && Mathf.Abs(horizontalValue) >= 0.2f){
            _elapsedTime = _readBreak;

            ChangeLanguage((int)Mathf.Sign(horizontalValue));
        }
    }

    public void OnButtonLeft(){
        ChangeLanguage(-1);
    }

    private void ChangeLanguage(int change){
        if(change == 0) return;

        int translator = (int)AutoTranslator.Language;
        AutoTranslator.Language = (SupportedLanguages)((translator + change)%((int)SupportedLanguages.MAX_COUNT));
        _image.sprite = _sprites[(int)AutoTranslator.Language];

        Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.LocalizationUpdate));
    }

    public void OnButtonRight(){
        ChangeLanguage(1);
    }
}
