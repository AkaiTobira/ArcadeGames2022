using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigDug{
    public enum RockStates{
        Idle,
        StartMoving,
        Moving,
        Break,
    }

    public class RockHandler : ESM.SMC_1D<RockStates>, IDealDamage
    {
        [SerializeField] Transform rayStart;
        [SerializeField] LayerMask _layer;
        [SerializeField] float _rayLenght;


        private void Awake() {
            ForceState(RockStates.Idle, true);
        }

        float idleDelay = 0.5f;
        float moveDelay = 2f;
        float destroyDelay = 0.5f;

        public int GetDamage(){ 
            Debug.Log("Rock -> Damage Delivered");
            return 1; }
        protected override void OnStateEnter(RockStates enteredState){
            switch(enteredState){
                case RockStates.Idle:
                    idleDelay = 0.5f;
                break;
                case RockStates.StartMoving:
                    moveDelay = 2.0f;
                break;
                case RockStates.Moving:
                    transform.parent.GetComponent<DD_BrickController>().DisableCenter();
                    GetComponent<Rigidbody2D>().simulated = true;
                break;
                case RockStates.Break:
                    RequestDisable(0.5f);
                break;
            }
        }

        protected override void OnStateExit(RockStates exitedState){}
        bool _canStartMoving = false;
        protected override void UpdateState()
        {
            switch(ActiveState){
                case RockStates.Idle:
                    idleDelay -= Time.deltaTime;
                break;
                case RockStates.StartMoving:
                    moveDelay -= Time.deltaTime;
                break;
                case RockStates.Moving:
                    ProcessMove(Vector2.down);
                break;
            }
        }

        protected override RockStates CheckStateTransitions()
        {
            switch(ActiveState){
                case RockStates.Idle: 
                    if(CanStartMoving() && idleDelay <= 0) return RockStates.StartMoving;
                    break;
                case RockStates.StartMoving:
                    if(moveDelay < 0) return RockStates.Moving;
                    break;
                case RockStates.Moving:
                    if(!CanStartMoving()) return RockStates.Break;
                    break;
                case RockStates.Break:
                    break;
            }

            return ActiveState;
        }

        private bool CanStartMoving(){
                RaycastHit2D[] hit = Physics2D.RaycastAll(rayStart.position, Vector2.down, _rayLenght, _layer);
                Debug.DrawLine(rayStart.position, rayStart.position + Vector3.down * _rayLenght, Color.magenta);

    //            string debug = hit.Length.ToString();
    //            for(int i = 0; i < hit.Length; i++) {
    //                debug += hit[i].collider.name + " : ";
    //            }
    //            Debug.Log(debug);

                if(hit.Length >= 2 && hit[0].collider.name == "UpBrick (1)" && hit[1].collider.name == "BrickInternal (1)") return false;
                return true;
        }

    }

}

