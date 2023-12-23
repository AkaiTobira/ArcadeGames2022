using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class AutoTranslatorImage : AutoTranslatorUnitBase
{
    [SerializeField] protected int FolderID = 0;
    [SerializeField] protected GameType game;
    Image _image;

    protected override void Initialize(){
        _image = GetComponent<Image>();
    }

    protected override void Refresh(){
        _image.sprite = AutoTranslator.LoadImage(game, FolderID)[0];
    }
}
