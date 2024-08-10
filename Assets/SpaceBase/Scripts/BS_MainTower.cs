using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_MainTower : MonoBehaviour
{
    [SerializeField] protected GameObject _missle;
    [SerializeField] protected Transform[] _missleSpawnPoint;
    [SerializeField] protected LayerMask _EnemylayerMask;
    [SerializeField] protected string shotSound = "SpaceBase_GunP";
    public bool RotationLocked = false;
    protected Transform ScanForAimHelper(Transform spawnPoint){

        RaycastHit2D[] hits = Physics2D.RaycastAll(spawnPoint.position, spawnPoint.up, 50, _EnemylayerMask);

        for(int i = 0; i < hits.Length; i++){
//            Debug.Log(hits[i].transform.name);
            ITakeDamage side = hits[i].transform.gameObject.GetComponent<ITakeDamage>();
            if(side != null){
                Debug.DrawLine(
                    spawnPoint.position, 
                    hits[i].transform.position, 
                    Color.red);
                return hits[i].transform;
            }
        }

        Debug.DrawLine(
            spawnPoint.position, 
            spawnPoint.position + spawnPoint.up * 50, 
            Color.blue);

        return null;
    }

    public virtual void Shoot(Vector3 direction, MonoBehaviour spawner, bool canShoot, bool canShootContinuesly, bool ignoreShoot = true){
    }
}
