using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameOver{
    Victory,
    Dead,
    TimesUp
}

public class GameOverActivator : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] GameObject[] LoseTextes;
    [SerializeField] Transform ScreenCenter;
    [SerializeField] Button _endButton;
    [SerializeField] SceneLoader _sceneLoader;
    [SerializeField] TimerCount _timer;

    bool isGameOverReach = false;

    private void Start() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.GameOver);
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){
        if(isGameOverReach) return;
        if(gameplayEvent.type == GameplayEventType.GameOver){
            GameOver reason = (GameOver)gameplayEvent.parameter;
            isGameOverReach = true;
            _timer.gameObject.SetActive(false);
            AudioSystem.Instance.PlayMusic("DigDug_BG1", 0.2f);
            switch (reason) {
                case GameOver.Victory: 
                    
                    AudioSystem.Instance.PlayEffect("DigDug_Victory", 1);
                    HighScoreRanking.LoadRanking(GameType.DigDug);
                
                    TimersManager.Instance.FireAfter( 10.0f, () => {
                        
                        HighScoreRanking.TryAddNewRecord(TimerCount.ElapsedTime);
                        _sceneLoader.OnSceneLoadAsync();
                    } );
                    break;
                case GameOver.Dead: 
                    LoseTextes[0].SetActive(true);
                    DigDugger.Player.enabled = false;
                    AudioSystem.Instance.PlayEffect("DigDug_Dead",1);
                    
                    TweenManager.Instance.TweenTo(transform,ScreenCenter, 1f, () => {
                        _endButton.Select();

                        Vector3 pos = transform.position;
                        pos.z = 0;
                        transform.position = pos;
                    });
                    break;
                case GameOver.TimesUp:
                    LoseTextes[1].SetActive(true);
                    DigDugger.Player.enabled = false;
                    AudioSystem.Instance.PlayEffect("DigDug_Dead",1);
                    
                    TweenManager.Instance.TweenTo( transform,ScreenCenter, 0.2f, () => {
                        _endButton.Select();

                        Vector3 pos = transform.position;
                        pos.z = 0;
                        transform.position = pos;
                    });

                    break;
            }

            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.LocalizationUpdate));
        }
    }

}
