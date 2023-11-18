using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public  class DD_NavPoint : CMonoBehaviour{
    [SerializeField] float passWeight = 2f;
    public virtual float GetWalkWeight(){ return passWeight;}
}

