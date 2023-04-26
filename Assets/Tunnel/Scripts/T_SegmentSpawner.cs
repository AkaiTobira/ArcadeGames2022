using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_SegmentSpawner : MonoBehaviour
{
    [SerializeField] T_Segment _prefab;
    [SerializeField] float _spawnBreak = 4f;
    [SerializeField] Vector3 _spawnPositionOffset;
    [SerializeField] T_Player _player;
    [SerializeField] float _multiplierSpeed = 0.005f;
    [SerializeField] float _rotationSpeed = 10f;
    Dictionary<string, string[]> _enemiesPatterns = new Dictionary<string, string[]>{
        {"00000000000", new string[]{ 
            "00000000000", "00000000000", "00000000000", "00000000000", "00000000000",
            "00000100000", "00000000100", "00100000000", "00000000010", "01110111101",
            "00000200000", "00000000200", "00200000000", "00000000020", "02220222202",
            "00000300000", "00000000300", "00300000000", "00000000030", "03330333303",
            "00200100300", "00000100030"}},
        {"00000100000", new string[]{ "00000000000", "00000000100", "00100000000", "00000000010", "01111111111"}},
        {"00000000100", new string[]{ "00000100000", "00000000000", "00100000000", "00000000010", "01111111111"}},
        {"00100000000", new string[]{ "00000100000", "00000000100", "00000000000", "00000000010", "01111111111"}},
        {"00000000010", new string[]{ "00000100000", "00000000100", "00100000000", "00000000000", "01111111111"}},
        {"01110111101", new string[]{ "01110111101", "01110111101", "00000000000"}},
        {"01111111111", new string[]{ "00000100000", "00000000100", "00000000000", "00000000010", "01111111111", "01111111111"}},
        {"00000200000", new string[]{ "00000000000", "00000000200", "00200000000", "00000000020", "02222222222"}},
        {"00000000200", new string[]{ "00000200000", "00000000000", "00200000000", "00000000020", "02222222222"}},
        {"00200000000", new string[]{ "00000200000", "00000000200", "00000000000", "00000000020", "00000000030","02222222222"}},
        {"00000000020", new string[]{ "00000200000", "00000000200", "00200000000", "00000000000", "02222222222"}},
        {"02220222202", new string[]{ "02220222202", "02220222202", "00000000000"}},
        {"02222222222", new string[]{ "00000200000", "00000000200", "00000000000", "00000000020", "02222222222", "02222222222"}},
        {"00000300000", new string[]{ "00000000000", "00000000300", "00300000000", "00000000030", "03333333333"}},
        {"00000000300", new string[]{ "00000300000", "00000000000", "00300000000", "00000000030", "03333333333"}},
        {"00300000000", new string[]{ "00000300000", "00000000300", "00000000000", "00000000030", "03333333333"}},
        {"00000000030", new string[]{ "00000300000", "00000000300", "00300000000", "00000000000", "03333333333"}},
        {"03330333303", new string[]{ "03330333303", "03330333303", "00000000000"}},
        {"03333333333", new string[]{ "00000300000", "00000000300", "00000000000", "00000000030", "03333333333", "03333333333"}},
        {"12301230123", new string[]{ "01000300200", "00200100300", "00000000000", "00000100030", "12301230123", "12301230123"}},
        {"01000300200", new string[]{ "00000000000", "00200100300", "00000000000", "00000100030", "12301230123", "12301230123"}},
        {"00200100300", new string[]{ "00000000000", "00200100300", "00000000000", "00000100030", "12301230123", "12301230123"}},
        {"00000100030", new string[]{ "00000000000", "00000100030", "00200100300", "00000100030", "12301230123", "12301230123"}},
    };


    List<T_Segment> _spawned = new List<T_Segment>();
    List<Vector3> _positions = new List<Vector3>();
    List<string> _patterns = new List<string>() {
        "00000000000", "00000000000", "00000000000", "00000000000", "00000000000", "00000000000"
    };
    List<Color> _colors = new List<Color> { Color.blue};

    float timer = 0;
    float rotation = 0;
    Vector3 _tripBegin;

    public static float MULTIPLER = 1.0f;

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.P)) T_Segment.Stop = !T_Segment.Stop;

        if(T_Segment.Stop) return;

        MULTIPLER += _multiplierSpeed * Time.deltaTime;

        timer -= Time.deltaTime;
        rotation += Time.deltaTime * _rotationSpeed * MULTIPLER;

        rotation = rotation % 360;

        if(timer < 0){
            timer += _spawnBreak * Mathf.Max(3.0f - MULTIPLER, 0.5f);

            T_Segment segment = GetNextSegment();
            _spawned.Add(segment);

            if(_positions.Count == 0) GeneratePositions();
            if(_colors.Count == 1) GenerateColors();
            if(_patterns.Count == 1) _patterns.Add(
                    _enemiesPatterns[_patterns[0]][Random.Range(0, _enemiesPatterns[_patterns[0]].Length)]
                );

            segment.Setup(_positions[0] + _spawnPositionOffset, rotation, _colors[0], _patterns[0]);
            _positions.RemoveAt(0);
            _colors.RemoveAt(0);
            _patterns.RemoveAt(0);

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

    void GenerateColors(){
        Color colors = _colors[0];


        Color targetColor = new Color( 
            Mathf.Max(
                Mathf.Min(
                    colors.r + ((Random.Range(0,2) == 0)? - Random.Range(0, 0.5f) : Random.Range(0, 0.5f)),
                    1), 
                0),
            Mathf.Max(
                Mathf.Min(
                    colors.r + ((Random.Range(0,2) == 0)? - Random.Range(0, 0.5f) : Random.Range(0, 0.5f)),
                    1), 
                0),
            Mathf.Max(
                Mathf.Min(
                    colors.r + ((Random.Range(0,2) == 0)? - Random.Range(0, 0.5f) : Random.Range(0, 0.5f)),
                    1),
                0),
                1
            );

        if(targetColor.r < 0.1f && targetColor.g < 0.1f && targetColor.b < 0.1f){
            targetColor = Color.magenta;
        }


        for(float i = 1; i< 8; i++){
            float percent = i/7;
            _colors.Add( colors *(1.0f-percent)  + targetColor * percent );
        }


    }


    void SortCanvas(){
        for(int i = 0; i < _spawned.Count; i++) {
            if(!Guard.IsValid(_spawned[i])) continue;
            _spawned[i].GetComponent<Canvas>().sortingOrder = i;
        }
    }

    T_Segment GetNextSegment(){

    //    Debug.Log("GetNextSegment :--> Begin");

        for(int i = 0; i < _spawned.Count; i++) {
            T_Segment segment = _spawned[i];
            if(!Guard.IsValid(segment)) {
                _spawned.RemoveAt(i);
                break;
            }
        }

    //    Debug.Log("GetNextSegment :--> End");

        T_Segment segment1 = Instantiate(_prefab, new Vector3(), Quaternion.identity, transform);
        segment1.GetComponent<Canvas>().sortingOrder = _spawned.Count;
        segment1.name += _spawned.Count;
        return segment1;
    }
}
