using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsCounter : MonoBehaviour
{
    private TextMeshProUGUI _text;
    [SerializeField] PlayerIndex _index = PlayerIndex.Player1;
    private static int[] _score = new int[(int)PlayerIndex.None];

    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        _text.text = AutoTranslator.Translate("Score") + " " +  _score[(int)_index].ToString().PadLeft(8, '0');
    }

    public static int GetScore(PlayerIndex index){
        return _score[(int)index];
    }

    public static void AddPoints(PlayerIndex index, int score)
    {
        _score[(int)index] += score;
        if(_score[(int)index] < 0) _score[(int)index] = 0;
    }

    public static void Reset(){
        _score = new int[(int)PlayerIndex.None];
    }
}
