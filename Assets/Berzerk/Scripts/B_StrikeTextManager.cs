
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class B_StrikeTextManager : MonoBehaviour
{
    private static B_StrikeTextManager _instance;
    [SerializeField] B_StrikeText _prefab;

    [SerializeField] GameObject _begin;
    [SerializeField] GameObject _end;
    [SerializeField] GameObject _trnsparent;
    [SerializeField] GameObject[] _texts;

    [SerializeField] Color[] _colors;
    private bool _isTransparentActive = false;

    float timer = 0;

    private void Awake() {
        _instance = this;
    }

    private void SpawnText_Internal(){
        Color randomColor = _colors[Random.Range(0, _colors.Length)];

        for(int i = 0; i < Random.Range(3, BLevelsManager.CurrentLevel + 3); i++){

            B_StrikeText text = Instantiate(
                _prefab, 
                CUtils.GetPointInsideRectTransform(transform as RectTransform),
                Quaternion.Euler(0,0,-90));
            text.GetComponent<TextMeshProUGUI>().color = randomColor;
            (text.transform as RectTransform).SetParent(transform);
        }
    }

    private void SpawnTransparent(){
        if(AutoTranslator.Language == SupportedLanguages.ES) return;
        timer = 0;
       // Debug.LogWarning(_isTransparentActive);

        //if(_isTransparentActive) {
            //MarkTransparentAsInactive();
        //    TimersManager.Instance.FireAfter(1.2f, MarkTransparentAsInactive);
        //    return;
        //}
        _isTransparentActive = true;
        int randomValue = Random.Range(0, _texts.Length);
        for(int i = 0; i < _texts.Length; i++) {
            _texts[i].SetActive(randomValue == i);
        }

        TweenManager.Instance.TweenTo(_trnsparent.transform, _end.transform, 1.0f);
        //TimersManager.Instance.FireAfter(3.0f, MarkTransparentAsInactive);
    }

    private void Update() {
        timer += Time.deltaTime;
        if(timer > 2f && _isTransparentActive){
            MarkTransparentAsInactive();
        }
    }

    private static void MarkTransparentAsInactive(){
        if(Guard.IsValid(_instance)){
            //Debug.LogWarning("DisableEnabled");
            TweenManager.Instance.TweenTo(_instance._trnsparent.transform, _instance._begin.transform, 1.0f);
            _instance._isTransparentActive = false;
            
        }
    }

    public static void SpawnText(){
        if(Guard.IsValid(_instance)){
            _instance.SpawnTransparent();
        }
    }
}
