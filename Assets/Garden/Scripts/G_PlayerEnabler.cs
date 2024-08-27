using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class G_PlayerEnabler : MonoBehaviour
{
    [SerializeField] private GameObject _pressToJoin;
    [SerializeField] private G_Player _player2;
    [SerializeField] private PointsCounter _player2Counter;
    
    private bool _isPlayerActive = false;


    private void ActivatePlayer()
    {
        _isPlayerActive = true;
        _pressToJoin.SetActive(false);
        _player2.gameObject.SetActive(true);
        _player2Counter.gameObject.SetActive(true);
        G_Player._takenPosition[(int)PlayerIndex.Player2] = 0;
    }

    void Update(){
        if(!_isPlayerActive && InputHandler.GetKey(InputKey.Action_Any_Player2)){
            ActivatePlayer();
        }
    }
}
