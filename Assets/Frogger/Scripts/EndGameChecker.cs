using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameChecker : CUpdateMonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] Target[] objects;
    [SerializeField] Button _mainMenuButton;
    [SerializeField] Transform centerPoint;
    [SerializeField] Transform bottomPoint;

    [SerializeField] Frogger _frogger1;
    [SerializeField] GameObject[] _hpInterface1;
    [SerializeField] TextMeshProUGUI _player1GameOver;
    [SerializeField] SceneLoader _loaderWin1;

    [SerializeField] Frogger _frogger2;
    [SerializeField] GameObject[] _hpInterface2;
    [SerializeField] TextMeshProUGUI _player2GameOver;
    [SerializeField] SceneLoader _loaderWin2;
    
    [SerializeField] TextMeshProUGUI _player2JoinText;
    [SerializeField] SceneLoader _loaderLose;


    int HealthPoints1 = 2;
    int HealthPoints2 = 2;
    bool player2Active = false;

    protected override void Awake() {
        base.Awake();
        AudioSystem.Instance.PlayMusic("Frogger_BG", 1);
        HighScoreRanking.LoadRanking(GameType.Frogger);
        HealthPoints1 = 2;
        HealthPoints2 = 2;
        _player2JoinText.enabled = true;
        player2Active = false;
        _frogger2.gameObject.SetActive(false);

        objects[0].gameObject.SetActive(false);
        objects[4].gameObject.SetActive(false);

        UpdateHP();
        Events.Gameplay.RegisterListener(this, GameplayEventType.PlayerDied);
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){
        if(gameplayEvent.type == GameplayEventType.PlayerDied){
            PlayerIndex playerIndex = (PlayerIndex)gameplayEvent.parameter;

            if(playerIndex == PlayerIndex.Player1){
                HealthPoints1--;
                if(HealthPoints1 < 0){
                    _frogger1.DoNotRespawn = true;
                }
            }

            if(playerIndex == PlayerIndex.Player2){
                HealthPoints2--;
                if(HealthPoints2 < 0){
                    _frogger2.DoNotRespawn = true;
                }
            }

            UpdateHP();

            if(_frogger1.DoNotRespawn && (_frogger2.DoNotRespawn || !player2Active)){
                TimersManager.Instance.FireAfter(1f, () =>{
                    AudioSystem.Instance.PlayMusic("Frogger_BG", 0.2f);
                    _loaderLose.OnSceneLoadAsync();
                } );
            }
        }
    }

    private void UpdateHP(){
        if(HealthPoints1 < 0) _player1GameOver.gameObject.SetActive(true);
        for(int i = 0; i < _hpInterface1.Length; i++){
            _hpInterface1[i].SetActive(i <= HealthPoints1);
        }

        if(HealthPoints2 < 0) _player2GameOver.gameObject.SetActive(true);
        for(int i = 0; i < _hpInterface2.Length; i++){
            _hpInterface2[i].SetActive(i <= HealthPoints2 && player2Active);
        }
    }

    public override void CUpdate()
    {
        if(!enabled) return;
        if(!player2Active && InputHandler.GetKey(InputKey.Action_Any_Player2))
        {
            player2Active = true;
            _frogger2.gameObject.SetActive(true);
            UpdateHP();
            _player2JoinText.gameObject.SetActive(false);

            objects[0].gameObject.SetActive(true);
            objects[4].gameObject.SetActive(true);
        }

        int score1 = 0, score2 = 0;

        for(int i = 0; i < objects.Length; i++){
            if(objects[i].IsPlayerOneActive()) score1++;
            if(objects[i].IsPlayerTwoActive()) score2++;
        } 

        if(score1 > 2 || score2 > 2){
            enabled = false;
            _frogger1.DoNotRespawn = true;
            _frogger2.DoNotRespawn = true;

            
            TimersManager.Instance.FireAfter(1f, () =>{
                AudioSystem.Instance.PlayMusic("Frogger_BG", 0.2f);
                AudioSystem.Instance.PlayEffect("Frogger_Victory", 1);
                if(score1 > score2) _loaderWin1.OnSceneLoadAsync();
                else _loaderWin2.OnSceneLoadAsync();
            } );
        }

        base.CUpdate();
    }
}
