using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPreview : MonoBehaviour
{
    [SerializeField] Image[] _mitos;
    [SerializeField] Image[] _rybos;
    [SerializeField] Image   _centros;

    [SerializeField] Sprite _bad;

    public void Setup(int big, int med, int small){

        if(big != 0) _centros.sprite = _bad;
        for(int i =0; i < med ; i++) _rybos[i].sprite = _bad; 
        for(int i =0; i < small ; i++) _mitos[i].sprite = _bad; 

    }



}
