using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlay : MonoBehaviour
{

    [SerializeField] bool _playSoundOnStart;
    [SerializeField] bool _playMusicOnStart;
    [SerializeField] bool _playSoundOnEnable;
    [SerializeField] bool _playMusicOnEnable;
    
    [SerializeField] string _soundName;
    [SerializeField] string _musicName;

    [SerializeField] float _volume = 1.0f;


    private void Start() {
        if(_playMusicOnStart) PlayMusic();
        if(_playSoundOnStart) PlaySound();
    }

    private void OnEnable(){
        if(_playMusicOnEnable) PlayMusic();
        if(_playSoundOnEnable) PlaySound();
    }

    public void PlaySound(){
        AudioSystem.PlaySample(_soundName, _volume);
        //AudioSystem.Instance.PlayEffect(_soundName, _volume);
    }

    public void PlayMusic(){
        AudioSystem.PlayBackground(_musicName, _volume);
        //AudioSystem.Instance.PlayMusic(_musicName, _volume);
    }
}
