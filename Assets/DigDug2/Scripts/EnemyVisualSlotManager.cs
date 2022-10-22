using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisualSlotManager : MonoBehaviour, IListenToGameplayEvents
{

    public static int Enemy1Count = 0;
    public static int Enemy2Count = 0;
    public static int Enemy3Count = 0;


    private int enemy1Count = 0;
    private int enemy2Count = 0;
    private int enemy3Count = 0;

    private bool gameWinEnabled;

    [SerializeField] EnemyVisualSlot[] slots;

    private void Start() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.GameOver);

        TimersManager.Instance.FireAfter(10f, () => {gameWinEnabled = true;} );
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){
        if(gameplayEvent.type == GameplayEventType.GameOver){
            Enemy1Count = 0;
            Enemy2Count = 0;
            Enemy3Count = 0;
        }
    }

    void Update()
    {
        if( enemy1Count != Enemy1Count || 
            enemy2Count != Enemy2Count || 
            enemy3Count != Enemy3Count){

// /            Debug.Log(Enemy1Count + " " + enemy2Count + " " + enemy3Count);
        };
            int activeIndex = 0;
            int counter     = 0;
            for(int i = 0; i < slots.Length; i++) {
                slots[i].gameObject.SetActive(activeIndex < 3);
                if(counter >= enemy1Count + enemy2Count + enemy3Count) activeIndex = 3;
                else if(counter >= enemy1Count + enemy2Count) activeIndex = 2;
                else if(counter >= enemy1Count) activeIndex = 1;
                
                slots[i].Setup(activeIndex);
                counter += 1;

//                Debug.Log(counter + " " + activeIndex);
            }
     //   }

        if( enemy1Count != Enemy1Count || 
            enemy2Count != Enemy2Count || 
            enemy3Count != Enemy3Count){

            enemy1Count = Enemy1Count;
            enemy2Count = Enemy2Count;
            enemy3Count = Enemy3Count;
        };
        if(enemy1Count + enemy2Count + enemy3Count == 0 && gameWinEnabled){
            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.GameOver, GameOver.Victory));
            gameWinEnabled = false;
        }
    }
}
