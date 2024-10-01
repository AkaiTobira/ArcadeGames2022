using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_Angel : MonoBehaviour
{
    [SerializeField] Transform _angelTarget;
    [SerializeField] Transform _bozon;
    [SerializeField] Transform _bozonTarget;

    private void OnEnable() {
        TweenManager.Instance.TweenTo(transform, _angelTarget, 1f);
        TweenManager.Instance.TweenTo(_bozon, _bozonTarget, 2f);
    }
}
