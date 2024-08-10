using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_Instances : MonoBehaviour
{
    public static BS_Instances Inst;
    [SerializeField] BS_Upgrade[] upgrades;
    [SerializeField] int[] probs;
    [SerializeField] int spawnProbs;
    [SerializeField] int currentWeaponBoost = 10;

    private int spawnBoost = 50;

    private void Awake() {
        Inst = this;
    }

    public void DropBonus(Vector3 spawnPosition){
        if(CUtils.Rand(100) < spawnProbs + spawnProbs){
            SelectAndDropUpgrade(spawnPosition);
            spawnBoost = 0;
        }else{
            spawnBoost += 10;
        }
    }

    private void SelectAndDropUpgrade(Vector3 spawnPosition){
        int val = 0;
        for(int i = 0; i < probs.Length; i++) val += probs[i];
        val = CUtils.Rand(val + currentWeaponBoost);

        var playerUpgrade = PlayerList<BS_Player>.Get(PlayerIndex.Player1).GetUpgradeType();

        for(int i = 0; i < probs.Length; i++){
            int boost = upgrades[i].GetUpgradeType() == playerUpgrade ? currentWeaponBoost : 0; 

            if(val < probs[i] + boost) {
                Instantiate(upgrades[i], spawnPosition, Quaternion.identity, transform);
                return;
            }
            val -= probs[i];
            val -= boost;
        }
    }

}
