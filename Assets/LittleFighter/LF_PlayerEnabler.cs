using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LF_PlayerEnabler : MonoBehaviour
{
    [SerializeField] private GameObject _pressToJoin;
    [SerializeField] private GameObject _player2Score;
    [SerializeField] private GameObject _player2HpBar;
    [SerializeField] private LF_Player _player2;
    
    private bool _isPlayerActive = false;


    private void ActivatePlayer()
    {
        _isPlayerActive = true;

        _player2Score.SetActive(true);
        _pressToJoin.SetActive(false);
        _player2HpBar.SetActive(true);


        _player2.gameObject.SetActive(true);
        _player2.IsAwake = true;
        _player2.transform.position = PlayerList<LF_Player>.Get(PlayerIndex.Player1).transform.position;
        
        CameraFollow.Instance.SetNewFollowable( new Transform[] {
            PlayerList<LF_Player>.Get(PlayerIndex.Player1).transform,
            _player2.transform
        });
    }

    void Update(){
        if(!_isPlayerActive && InputHandler.GetKey(InputKey.Action_Any_Player2)){
            ActivatePlayer();
        }
    }
}
