using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BS_UIWeaponry : CMonoBehaviour
{
    [SerializeField] Sprite[] _sprites;

    private static BS_UIWeaponry uIWeaponry;

    protected override void Awake() {
        uIWeaponry = this;
    }

    public static void SetUIWeaponry(UpgradeType type, int level){
        if(level >= 5) level = 4;

        int typeIndex = 0;
        if( type == UpgradeType.TMissle ) typeIndex = 15;
        if( type == UpgradeType.Laser ) typeIndex = 5;
        if( type == UpgradeType.Rocket ) typeIndex = 10;
        if( type == UpgradeType.Flamethower ) typeIndex = 0;
        

        if(Guard.IsValid(uIWeaponry)){
            uIWeaponry.GetComponent<Image>().sprite = uIWeaponry._sprites[typeIndex + level];
        }
    }
}
