using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWall : MonoBehaviour
{
    [SerializeField] BExitIndex exitId;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy")){
            other.GetComponent<BEnemy>().Kill();
        }
        if(other.CompareTag("Player")){
            if(gameObject.CompareTag("Obstacle")){
                other.GetComponent<Berzerk>().Kill();
            }else{
                BLevelsManager.ChangeLevel(exitId);
            }
        }
        if(other.CompareTag("Missle")){
            Destroy(other);
        }
    }
}
