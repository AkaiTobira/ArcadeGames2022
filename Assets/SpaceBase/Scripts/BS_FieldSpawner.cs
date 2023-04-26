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

        Vector2 size = new Vector2( (int)(810 / 48) + 1, (int)(750 / 36) + 1 );

        for(int i = 0; i < size.x * size.y; i++){
            GameObject go = Instantiate(_tile, new Vector3(), Quaternion.identity, transform);
            go.GetComponent<Image>().sprite = _floorFields[Random.Range(0,_floorFields.Length)];
        }
    }
}
