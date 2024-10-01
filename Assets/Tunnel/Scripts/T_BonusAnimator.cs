using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum T_BonusType{
    Slower,
    Faster,
    Protect,
}

public class T_BonusAnimator : T_EnemyAnimator
{
    [SerializeField] T_BonusType _type;
    [SerializeField] AutoTranslatorUnit _translatorUnit;
    
    protected override void Awake(){
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _translatorUnit.SetTag("T" + _type); 
    }

    public T_BonusType GetBonusType() { return _type; }
}
