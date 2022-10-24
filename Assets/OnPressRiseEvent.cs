using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPressRiseEvent : MonoBehaviour
{
    [SerializeField] GameplayEventType _eventType;

    public void OnButtonPressed(){
        Events.Gameplay.RiseEvent(new GameplayEvent(_eventType));
        AudioSystem.Instance.PlayEffect("Button", 1);
    }

}
