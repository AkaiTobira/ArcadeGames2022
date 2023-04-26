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
    public bool _isDetector = false;
    public bool _destroySelfAtCollision = false;
    [SerializeField] MonoBehaviour _Parent;

    private void OnTriggerEnter2D(Collider2D other) {

        if(!Guard.IsValid(other) || !Guard.IsValid(_Parent)) return;

        LF_ColliderSide side = other.GetComponent<LF_ColliderSide>();
        if(Guard.IsValid(side)){
            if(!Guard.IsValid(side._Parent)) return;
            if(side._sides == _sides) return;
            if(tag == "Sight"){
                IUseDetector useDetector = _Parent.GetComponent<IUseDetector>();
                useDetector.Detected(side._Parent);
                return;
            }else if(tag == "AttackBox"){
                
                if(side.tag == "HitBox"){

                    if(_dealDamage && !side._dealDamage){
                        ITakeDamage takeDamage = side._Parent.GetComponent<ITakeDamage>();
                        IDealDamage dealDamage = _Parent.GetComponent<IDealDamage>();
//                                            Debug.Log((takeDamage == null) + " " + (dealDamage == null));
                        if(takeDamage != null && dealDamage != null){
                            takeDamage.TakeDamage(dealDamage.GetDamage(), _Parent);
                        }
                    }
                    if(_destroySelfAtCollision) Destroy(gameObject);
                }
            }else if(tag == "PickupBox"){
                if(side._sides == ConflictSide.Player){
                    ITakeDamage takeDamage = side._Parent.GetComponent<ITakeDamage>();
                    IPickedUp pickedUp = _Parent.GetComponent<IPickedUp>();
                    if(takeDamage != null && pickedUp != null){
                        pickedUp.PickedUp();
                    }
                }
            }
        }
    }

    public void SetParent(MonoBehaviour mb){
        _Parent = mb;
    }

    private void OnTriggerExit2D(Collider2D other) {
        LF_ColliderSide side = other.GetComponent<LF_ColliderSide>();
        if(Guard.IsValid(side)){
            if(side._sides == _sides) return;
            if(_isDetector){
                IUseDetector useDetector = _Parent.GetComponent<IUseDetector>();
                useDetector.SignalLost(side._Parent);
            }
        }
    }
}
