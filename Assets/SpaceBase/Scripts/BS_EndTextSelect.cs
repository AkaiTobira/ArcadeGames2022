using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_EndTextSelect : MonoBehaviour {

    public static GameOver _reason;

    [SerializeField] GameObject _victoryText;
    [SerializeField] GameObject _loseText;

    private void Awake() {
    //    _victoryText.SetActive(_reason == GameOver.Victory);
    //    _loseText.SetActive(_reason == GameOver.Dead);
    }

}