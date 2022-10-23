using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




public class LevelManager : MonoBehaviour
{
    [Serializable]
    private class CameraSetup{
        [SerializeField] public float Left;
        [SerializeField] public float Right;
        [SerializeField] public float Top;
        [SerializeField] public float Bottom;
    }


    [SerializeField] LevelController[] _levels;
    [SerializeField] CameraSetup[] _cameraSetups;
    [SerializeField] int[] _timerMax;
    [SerializeField] TimerCount _timer;
    [SerializeField] string[] _audioClipsNames;

    private void Start() {
        
        int selectedLevel = UnityEngine.Random.Range(0, _levels.Length);
        CameraFollow.Instance.SetValues( 
            new CameraFollow.KeyValuePairs(true, _cameraSetups[selectedLevel].Left),
            new CameraFollow.KeyValuePairs(true, _cameraSetups[selectedLevel].Right),
            new CameraFollow.KeyValuePairs(true, _cameraSetups[selectedLevel].Top),
            new CameraFollow.KeyValuePairs(true, _cameraSetups[selectedLevel].Bottom)
            );
        _levels[selectedLevel].gameObject.SetActive(true);
        _timer.time = 0;//_timerMax[selectedLevel];
        AudioSystem.Instance.PlayMusic(_audioClipsNames[selectedLevel],1);
    }
}
