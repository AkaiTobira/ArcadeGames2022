using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextAssets : MonoBehaviour
{
    [SerializeField] private TextAsset _textFile;
    [NonReorderable][SerializeField] private TMP_FontAsset[] _text;
    
    private static TextAssets instance;

    void Awake() {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(this);
        } 
    }

    public static TMP_FontAsset GetFont(){
        return instance._text[(int)AutoTranslator.Language];
    }

    public static TextAsset Translations => instance?._textFile;
}
