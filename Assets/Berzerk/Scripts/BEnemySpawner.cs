using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEnemySpawner : MonoBehaviour
{
    [SerializeField] BEnemy[] _enemyPrefab;

    BEnemy[] _spawnedEnemy;

    private void Awake() {
        _spawnedEnemy = new BEnemy[_enemyPrefab.Length];
    }


    public void Spawn(int colorIndex){
        Deactivate();

        if(_spawnedEnemy[colorIndex]== null) 
            _spawnedEnemy[colorIndex] = Instantiate(_enemyPrefab[colorIndex], transform.position, Quaternion.identity, transform);

        _spawnedEnemy[colorIndex].transform.position = transform.position;
        _spawnedEnemy[colorIndex].Initialize();
    }

    private void Deactivate(){
        for(int i = 0; i < _spawnedEnemy.Length; i++) {
            if(Guard.IsValid(_spawnedEnemy[i])) _spawnedEnemy[i].gameObject.SetActive(false);
        }
    }

}
