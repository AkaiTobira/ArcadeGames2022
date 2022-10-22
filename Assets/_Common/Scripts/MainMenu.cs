using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [SerializeField] Button _play;
    [SerializeField] string _backgroundMusicName;  

    void Start()
    {
        _play.Select();


        AudioSystem.Instance.PlayMusic(_backgroundMusicName, 0);
        TimersManager.Instance.FireAfter(1, () => {
            AudioSystem.Instance.PlayMusic(_backgroundMusicName, 1);
        });

    }
}
