using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEnemySpawnerManager : CMonoBehaviour
{
    [SerializeField] BEnemySpawner[] _spawners;

    protected override void Awake() {
        base.Awake();

        CallAfterFixedUpdate(() => {
            SpawnEnemies();
        });
    }

    private void ShuffleSpawners(){
        for(int i = 0; i < _spawners.Length; i++) {
            int a = Random.Range(0, _spawners.Length);
            int b = Random.Range(0, _spawners.Length);
            BEnemySpawner temp = _spawners[a];
            _spawners[a] = _spawners[b];
            _spawners[b] = temp;
        }
    }


    public void SpawnEnemies(){
        int enemyAmount = 
            8             
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)            
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2)
            + Random.Range(0,2);
        
        ShuffleSpawners();

        for(int i = 0; i < _spawners.Length; i++) {
            _spawners[i].gameObject.SetActive(false);
        }

        int color = Random.Range(0,3);

        while(enemyAmount > 0){
            for(int i = 0; i < _spawners.Length; i++) {
                BEnemySpawner spawner = _spawners[i];

                if(spawner.gameObject.activeSelf) continue;
                float distance = (Berzerk.Instance.transform.position - _spawners[i].transform.position).magnitude;
                if(Random.Range( 0, distance/20.0f) < 0.1f) continue;
                spawner.gameObject.SetActive(true);
                spawner.Spawn(color);
                
                if(enemyAmount-- < 0) return;
            }
        }
    }
}
