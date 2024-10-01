using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverActivator : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] GameObject[] LoseTextes;
    [SerializeField] Transform ScreenCenter;
    [SerializeField] Button _endButton;
    [SerializeField] SceneLoader _sceneLoader;
    [SerializeField] TimerCount _timer;
    [SerializeField] DD2_StartLevel _StartLevel;

    public static bool isGameOverReach = false;

    private void Start() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.GameOver);
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){
        if(isGameOverReach) return;
        if(gameplayEvent.type == GameplayEventType.GameOver){
            GameOver reason = (GameOver)gameplayEvent.parameter;
            isGameOverReach = true;
//            _timer.gameObject.SetActive(false);
            AudioSystem.Instance.PlayMusic("DigDug_BG1", 0.2f);
            DD2_GameOverText.reason = reason;

            switch (reason) {
                case GameOver.Kill:
                case GameOver.Victory: 
                    
                    

                    AudioSystem.Instance.PlayEffect("DigDug_Victory", 1);
                    HighScoreRanking.LoadRanking(GameType.DigDug2);

                    DigDugPlayedMaps.LockMap(LevelManager.SelectedLevel);
                    _StartLevel.ChangeVisisbility(true);
                    
                    break;
                case GameOver.Dead: 
                    
                    LoseTextes[0].SetActive(true);
                    DigDugger.Player.enabled = false;
                    AudioSystem.Instance.PlayEffect("DigDug_Dead",1);
                    
                    TimersManager.Instance.FireAfter(3f, () => _sceneLoader.OnSceneLoadAsync());
                    break;
                case GameOver.TimesUp:
                    LoseTextes[1].SetActive(true);
                    DigDugger.Player.enabled = false;
                    AudioSystem.Instance.PlayEffect("DigDug_Dead",1);
                    
                    TimersManager.Instance.FireAfter(3f, () => _sceneLoader.OnSceneLoadAsync());
                    break;
            }

            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.LocalizationUpdate));
        }
    }

}
