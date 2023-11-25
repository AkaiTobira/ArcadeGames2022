using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
[RequireComponent(typeof(RawImageAnimator))]
public class AutoTranslatorAnimator : AutoTranslatorUnitBase
{
    [SerializeField] int FolderID = 0;
    [SerializeField] GameType game;
    RawImageAnimator _animator;



    protected override void Initialize(){
        _animator = GetComponent<RawImageAnimator>();
    }

    protected override void Refresh(){
        _animator.SetSprites(AutoTranslator.LoadImage(game, FolderID));
    }

}
