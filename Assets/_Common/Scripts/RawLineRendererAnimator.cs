using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RawLineRendererAnimator : RawAnimator
{
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Texture[] _textures;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void UpdateAnimation(int frame){
        _lineRenderer.material.SetTexture("_MainTex", _textures[frame]);
    }

    protected override int GetFramesCount()
    {
        return _textures.Length;
    }

    public override void SetSprites(Sprite[] sprites)
    {
        throw new System.NotImplementedException();
    }
}
