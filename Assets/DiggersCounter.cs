using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggersCounter : MonoBehaviour
{
    public static int ActiveDiggers = 0;

    [SerializeField] int diggerCounter = 0;

    void Awake()
    {
        ActiveDiggers = 0;
        
    }

    void Start(){
        enabled = false;
        TimersManager.Instance.FireAfter(10, () => {enabled = true;});
    }

    // Update is called once per frame
    void Update()
    {
        diggerCounter = ActiveDiggers;
        if(ActiveDiggers == 0){
            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.GameOver, GameOver.Victory));
        }
    }
}
