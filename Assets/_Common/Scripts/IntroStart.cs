using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroStart : MonoBehaviour
{
    [SerializeField] GameType gameType = GameType.NotLoaded;

    void Start()
    {
        AudioSystem.Instance.PlayMusic("Intro_BG", 0);
        CButton_GameDependent.SetActiveGameType(gameType);
    }
}
