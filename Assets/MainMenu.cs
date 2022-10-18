using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [SerializeField] Button _play;

    void Start()
    {
        _play.Select();


        AudioSystem.Instance.PlayMusic("BG", 0);
        TimersManager.Instance.FireAfter(1, () => {
            AudioSystem.Instance.PlayMusic("BG", 1);
        });

    }
}
