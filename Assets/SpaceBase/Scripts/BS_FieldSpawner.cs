using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BS_FieldSpawner : MonoBehaviour
{
    [SerializeField] Sprite[] _floorFields;
    [SerializeField] GameObject _tile;


    private void Awake() {
        SpawnTiles();
    }

    private void SpawnTiles(){
        for(int i = 0; i < (31*31-1); i++){
            GameObject go = Instantiate(_tile, new Vector3(), Quaternion.identity, transform);
            go.GetComponent<Image>().sprite = _floorFields[Random.Range(0,_floorFields.Length)];
        }
    }
}
