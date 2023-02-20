using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FollowerState{
    Move,
    Dead,
}

public class BFollower : ESM.SMC_4D<FollowerState>
{

    private bool _isDead = false;
    private Vector2 _inputs = new Vector2();

    private void OnEnable() {
        _isDead = false;
        ForceState(FollowerState.Move, true);
    }

    protected override void OnStateEnter(FollowerState enteredState)
    {
        switch (enteredState) {
            case FollowerState.Move:
                GetComponent<BoxCollider2D>().enabled = true;
                break;
            case FollowerState.Dead:
                GetComponent<BoxCollider2D>().enabled = false;
                break;
        }
    }

    protected override void OnStateExit(FollowerState exitedState)
    {
        
    }

    protected override FollowerState CheckStateTransitions()
    {
        switch (ActiveState) {
            case FollowerState.Move:
                if(_isDead) return FollowerState.Dead;
                break;
        }

        return ActiveState;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<Berzerk>().Kill();
            _isDead = true;
        }
    }


    protected override void UpdateState()
    {
        switch(ActiveState){
            case FollowerState.Move:
                
                _inputs = (Berzerk.Instance.transform.position - transform.position).normalized;

                float speedMultiplier = Mathf.Min(BLevelsManager.Timer/100f + BLevelsManager.CurrentLevel/20.0f, 1f);

                ProcessMove(_inputs *  speedMultiplier);
                
            break;
        }
    }


}
