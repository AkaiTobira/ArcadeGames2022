using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResizerUnit : MonoBehaviour
{

    [SerializeField] float _targetScale;

    private void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player")){

            Debug.Log("OnPlayerEnter");

            CameraFollow.SetupSize(_targetScale);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            CameraFollow.Clear();
        }
    }

}
