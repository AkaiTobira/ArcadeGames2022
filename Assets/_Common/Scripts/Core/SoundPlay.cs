using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlay : MonoBehaviour
{

    [SerializeField] bool _playSoundOnStart;
    [SerializeField] bool _playMusicOnStart;
    [SerializeField] string _soundName;
    [SerializeField] string _musicName;

    [SerializeField] float _volume = 1.0f;


    private void Start() {
        if(_playMusicOnStart) PlayMusic();
        if(_playSoundOnStart) PlaySound();
    }

    public void PlaySound(){
        AudioSystem.Instance.PlayEffect(_soundName, _volume);
    }

    public void PlayMusic(){
        AudioSystem.Instance.PlayMusic(_musicName, _volume);
    }

}
