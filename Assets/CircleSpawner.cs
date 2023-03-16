using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    [SerializeField] T_Segment _prefab;
    [SerializeField] float _spawnBreak = 4f;

    List<T_Segment> _spawned = new List<T_Segment>();

    List<Vector3> _positions = new List<Vector3>();


    float timer = 0;

    Vector3 _tripBegin;

    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0){
            timer += _spawnBreak;

            T_Segment segment = GetNextSegment();
            _spawned.Add(segment);

            if(_positions.Count == 0){
                GeneratePositions();
            }

            segment.Setup(_positions[0]);
            _positions.RemoveAt(0);

            SortCanvas();
        }
    }

    void GeneratePositions(){
        float randomValue = Random.Range(0.0f, 1.0f);
        if(randomValue < 0.7f){
            _positions.Add(new Vector3());
        }else{
            randomValue = Random.Range(0,2);
            if(randomValue == 0){
                randomValue = Random.Range(0,2) == 0 ? -2.5f : 2.5f;

                _positions.Add(new Vector3(randomValue * 0.25f, 0));
                _positions.Add(new Vector3(2.0f * randomValue * 0.25f, 0));
                _positions.Add(new Vector3(3.0f * randomValue * 0.25f, 0));
                _positions.Add(new Vector3(randomValue, 0));
                _positions.Add(new Vector3(3.0f * randomValue * 0.25f, 0));
                _positions.Add(new Vector3(2.0f * randomValue * 0.25f, 0));
                _positions.Add(new Vector3(randomValue * 0.25f, 0));
                _positions.Add(new Vector3(0, 0));


            }else{
                randomValue = Random.Range(0,2) == 0 ? -2.5f : 2.5f;

                _positions.Add(new Vector3(0, randomValue * 0.25f, 0));
                _positions.Add(new Vector3(0, 2.0f * randomValue * 0.25f, 0));
                _positions.Add(new Vector3(0, 3.0f * randomValue * 0.25f, 0));
                _positions.Add(new Vector3(0, randomValue, 0));
                _positions.Add(new Vector3(0, 3.0f * randomValue * 0.25f, 0));
                _positions.Add(new Vector3(0, 2.0f * randomValue * 0.25f, 0));
                _positions.Add(new Vector3(0, randomValue * 0.25f, 0));
                _positions.Add(new Vector3(0, 0, 0));
            }
        }
    }


    void SortCanvas(){
        for(int i = 0; i < _spawned.Count; i++) {
            _spawned[i].GetComponent<Canvas>().sortingOrder = i;
        }
    }

    T_Segment GetNextSegment(){
        for(int i = 0; i < _spawned.Count; i++) {
            T_Segment segment = _spawned[i];
            if(!segment.enabled){
                _spawned.RemoveAt(i);
                return segment;
            }
        }

        T_Segment segment1 = Instantiate(_prefab, new Vector3(), Quaternion.identity, transform);
        segment1.GetComponent<Canvas>().sortingOrder = _spawned.Count;
        return segment1;
    }

}
