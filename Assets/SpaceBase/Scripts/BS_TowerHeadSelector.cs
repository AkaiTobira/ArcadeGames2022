using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_TowerHeadSelector : MonoBehaviour
{
    [Serializable] class Tower {
        [SerializeField] public UpgradeType type;
        [SerializeField] public BS_MainTower[] towers;
    }

    [SerializeField] Tower[] _towers;

    public BS_MainTower GetTower(UpgradeType type, int strenght)
    {
        for(int i = 0; i < _towers.Length; i++){
            for(int j = 0; j < _towers[i].towers.Length; j++){
                _towers[i].towers[j].gameObject.SetActive(false);
            }
        }

        for(int i = 0; i < _towers.Length; i++){
            if(_towers[i].type == type){
                BS_MainTower tower = _towers[i].towers[Mathf.Min(_towers[i].towers.Length-1, strenght)];
                tower.gameObject.SetActive(true);

                return tower;
            }
        }

        return null;
    }
}
