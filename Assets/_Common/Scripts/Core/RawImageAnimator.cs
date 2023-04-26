using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RawImageAnimator : RawAnimator
{
    protected Image _image;
    [SerializeField] private Sprite[] _sprites;

    protected override void Awake() {
        _image = GetComponent<Image>();
        base.Awake();
    }

    protected override void UpdateAnimation(int frame){
        _image.sprite = _sprites[frame];
    }

    protected override int GetFramesCount()
    {
        return _sprites.Length;
    }
}
