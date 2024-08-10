using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_AltileryTower : BS_MainTower
{
    public override void Shoot(Vector3 direction, MonoBehaviour spawner, bool canShoot, bool canShootContinuesly, bool ignoreShoot = true){
        if(!canShoot) return;

        Vector2 playerPos = PlayerList<BS_Player>.Get(PlayerIndex.Player1).transform.position;

        BS_AltileryMissleSpawner missle = Instantiate(
                            _missle, 
                            playerPos, 
                            Quaternion.identity, 
                            BS_Instances.Inst.transform
                        ).GetComponent<BS_AltileryMissleSpawner>();
        missle.SetupParent(spawner);
        

    }
}
