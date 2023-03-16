using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LF_EnemySpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> Spawners;
    [SerializeField] GameObject[] EnemiesPrefabs;
    [SerializeField] int[] _probabilities;
    [SerializeField] float _increaseDelay = 5f;

    public static int Counter = 0;

    private float _increaseDelayTimer;
    private int _maxSpawnedCounter = 3;
    private int _spawnedEnemyCounter = 0 ;

    public static int EnemyLevel = 0;

    private void Awake() {
        PointsCounter.Score = 0;
        HighScoreRanking.LoadRanking(GameType.LittleFighter);
        _increaseDelayTimer = _increaseDelay;
    }

    private void Ranomize(){
        for(int i = 0; i < Spawners.Count; i++) {
            int a = Random.Range(0, Spawners.Count);
            int b = Random.Range(0, Spawners.Count);

            GameObject temp = Spawners[a];
            Spawners[a] = Spawners[b];
            Spawners[b] = temp;
        }
    }


    private void Update() {
        _increaseDelayTimer -= Time.deltaTime;
        if(_increaseDelayTimer < 0){
            _increaseDelayTimer = _increaseDelay;
            _increaseDelay *= 1.2f;
            _maxSpawnedCounter = Mathf.Min(_maxSpawnedCounter+1, Spawners.Count);
        }

        if(Counter < _maxSpawnedCounter){
            Ranomize();
            GameObject go = Instantiate(
                EnemiesPrefabs[GetEnemyIndex()], 
                Spawners[0].transform.position, 
                Quaternion.identity, 
                transform); 
            go.name += Counter;
            _spawnedEnemyCounter++;
        }

        EnemyLevel = _spawnedEnemyCounter / 10;
    }

    private int GetEnemyIndex(){

        int sum = 0;
        for(int i = 0; i < _probabilities.Length; i++) sum += _probabilities[i];

        int selected = Random.Range(0, sum);
        sum = 0;
        for(int i = 0; i < _probabilities.Length; i++){
            sum += _probabilities[i];
            if(selected <= sum) return i;
        }

        return 0;
    }
}
