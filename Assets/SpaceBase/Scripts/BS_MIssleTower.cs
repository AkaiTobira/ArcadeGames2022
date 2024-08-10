using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_MissleTower : BS_MainTower
{
    public override void Shoot(Vector3 direction, MonoBehaviour spawner, bool canShoot, bool canShootContinuesly, bool ignoreShoot = true){
        if(!canShoot) return;

        if(!ignoreShoot){
            Transform aimedTarget = ScanForAimHelper(_missleSpawnPoint[0]);

            AudioSystem.PlaySample(shotSound, 1, true);
            //Aim Helper;
            if(Guard.IsValid(aimedTarget)){
                Vector3 aimedDirection = (aimedTarget.position - transform.position).normalized;
                if(aimedDirection.sqrMagnitude < 2500 && Vector3.Angle(aimedDirection, direction) < 40){
                    direction = (aimedTarget.position -  transform.position).normalized;
                }
            //    Debug.Log("Shoot" + direction + " " + transform.up + " " + Vector3.Angle(aimedTarget, _towerHead.transform.up));
            } 
        }

        
        for(int i = 0; i < _missleSpawnPoint.Length; i++){
            LF_ColliderSide side = 
                Instantiate(
                    _missle, 
                    _missleSpawnPoint[i].position, 
                    Quaternion.identity, 
                    BS_Instances.Inst.transform
                ).GetComponent<LF_ColliderSide>();
            side.SetParent(spawner);
            side.GetComponent<BS_Missle>().Setup(direction);
        }
    }

    
}
