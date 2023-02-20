using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroStart : MonoBehaviour
{
    void Start()
    {
        AudioSystem.Instance.PlayMusic("Intro_BG", 0);
    }
}
