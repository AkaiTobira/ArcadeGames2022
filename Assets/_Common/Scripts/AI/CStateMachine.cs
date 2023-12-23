using System.Collections.Generic;
using AI;
using UnityEngine;

namespace AI.FSM
{
    public abstract class IState<StateType>
    where StateType : System.Enum{
        public virtual void OnEnter(){}
        public virtual void OnExit(){}
        public abstract void OnUpdate();
        public abstract StateType Transite(ref bool IsChanged);
    }

    public class FSM<StateType> : IFSM
    where StateType : System.Enum
    {
        StateType _activeStateEnum;
        protected IState<StateType> _activeState;
        Dictionary<StateType, IState<StateType>> _states;
        private static Dictionary<int, StateType> IntToStateType;
    
        public FSM(Dictionary<StateType, IState<StateType>> states, StateType startingState){
            _states = states;
            _activeStateEnum = startingState;
            _activeState = states[startingState];

            if(IntToStateType == null) CUtils.GetIntToEnumValues<StateType>(ref IntToStateType);
        }

        public void ForceState(int stateIndex){
            StateType stateEnum = IntToStateType[stateIndex];
            if(_states.TryGetValue(stateEnum, out IState<StateType> val)){
                _activeState.OnExit();

                _activeState     = val;
                _activeStateEnum = stateEnum;

                _activeState.OnEnter();
            }
        }

    //    public IState<StateType> GetActiveState(){ return _activeState; }

        public StateType GetActiveState() { return _activeStateEnum; }

        public void Update(){
            _activeState.OnUpdate();

            bool isChanged = false;
            StateType newState = _activeState.Transite(ref isChanged);
            if(isChanged){
                _activeState.OnExit();

                Debug.Log("Transition from " + _activeStateEnum + " to " + newState);

                _activeState     = _states[newState];
                _activeStateEnum = newState;

                _activeState.OnEnter();
            }
        }
    }
}


