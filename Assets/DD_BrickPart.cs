using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_BrickPart : MonoBehaviour
{
    [SerializeField] public DD_BrickController _mainBrick;


    private void OnTriggerEnter2D(Collider2D other) {

        print(other.gameObject.name);

        gameObject.SetActive(false);
    }

}
