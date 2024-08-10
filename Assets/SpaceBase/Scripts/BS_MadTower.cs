using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_MadTower : BS_MainTower
{
    [SerializeField] float _rageLenght = 3f;
    [SerializeField] float _rageStep = 0.2f;

    public override void Shoot(Vector3 direction, MonoBehaviour spawner, bool canShoot, bool canShootContinuesly, bool ignoreShoot = true){
        if(!canShoot) return;
        if(RotationLocked) return;

        RotationLocked = true;
        RotationManager.Instance.RotateBy(transform, new Vector3(0,0,360), _rageLenght, () => RotationLocked = false);

        for(float i = 0; i <= _rageLenght; i+= _rageStep){
            TimersManager.Instance.FireAfter(i, () => Shot(spawner));
        } 
    }

    private void Shot(MonoBehaviour spawner){
        if(!RotationLocked) return;
        AudioSystem.PlaySample(shotSound, 1, true);

        LF_ColliderSide side = Instantiate(
            _missle, 
            _missleSpawnPoint[0].position, 
            Quaternion.identity, 
            BS_Instances.Inst.transform).GetComponent<LF_ColliderSide>();
        side.SetParent(spawner);
        side.GetComponent<BS_Missle>().Setup(transform.up);

        side = Instantiate(
            _missle, 
            _missleSpawnPoint[1].position, 
            Quaternion.identity, 
            BS_Instances.Inst.transform).GetComponent<LF_ColliderSide>();
        side.SetParent(spawner);
        side.GetComponent<BS_Missle>().Setup(-transform.up);
    }
}
