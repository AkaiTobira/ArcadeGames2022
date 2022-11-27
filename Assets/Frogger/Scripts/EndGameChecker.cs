using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameChecker : CUpdateMonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] GameObject[] objects;
    [SerializeField] Button _mainMenuButton;
    [SerializeField] Transform centerPoint;
    [SerializeField] Transform bottomPoint;
    [SerializeField] GameObject[] _hpInterface;
    [SerializeField] Frogger _frogger;

    [SerializeField] SceneLoader _loaderWin;
    [SerializeField] SceneLoader _loaderLose;

    GameOver _type = GameOver.Victory;

    bool continued = false;
    bool canGoToMainMenu = false;

    int HealthPoints = 2;

    void Awake() {
        AudioSystem.Instance.PlayMusic("Frogger_BG", 1);
        HighScoreRanking.LoadRanking(GameType.Frogger);
        HealthPoints = 2;
        UpdateHP();
        Events.Gameplay.RegisterListener(this, GameplayEventType.PlayerDied);
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){
        if(gameplayEvent.type == GameplayEventType.PlayerDied){
            HealthPoints--;
            UpdateHP();

            if(HealthPoints < 0){
                _frogger.DoNotRespawn = true;
                TimersManager.Instance.FireAfter(1f, () =>{
                    AudioSystem.Instance.PlayMusic("Frogger_BG", 0.2f);
                    _loaderLose.OnSceneLoadAsync();
                } );
            }
        }
    }

    private void UpdateHP(){
        for(int i = 0; i < _hpInterface.Length; i++){
            _hpInterface[i].SetActive(i <= HealthPoints);
        }
    }

    public override void CUpdate()
    {
        if(!enabled) return;

        for(int i = 0; i < objects.Length; i++) {
            if(!objects[i].activeSelf && objects[i].transform.parent.gameObject.activeSelf) return;
        }

        enabled = false;
        _frogger.DoNotRespawn = true;
        HighScoreRanking.TryAddNewRecord(TimerCount.ElapsedTime);
        
        TimersManager.Instance.FireAfter(1f, () =>{
            AudioSystem.Instance.PlayMusic("Frogger_BG", 0.2f);
            AudioSystem.Instance.PlayEffect("Frogger_Victory", 1);
            _loaderWin.OnSceneLoadAsync();
        } );

        base.CUpdate();
    }
}
