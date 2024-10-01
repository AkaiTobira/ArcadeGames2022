using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum G_PlayerAnims{
    Idle,
    Shot,
}

public class G_PlayerAnimator : CUpdateMonoBehaviour
{
    [SerializeField] Sprite[] _idleAnim;
    [SerializeField] Sprite[] _shotAnim;
    [SerializeField] float _frameDuration = 0.2f;

    Image _image;
    float _elapsedTime;
    int _currentFrame;
    G_PlayerAnims _currentAnim;
    
    protected override void Awake()
    {
        base.Awake();
        _image = transform.GetChild(0).GetComponent<Image>();
    }

    public override void CUpdate()
    {
        base.CUpdate();

        _elapsedTime -= Time.deltaTime;
        if(_elapsedTime < 0){
            _elapsedTime += _frameDuration;

            switch(_currentAnim){
                case G_PlayerAnims.Idle: 
                    {
                        _currentFrame = ( _currentFrame + _idleAnim.Length + 1 )%_idleAnim.Length; 
                        _image.sprite = _idleAnim[_currentFrame];    
                    }
                    break;
                case G_PlayerAnims.Shot:
                    if(_currentFrame < _shotAnim.Length){
                        _image.sprite = _shotAnim[_currentFrame++];    
                    }else{
                        _currentFrame = -1;
                        _currentAnim = G_PlayerAnims.Idle;
                    }
                break;
            }
        }
    }

    public void SetAnim(G_PlayerAnims anim){
        _currentFrame = 0;
        _currentAnim = anim;
    }
}
