using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BS_FlamethowerTower : BS_MainTower
{
    [SerializeField] float _loadingTimeMax = 0.4f;

    private float _elapsedTime;
    public override void Shoot(Vector3 direction, MonoBehaviour spawner, bool canShoot, bool canShootContinuesly, bool ignoreShoot = true){
        _elapsedTime -= Time.deltaTime;
        if(canShootContinuesly){
            if(_elapsedTime < 0){
                _elapsedTime = _loadingTimeMax;

                BS_ResizableMissle missle = Instantiate(
                            _missle, 
                            _missleSpawnPoint[0].position, 
                            Quaternion.identity, 
                            BS_Instances.Inst.transform
                        ).GetComponent<BS_ResizableMissle>();

                missle.Setup(direction);
                missle.SetParent(spawner);
            }
        }
    }
}
