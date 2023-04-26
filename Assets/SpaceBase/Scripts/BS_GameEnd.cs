using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_GameEnd : MonoBehaviour, IListenToGameplayEvents
{

    [SerializeField] SceneLoader loader;

    private void Awake() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.GameOver);
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){

        if(gameplayEvent.type == GameplayEventType.GameOver){
            GameOver overType = (GameOver)gameplayEvent.parameter;
            BS_EndTextSelect._reason = overType;
            switch(overType){
                case GameOver.Victory:
                case GameOver.Dead:
                    loader.OnSceneLoadAsync();
                break;
                default: return;
            }
        }
    }
}
