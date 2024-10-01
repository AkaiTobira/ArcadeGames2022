using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RawImageAnimator))]
public class ConstAutoTranslatorAnimatorWithIntro : AutoTranslatorUnitBase
{
    [Serializable] 
    class SAWrapper{
        [SerializeField] public Sprite[] Sprites;
        [SerializeField] public Sprite[] IntroSprites;
    }

    [SerializeField] int FolderID = 0;
    [SerializeField] int FolderID2 = 0;
    
    [SerializeField] GameType game;
    RawImageAnimatorWithIntro _animator;

    [SerializeField] List<SAWrapper> _sprites = new List<SAWrapper>();

    protected override void Initialize(){
        _animator = GetComponent<RawImageAnimatorWithIntro>();
    }

    protected override void Refresh(){
        if(_sprites.Count <= (int)AutoTranslator.Language) return;
        _animator.SetSprites(_sprites[(int)AutoTranslator.Language].Sprites);
        _animator.SetIntroSprites(_sprites[(int)AutoTranslator.Language].IntroSprites);
    }


    public override void ForceRefresh(){
        Initialize();

        _sprites = new List<SAWrapper>();
        for(int i = 0; i < (int)SupportedLanguages.MAX_COUNT; i++){
            _sprites.Add( new SAWrapper() { 
                Sprites = AutoTranslator.LoadImage(game, FolderID, i),
                IntroSprites = AutoTranslator.LoadImage(game, FolderID2, i),
            });
        }

        Refresh();
    }
}
