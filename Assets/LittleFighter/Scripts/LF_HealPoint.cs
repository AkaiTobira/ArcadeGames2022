using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LF_HealPoint : MonoBehaviour, IPickedUp
{
    [SerializeField] float pointsRestored = 10;
    public void PickedUp(PlayerIndex side){
        PlayerList<LF_Player>.Get(side).TakeDamage(-pointsRestored);
        LF_HealSpawner._Timer = 15;
        Destroy(gameObject);
        LF_IntroTexts.ShowPowerUp();
    }
}
