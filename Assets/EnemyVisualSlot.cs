using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVisualSlot : MonoBehaviour
{
    [SerializeField] Image[] enemies;

    public void Setup(int enemyId){
        for(int i = 0; i < enemies.Length; i++) {
            enemies[i].enabled = (i == enemyId);
        }
    }
}
