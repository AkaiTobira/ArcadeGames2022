using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
   [SerializeField] GameObject _activated;
   [SerializeField] GameObject _activated2;

    public bool IsPlayerOneActive() { return _activated.activeSelf; }
    public bool IsPlayerTwoActive() { return _activated2.activeSelf; }
    
    public bool OnReach(PlayerIndex playerIndex){
        if(playerIndex == PlayerIndex.Player1 && !_activated2.activeSelf){
            if(_activated != null){
                bool wasActive = _activated.activeSelf;
                _activated.SetActive(true);
                return wasActive != _activated.activeSelf;
            }
        }
        else if(playerIndex == PlayerIndex.Player2 && !_activated.activeSelf){
            if(_activated2 != null){
                bool wasActive = _activated2.activeSelf;
                _activated2.SetActive(true);
                return wasActive != _activated2.activeSelf;
            }
        }      
        
        return false;
    }
}
