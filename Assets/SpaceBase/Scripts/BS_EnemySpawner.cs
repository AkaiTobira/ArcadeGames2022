using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] _enemyList;
    [SerializeField] Transform _enemiesParent;
    [SerializeField] Transform _player;
    GameObject spawnedEnemy = null;

    private bool _canSpawn = false;
    private float _spawnDelay = 0;

    public void Spawn(){
        _canSpawn = true;
    }

    private void Update() {

        

        if(Guard.IsValid(spawnedEnemy)){
            _canSpawn = false;

            Vector2 playerPostion = spawnedEnemy.transform.position;
            Vector2 enemyPosition = _player.position;

            if((playerPostion - enemyPosition).magnitude > 30){
                Destroy(spawnedEnemy);
                _canSpawn = true;
            }
            return;
        }
        _spawnDelay -= Time.deltaTime;
        if(_canSpawn){
        //    _spawnDelay = 25.0f;

            int spawnID = Random.Range(0, _enemyList.Length);
            spawnedEnemy = Instantiate(
                _enemyList[spawnID],
                transform.position,
                Quaternion.identity,
                _enemiesParent);
            _canSpawn = false;
        }
    }
}
