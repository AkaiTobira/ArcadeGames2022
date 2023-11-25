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

    public class DD_RockHandler : ESM.SMC_1D<RockStates>, IDealDamage
    {
        [SerializeField] Transform rayStart;
        [SerializeField] LayerMask _layer;
        [SerializeField] float _rayLenght;
        [SerializeField] BoxCollider2D _collider;



        protected override void Awake() {
            base.Awake();
            ForceState(RockStates.Idle, true);
        }

        const float IDLE_DELAY = 0.5f;
        const float MOVE_DELAY = 1.25f;

        float idleDelay = IDLE_DELAY;
        float moveDelay = MOVE_DELAY;

        public void Setup(){
            ForceState(RockStates.Idle, true);
        }

        public int GetDamage(){ 
//            Debug.Log("Rock -> Damage Delivered");
            return 1; }
            
        protected override void OnStateEnter(RockStates enteredState){
            switch(enteredState){
                case RockStates.Idle:
                    idleDelay = IDLE_DELAY;
                    _collider.enabled = true;
                break;
                case RockStates.StartMoving:
                    moveDelay = MOVE_DELAY;
                    AudioSystem.PlaySample("DigDug_Rock", 2);
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

                if(hit.Length >= 2 && hit[0].collider.name == "UpBrick (1)" && 
                    (hit[1].collider.name == "BrickInternal (1)" || 
                     hit[1].collider.name == "Rock"
                    )) return false;
                return true;
        }

    }

}

