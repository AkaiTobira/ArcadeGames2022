using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggersCounter : MonoBehaviour
{
    public static int ActiveDiggers = 0;

    [SerializeField] int diggerCounter = 0;

    private bool shotEvent = true;

    void Awake()
    {
        ActiveDiggers = 0;
        shotEvent = true;
    }

    void Start(){
        enabled = false;
        TimersManager.Instance.FireAfter(10, () => {
            if(Guard.IsValid(this)) enabled = true;});
    }


    // Update is called once per frame
    void Update()
    {
        diggerCounter = ActiveDiggers;
        if(ActiveDiggers == 0 && shotEvent){
            shotEvent = false;
            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.GameOver, GameOver.Victory));
        }

        if(ActiveDiggers > 0){shotEvent = true;}
    }
}
