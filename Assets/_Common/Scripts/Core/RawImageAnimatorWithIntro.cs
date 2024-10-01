using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RawImageAnimatorWithIntro : RawAnimator
{
    protected Image _image;
    [SerializeField] private Sprite[] _introSprites;
    [SerializeField] private Sprite[] _sprites;

    bool _showIntro = true;

    protected override void Awake() {
        _image = GetComponent<Image>();
        base.Awake();
    }

    protected override void UpdateAnimation(int frame){
        
        if(_showIntro) _image.sprite = _introSprites[frame];
        else _image.sprite = _sprites[frame];

        if(_showIntro && frame == _introSprites.Length-1) _showIntro = false;
    }

    protected override int GetFramesCount()
    {
        if(_showIntro) return _introSprites.Length;
        return _sprites.Length;
    }

    public void SetIntroSprites(Sprite[] sprites){
        _introSprites = sprites;
    }

    public override void SetSprites(Sprite[] sprites)
    {
        _sprites = sprites;
    }
}
