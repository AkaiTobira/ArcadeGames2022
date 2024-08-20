using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G_DurationBar : CUpdateMonoBehaviour
{
    [SerializeField] Image _bar;

    private bool _backwards;
    private float _current;
    private float _duration;


    public void FillIn(float durtiation, bool backwards = false){
        gameObject.SetActive(true);
        _duration = durtiation;
        _backwards = backwards;

        if(backwards){ _current = durtiation; } else { _current = 0f; }
        TimersManager.Instance.FireAfter(durtiation, () => gameObject.SetActive(false));
    }

    public override void CUpdate()
    {
        base.CUpdate();

        _current += _backwards ? - Time.deltaTime : Time.deltaTime;
        float percent = _current / _duration;
        _bar.fillAmount = Mathf.Min(1, Mathf.Max(percent, 0));
    }
}
