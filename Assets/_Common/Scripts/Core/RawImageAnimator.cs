using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RawImageAnimator : RawAnimator
{
    Image _image;
    [SerializeField] private Sprite[] _sprites;

    private void Awake() {
        _image = GetComponent<Image>();
    }

    protected override void UpdateAnimation(int frame){
        _image.sprite = _sprites[frame];
    }

    protected override int GetFramesCount()
    {
        return _sprites.Length;
    }
}
