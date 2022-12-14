using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] Transform[] _positions;
    [SerializeField] GameObject _asteroidPrefab;
    [SerializeField] GameObject _player;

    public static int AsteroidCount = 0;
    int generation = 0;

    ValidSizes _types = new ValidSizes{
        Sizes={
            Asteroid.EAsteroidSize.EAS_HUGE,
            Asteroid.EAsteroidSize.EAS_NORMAL,
            Asteroid.EAsteroidSize.EAS_SMALL,
        }
    };

    private void OnEnable() {
        AsteroidCount = 0;
        PointsCounter.Score = 0;
    }


    private void Start() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.ResizeAsteroids);
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){

        if(enabled == false) return;

        if(gameplayEvent.type == GameplayEventType.ResizeAsteroids){

            _types = (ValidSizes)gameplayEvent.parameter;
        }
    }


    private void Update() {
        if(AsteroidCount <= 2) SpawnAsteroids();
    }

    private void SpawnAsteroids(){


        List<int> indexes = new List<int>();
        for(int i = 0; i < _positions.Length; i++) indexes.Add(i);
        ShuffleList(indexes);

        int numberOfAsteroids = 5 + Random.Range(0, 4);

        if(_types.Sizes.Count == 0) return;
        for(int i = 0; i < indexes.Count; i++) {
            if( Vector3.Distance(_positions[indexes[i]].transform.position, _player.transform.position) < 2.0f) continue;

            Asteroid asteroid = Instantiate(
                _asteroidPrefab, 
                _positions[indexes[i]].transform.position, 
                Quaternion.identity).GetComponent<Asteroid>();
            

            Asteroid.EAsteroidSize type = _types.Sizes[ UnityEngine.Random.Range(0, _types.Sizes.Count)];

            asteroid.Setup( new Vector3( Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0).normalized, type, generation);
            (asteroid.transform as RectTransform).SetParent(transform);

            numberOfAsteroids -= 1;
            if(numberOfAsteroids < 0) break;
        }

        generation += 1;
    }

    private void ShuffleList(List<int> list){
        for(int i = 0; i< list.Count; i++){
            if( Random.Range(0.0f,1.0f) > 0.5f ){
                int point = Random.Range(0, list.Count);
                int temp = list[i];
                list[i] = list[point];
                list[point] = temp;
            }
        }
    }


}
