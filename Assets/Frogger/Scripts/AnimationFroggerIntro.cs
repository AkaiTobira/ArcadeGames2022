using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFroggerIntro : MonoBehaviour
{

    [SerializeField] Transform _center;
    [SerializeField] Transform _bottom;
    [SerializeField] Transform _block;
    [SerializeField] SceneLoader _loader;

    


    void Start()
    {
        BringMiddle();
    }

    void BringMiddle(){
        TweenManager.Instance.TweenTo(_block, _center, 1f);
        TimersManager.Instance.FireAfter(10f, BringBottom);
    }

    void BringBottom(){
        TweenManager.Instance.TweenTo(_block, _bottom, 1f, () => _loader.OnSceneLoad());
    }
}
