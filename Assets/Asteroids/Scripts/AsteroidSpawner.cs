using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] Transform[] _positions;
    [SerializeField] GameObject[]  _prefab;
    [SerializeField] GameObject _player;

    public static int AsteroidCount = 0;
    public static AsteroidSpawner Spawner;
    int generation = 0;
    int needToSpawn = 0;

    float timer = 1.5f;

    ValidSizes _types = new ValidSizes{
        Sizes={
            Asteroid.EAsteroidSize.EAS_HUGE,
            Asteroid.EAsteroidSize.EAS_NORMAL,
            Asteroid.EAsteroidSize.EAS_SMALL,
        }
    };
    private void OnEnable() {
        AsteroidCount = 0;
        PointsCounter.Reset();
    }

    private void Awake() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.ResizeAsteroids);
        Spawner = this;
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){
        if(enabled == false) return;
        if(gameplayEvent.type == GameplayEventType.ResizeAsteroids){
            _types = (ValidSizes)gameplayEvent.parameter;
        }
    }


    private void Update() {
        Debug.Log(AsteroidCount + " "  + needToSpawn);
        if(AsteroidCount + needToSpawn <= 2) {
            needToSpawn = 5 + Random.Range(0, 4); 
            generation += 1;
        }
        timer -= Time.deltaTime;
        if(timer < 0 && needToSpawn > 0) SpawnAsteroid();
    }

    private void SpawnAsteroid(){
        timer = 1.5f;

        List<int> indexes = new List<int>();
        for(int i = 0; i < _positions.Length; i++) indexes.Add(i);
        ShuffleList(indexes);

        if(_types.Sizes.Count == 0) return;
        for(int i = 0; i < indexes.Count; i++) {
            if( Vector3.Distance(_positions[indexes[i]].transform.position, _player.transform.position) < 2.0f) continue;

            Asteroid.EAsteroidSize type = _types.Sizes[ UnityEngine.Random.Range(0, _types.Sizes.Count)];
            Spawn(_positions[indexes[i]].transform.position, 
                    type, 
                    new Vector3(
                        Random.Range(-1.0f, 1.0f), 
                        Random.Range(-1.0f, 1.0f), 
                        0).normalized,
                    generation);
            
            needToSpawn -= 1;
            AsteroidCount += 1;
            return;
        }
    }

    public void Spawn(Vector3 pos, Asteroid.EAsteroidSize size, Vector3 direction, int generation){
            Asteroid asteroid = Instantiate(
                _prefab[(int)size], 
                pos, 
                Quaternion.identity).GetComponent<Asteroid>();

            
            asteroid.Setup(direction, size, generation);
            (asteroid.transform as RectTransform).SetParent(transform.parent);
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
