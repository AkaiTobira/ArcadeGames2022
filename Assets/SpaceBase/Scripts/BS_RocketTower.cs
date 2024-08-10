using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BS_RocketTower : BS_MainTower
{
    [SerializeField] float _loadingTimeMax = 2f;
    [SerializeField] Image _image;

    private float _elapsedTime;

    public override void Shoot(Vector3 direction, MonoBehaviour spawner, bool canShoot, bool canShootContinuesly, bool ignoreShoot = true){
        if(canShoot){ _elapsedTime = 0; }
        else if(canShootContinuesly && _elapsedTime < _loadingTimeMax) { 
            _elapsedTime += Time.deltaTime; 
            
        }else if((!canShoot && !canShootContinuesly && _elapsedTime > 0) || _elapsedTime > _loadingTimeMax){
            float percent = _elapsedTime / _loadingTimeMax;
            _image.fillAmount = percent;

            AudioSystem.PlaySample(shotSound, 1, true);
            int numberOfActiveShoots = (int)(percent * _missleSpawnPoint.Length);
//            Debug.LogWarning(numberOfActiveShoots + " " + (int)(percent * _missleSpawnPoint.Length) + " " + percent);

            for(int i = 0; i < numberOfActiveShoots; i++)
            {
                Vector2 aimDir = _missleSpawnPoint[i].up;
                Transform aimedTarget = ScanForAimHelper(_missleSpawnPoint[i]);

                //Aim Helper;
                if(Guard.IsValid(aimedTarget)){
                    Vector3 aimedDirection = (aimedTarget.position - transform.position).normalized;
                    if(aimedDirection.sqrMagnitude < 2500 && Vector3.Angle(aimedDirection, aimDir) < 40){
                        aimDir = (aimedTarget.position -  transform.position).normalized;
                    }
                //    Debug.Log("Shoot" + direction + " " + transform.up + " " + Vector3.Angle(aimedTarget, _towerHead.transform.up));
                }

                LF_ColliderSide side = 
                    Instantiate(
                        _missle, 
                        _missleSpawnPoint[i].position, 
                        Quaternion.identity, 
                        BS_Instances.Inst.transform
                    ).GetComponent<LF_ColliderSide>();
                side.SetParent(spawner);
                side.GetComponent<BS_HomingMissle>().Setup(aimDir, aimedTarget);
            }

            _elapsedTime = 0;
        } else if(!canShootContinuesly) { _elapsedTime = 0; }
        _image.fillAmount = _elapsedTime / _loadingTimeMax; 
    }
}
