using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    public interface IFSM{
        void Update();
        void ForceState(int stateIndex);
    }
}
