using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LF_EnemySpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> Spawners;
    [SerializeField] GameObject[] EnemiesPrefabs;
    [SerializeField] int[] _probabilities;


    public static int Counter = 0;

    private void Awake() {
        PointsCounter.Score = 0;
        HighScoreRanking.LoadRanking(GameType.LittleFighter);
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
        
        if(Counter > 0) return;

        Ranomize();
        int number = Random.Range(2, Spawners.Count);
        for(int i = 0; i < number; i++) {
            Instantiate(EnemiesPrefabs[GetEnemyIndex()], Spawners[i].transform.position, Quaternion.identity, transform); 
        }
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
