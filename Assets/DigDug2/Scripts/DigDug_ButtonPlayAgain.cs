using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigDug_ButtonPlayAgain : MonoBehaviour, IListenToGameplayEvents
{
    private void Awake() {
        gameObject.SetActive(!DigDugPlayedMaps.IsFull());
        Events.Gameplay.RegisterListener(this, GameplayEventType.SpamWithWindow);
    }

    public void OnGameEvent(GameplayEvent gEvent){
        if(gEvent.type == GameplayEventType.SpamWithWindow){
            gameObject.SetActive(!DigDugPlayedMaps.IsFull());
        }
    }
}
