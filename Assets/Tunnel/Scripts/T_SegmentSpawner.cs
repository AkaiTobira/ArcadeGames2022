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
        {"0000000000", new string[]{ 
            "0000000000", "0000000000", "0000000000", "0000000000", "0000000000",
            "0000100000", "0000000100", "0010000000", "0000000010", "0110111101",
            "0000100001", "0001000100", "0010000001", "0000100010",
            "0000200000", "0000000200", "0020000000", "0000000020", "0220222202",
            "0000200002", "0002000200", "0020000002", "0000200020",
            "0000300000", "0000000300", "0030000000", "0000000030", "0330303303",
            "0020100300", "0000100030"}},
        {"0000100000", new string[]{ "0000000000", "0000000100", "0010000000", "0000000010", "0111011101"}},
        {"0000000100", new string[]{ "0000100000", "0000000000", "0010000000", "0000000010", "0111011101"}},
        {"0010000000", new string[]{ "0000100000", "0000000100", "0000000000", "0000000010", "0111011101"}},
        {"0000000010", new string[]{ "0000100000", "0000000100", "0010000000", "0000000000", "0111011101"}},

        {"0000100001", new string[]{ "0001000100", "0000000100", "0010000000", "0001000100", "0111011101"}},
        {"0001000100", new string[]{ "0000100000", "0000000000", "0010000001", "0000000010", "0010000001"}},
        {"0010000001", new string[]{ "0000100000", "0000000100", "0000100010", "0000000010", "0111011101"}},
        {"0000100010", new string[]{ "0000100000", "0000100010", "0010000000", "0000000000", "0111011101"}},

        {"0000200002", new string[]{ "0000000000", "0000000200", "0002000200", "0000200020", "0020000002"}},
        {"0002000200", new string[]{ "0000200020", "0000000000", "0020000000", "0000000020", "0220220222"}},
        {"0020000002", new string[]{ "0000200000", "0000000200", "0000200020", "0000000020", "0000000030", "0220220222"}},
        {"0000200020", new string[]{ "0000200020", "0020000002", "0002000200", "0000000000", "0220220222"}},

        {"0110111101", new string[]{ "0110111101", "0110111101", "0000000000"}},
        {"0111011101", new string[]{ "0000100000", "0000000100", "0000000000", "0000000010", "0111011101", "0111011101"}},
        {"0000200000", new string[]{ "0000000000", "0000000200", "0020000000", "0000000020", "0220220222"}},
        {"0000000200", new string[]{ "0000200000", "0000000000", "0020000000", "0000000020", "0220220222"}},
        {"0020000000", new string[]{ "0000200000", "0000000200", "0000000000", "0000000020", "0000000030", "0220220222"}},
        {"0000000020", new string[]{ "0000200000", "0000000200", "0020000000", "0000000000", "0220220222"}},
        {"0220222202", new string[]{ "0220222202", "0220222202", "0000000000"}},
        {"0220220222", new string[]{ "0000200000", "0000000200", "0000000000", "0000000020", "0220220222", "0220220222"}},
        {"0000300000", new string[]{ "0000000000", "0000000300", "0030000000", "0000000030", "0333333333"}},
        {"0000000300", new string[]{ "0000300000", "0000000000", "0030000000", "0000000030", "0333333333"}},
        {"0030000000", new string[]{ "0000300000", "0000000300", "0000000000", "0000000030", "0333333333"}},
        {"0000000030", new string[]{ "0000300000", "0000000300", "0030000000", "0000000000", "0333333333"}},
        {"0330303303", new string[]{ "0330303303", "0330303303", "0000000000"}},
        {"0333333333", new string[]{ "0000300000", "0000000300", "0000000000", "0000000030", "0333333333", "0333333333"}},
        {"1230230123", new string[]{ "0100300200", "0020100300", "0000000000", "0000100030", "1230230123", "1230230123"}},
        {"0100300200", new string[]{ "0000000000", "0020100300", "0000000000", "0000100030", "1230230123", "1230230123"}},
        {"0020100300", new string[]{ "0000000000", "0020100300", "0000000000", "0000100030", "1230230123", "1230230123"}},
        {"0000100030", new string[]{ "0000000000", "0000100030", "0020100300", "0000100030", "1230230123", "1230230123"}},
    };


    List<T_Segment> _spawned = new List<T_Segment>();
    List<Vector3> _positions = new List<Vector3>();
    List<string> _patterns = new List<string>() {
        "0000000000", "0000000000", "0000000000", "0000000000", "0000000000", "0000000000"
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

        if(targetColor.r < 0.25f && targetColor.g < 0.25f && targetColor.b < 0.25f){
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
