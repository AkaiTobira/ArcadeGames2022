using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface IPickedUp{
    void PickedUp();
}

public class LF_HealSpawner : CMonoBehaviour
{
    [SerializeField] LF_HealPoint _healPrefab;

    private LF_HealPoint Spawned;
    public static float _Timer = 3f;

    void Spawn(){
        RectTransform rt = GetComponent<RectTransform>();

        Spawned = Instantiate(
            _healPrefab, 
            CUtils.GetPointInsideRectTransform(rt), 
            Quaternion.identity, rt);
    }


    private void Update() {
        _Timer -= Time.deltaTime;
        if(Guard.IsValid(Spawned)) return;
        if(_Timer > 0) return; 

        Spawn();
    }


}
