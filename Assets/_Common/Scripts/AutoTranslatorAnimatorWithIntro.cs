using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RawImageAnimatorWithIntro))]
public class AutoTranslatorAnimatorWithIntro : AutoTranslatorImage
{
    [SerializeField] protected int FolderID2 = 0;
    RawImageAnimatorWithIntro _animator;

    protected override void Initialize(){
        base.Initialize();
        _animator = GetComponent<RawImageAnimatorWithIntro>();
    }

    protected override void Refresh(){
        base.Refresh();
        _animator.SetSprites(AutoTranslator.LoadImage(game, FolderID));
        _animator.SetIntroSprites(AutoTranslator.LoadImage(game, FolderID2));
    }
}
