using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DD2_GameOverText : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI _text;
    [SerializeField] public string text1;
    [SerializeField] public string text2;

    public static GameOver reason;

    private void Awake() {
        _text.GetComponent<AutoTranslatorUnit>().SetTag( reason == GameOver.Victory ? text1 : text2);
    }
}
