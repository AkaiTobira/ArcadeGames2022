using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationAsteroidsIntro : MonoBehaviour
{

    [SerializeField] Transform _center;
    [SerializeField] Transform _bottom;
    [SerializeField] Transform[] _blocks;
    [SerializeField] SceneLoader _loader;

    [SerializeField] Button _endButton;

    void Start()
    {
        BringMiddle(0);
    }

    void BringMiddle(int index){
        if(index >= _blocks.Length) return;

        TweenManager.Instance.TweenTo(_blocks[index], _center, 1f);

        if(index >= _blocks.Length - 1){
            _endButton.Select();
            return;
        }


        TimersManager.Instance.FireAfter(5f, () => {
            BringBottom(index);
            BringMiddle(index + 1);
        });
    }

    void BringBottom(int index){
        if(index >= _blocks.Length - 1){
            _endButton.Select();
            return;
        }

        TweenManager.Instance.TweenTo(_blocks[index], _bottom, 1f);
    }
}
