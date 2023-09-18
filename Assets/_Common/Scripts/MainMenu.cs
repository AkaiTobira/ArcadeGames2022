using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [SerializeField] Button _play;
    [SerializeField] string _backgroundMusicName;
    [SerializeField] GameType _type;

    void Start()
    {
        _play.Select();
        
        CButton_GameDependent.SetActiveGameType(_type);
        
        AudioSystem.Instance.PlayMusic(_backgroundMusicName, 0);
        TimersManager.Instance.FireAfter(1, () => {
            AudioSystem.Instance.PlayMusic(_backgroundMusicName, 1);
        });

        continueButtonPressed = false;
    }


    bool continueButtonPressed = false;

    void Update(){
        if(Input.GetKey(KeyCode.C) && !continueButtonPressed){ _play.onClick.Invoke(); continueButtonPressed = true; }
    }
}
