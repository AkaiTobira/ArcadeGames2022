using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlay : MonoBehaviour
{

    [SerializeField] string _soundName;

    public void PlaySound(){
        AudioSystem.Instance.PlayEffect(_soundName, 1);
    }

}
