using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ConflictSide{
    Player,
    AI,
}

public class LF_ColliderSide : MonoBehaviour
{
    public ConflictSide _sides;
    public bool _dealDamage = false;
    [SerializeField] MonoBehaviour _Parent;

    private void OnTriggerEnter2D(Collider2D other) {
        LF_ColliderSide side = other.GetComponent<LF_ColliderSide>();
        if(Guard.IsValid(side)){
            if(side._sides == _sides) return;
            if(_dealDamage && !side._dealDamage){
                ITakeDamage takeDamage = side._Parent.GetComponent<ITakeDamage>();
                IDealDamage dealDamage = _Parent.GetComponent<IDealDamage>();
            
                if(takeDamage != null && dealDamage != null){
                    takeDamage.TakeDamage(dealDamage.GetDamage());
                }
            }
        }
    }
}
