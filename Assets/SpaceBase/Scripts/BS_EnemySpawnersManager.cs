using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_EnemySpawnersManager : MonoBehaviour
{
    [SerializeField] List<BS_EnemySpawner> Spawners;

    int numberOfActiveEnemies = 4;
    float _timer = 30f;

    private void Update() {

        
        _timer -= Time.deltaTime;
        if(_timer < 0){
            numberOfActiveEnemies++;
            CUtils.Shuffle<BS_EnemySpawner>(Spawners);
            for(int i = 0; i< Mathf.Min(numberOfActiveEnemies, Spawners.Count); i++){
                Spawners[i].Spawn();
            }

            _timer = numberOfActiveEnemies * 7f;
        }
    }
}
