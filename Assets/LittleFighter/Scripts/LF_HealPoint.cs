using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LF_HealPoint : MonoBehaviour, IPickedUp
{
    [SerializeField] int pointsRestored = 10;
    public void PickedUp(){
        LF_Player.Player.TakeDamage(-pointsRestored);
        LF_HealSpawner._Timer = 15;
        Destroy(gameObject);
        LF_IntroTexts.ShowPowerUp();
    }
}
