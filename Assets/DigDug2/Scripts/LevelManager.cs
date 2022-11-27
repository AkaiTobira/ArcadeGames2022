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

    public static int SelectedLevel = -1;
    public static int NUMBER_OF_LEVELS = 3; 

    private static List<int> indexes = new List<int>();

    private void Start() {
        NUMBER_OF_LEVELS = _levels.Length;

        for(int i = 0; i < NUMBER_OF_LEVELS; i++) indexes.Add(i);
        for(int i = 0; i < NUMBER_OF_LEVELS; i++) {
            int  firstIndex = UnityEngine.Random.Range(0, _levels.Length);
            int secondIndex = UnityEngine.Random.Range(0, _levels.Length);

            int temp = indexes[firstIndex];
            indexes[firstIndex]  = indexes[secondIndex];
            indexes[secondIndex] = temp;
        }

        for(int i = 0; i< NUMBER_OF_LEVELS; i++){
            SelectedLevel = i;
            if(!DigDugPlayedMaps.IsLocked(SelectedLevel)) break;
        }

        CameraFollow.Instance.SetValues( 
            new CameraFollow.KeyValuePairs(true, _cameraSetups[SelectedLevel].Left),
            new CameraFollow.KeyValuePairs(true, _cameraSetups[SelectedLevel].Right),
            new CameraFollow.KeyValuePairs(true, _cameraSetups[SelectedLevel].Top),
            new CameraFollow.KeyValuePairs(true, _cameraSetups[SelectedLevel].Bottom)
            );

        _levels[SelectedLevel].gameObject.SetActive(true);
        _timer.time = 0;//_timerMax[selectedLevel];
        AudioSystem.Instance.PlayMusic(_audioClipsNames[SelectedLevel],1);
    }
}
