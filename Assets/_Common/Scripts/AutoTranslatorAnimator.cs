using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RawImageAnimator))]
public class AutoTranslatorAnimator : AutoTranslatorImage
{
    RawImageAnimator _animator;

    protected override void Initialize(){
        base.Initialize();
        _animator = GetComponent<RawImageAnimator>();
    }

    protected override void Refresh(){
        base.Refresh();
        _animator.SetSprites(AutoTranslator.LoadImage(game, FolderID));
    }
}
