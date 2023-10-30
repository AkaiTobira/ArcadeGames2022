using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsCounter : MonoBehaviour
{
    private TextMeshProUGUI _text;
    public static int Score = 0;

    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        _text.text = AutoTranslator.Translate("Score") + " " +  Score.ToString().PadLeft(8, '0');
    }
}
