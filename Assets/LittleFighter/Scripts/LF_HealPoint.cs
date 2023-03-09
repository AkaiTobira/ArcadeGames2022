using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LF_HealPoint : MonoBehaviour, IPickedUp
{
    public void PickedUp(){
        LF_Player.Player.TakeDamage(-5);
        LF_HealSpawner._Timer = 15;
        Destroy(gameObject);
    }
}
