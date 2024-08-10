using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum UpgradeType{
    Missle = 0,
    Rocket = 5,
    Laser,
    Flamethower,
    Health,
    Score,
    Invincible,
    Mad,
}


public class BS_Upgrade : MonoBehaviour, IPickedUp
{
    [SerializeField] UpgradeType _type;
    [SerializeField] int _strenght;
    [SerializeField] TextMeshProUGUI _text;

    public UpgradeType GetUpgradeType() { return _type; }

    private void Awake(){
        _text.SetText(_type.ToString().Substring(0,1));
    }

    public void PickedUp(PlayerIndex side)
    {
        BS_Player player = PlayerList<BS_Player>.Get(side);    
        switch (_type)
        {
            case UpgradeType.Health: player.TakeDamage(-_strenght); break;
            case UpgradeType.Missle: 
            case UpgradeType.Laser: 
            case UpgradeType.Rocket: 
            case UpgradeType.Flamethower: player.SwapWeapon(_type) ; break;
            case UpgradeType.Score: PointsCounter.AddPoints(side, _strenght); break;
            case UpgradeType.Invincible: player.InvincibleCollected(_strenght); break;
        }

        Debug.LogWarning("PuckedUp " + _type + " " + side);

        Destroy(gameObject);
    }
}
