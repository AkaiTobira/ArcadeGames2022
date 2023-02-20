using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEnemyMissle : ABMissle
{
    protected override float GetSpeedMultiplier()
    {
        return 1.0f + BENEMY_CONSTS.MISSLE_SPEEDUP * BLevelsManager.CurrentLevel; 
    }

    protected override void OnMissleHit(Collider2D other)
    {
        if(other.CompareTag("Obstacle")){
            Destroy(gameObject);
        }else if(other.CompareTag("Missle")){
            Destroy(gameObject);
        }else if(other.CompareTag("Enemy")){
            Destroy(gameObject);
        }else if(other.CompareTag("Player")){
            Destroy(gameObject);
        }
    }
}
