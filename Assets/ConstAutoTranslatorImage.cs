using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ConstAutoTranslatorImage : AutoTranslatorUnitBase
{
    [SerializeField] int FolderID = 0;
    [SerializeField] GameType game;
    Image _image;

    [SerializeField] List<Sprite> _sprites = new List<Sprite>();

    protected override void Initialize(){
        _image = GetComponent<Image>();
    }

    public override void ForceRefresh(){
        Initialize();

        _sprites = new List<Sprite>();
        for(int i = 0; i < (int)SupportedLanguages.MAX_COUNT; i++){
            _sprites.Add(AutoTranslator.LoadImage(game, FolderID, i)[0]);
        }

        Refresh();
    }

    protected override void Refresh(){
        if(_sprites.Count <= (int)AutoTranslator.Language) return;
        _image.sprite = _sprites[(int)AutoTranslator.Language];
    }
}
