using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class T_EnemyAnimator : RawImageAnimator
{

    [Serializable]
    class ScaleSpritePair{
        [SerializeField] public int maxScale;
        [SerializeField] public Sprite[] sprites;
    }

    [SerializeField] ScaleSpritePair[] _scaledSprites;
    [SerializeField] Transform _parentObject;
    ScaleSpritePair _activeSprite;

    protected override void Awake()
    {
        _activeSprite = _scaledSprites[0];
        base.Awake();
    }

    protected override void OnEnable()
    {
        _activeSprite = _scaledSprites[0];
        base.OnEnable();
    }

    protected override void UpdateAnimation(int frame){
        _image.sprite = _activeSprite.sprites[frame];
    }

    protected override int GetFramesCount()
    {
        float scale = _parentObject.localScale.x;

        if(scale > _activeSprite.maxScale){
            for(int i = 0; i < _scaledSprites.Length; i++){
                ScaleSpritePair cSprites = _scaledSprites[i];
                if(scale < cSprites.maxScale){
                    _activeSprite = cSprites;
                    break;
                }
            }
        }

        return _activeSprite.sprites.Length;
    }
}
