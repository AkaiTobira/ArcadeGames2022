using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class G_KeyLocker : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Sprite[] _codes;
    [SerializeField] Image[] _frames;
    [SerializeField] GameObject _keyBrokeing;

    private List<int> keys = new List<int>();

    private int _currentToReadIndex = 0;

    public void Awake(){
        EnableTextes();
    }
    public void ReadInput(PlayerIndex index)
    {
        int readed = -1;
        switch(index){
            case PlayerIndex.Player1: 
                if(InputHandler.GetKey(InputKey.Action_1_Player1)) readed = 0;
                if(InputHandler.GetKey(InputKey.Action_2_Player1)) readed = 1;
                if(InputHandler.GetKey(InputKey.Action_3_Player1)) readed = 2;
                if(InputHandler.GetKey(InputKey.Action_4_Player2)) readed = 3;
                if(InputHandler.GetKey(InputKey.Action_5_Player2)) readed = 4;
                if(InputHandler.GetKey(InputKey.Action_6_Player2)) readed = 5;
            break;
            case PlayerIndex.Player2:
                if(InputHandler.GetKey(InputKey.Action_1_Player2)) readed = 0;
                if(InputHandler.GetKey(InputKey.Action_2_Player2)) readed = 1;
                if(InputHandler.GetKey(InputKey.Action_3_Player2)) readed = 2;
                if(InputHandler.GetKey(InputKey.Action_4_Player1)) readed = 3;
                if(InputHandler.GetKey(InputKey.Action_5_Player1)) readed = 4;
                if(InputHandler.GetKey(InputKey.Action_6_Player1)) readed = 5;
            break;
        }

        if(readed != -1){
            if(keys[_currentToReadIndex] == readed) {
                _currentToReadIndex++;
                AudioSystem.PlaySample("Garden_CodeGood");
            }
            else {
                _currentToReadIndex = 0;
                AudioSystem.PlaySample("Garden_CodeWrong");
            }

            SetupReaded();
            EnableTextes();
        }
    }

    private void EnableTextes()
    {
        _keyBrokeing.SetActive(IsLocked());
        _text.text = AutoTranslator.Translate("GPress");
    }

    private void SetupReaded()
    {
        for(int i = 0; i < keys.Count; i++){
            _frames[i].color = _currentToReadIndex <= i ? Color.white : Color.gray;
            _frames[i].sprite = _codes[keys[i]];
        }
    }

    public void Hide(){
        keys.Clear();
        _currentToReadIndex = 0;
        EnableTextes();
    }

    public bool IsLocked()
    {
        return _currentToReadIndex != keys.Count;
    }

    public void LockPlayer()
    {
        keys.Clear();
        _currentToReadIndex = 0;
        for(int i =0; i < 3; i ++){
            keys.Add(CUtils.Rand(6));
        }

        SetupReaded();
        EnableTextes();
    }
}
