using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreLabel : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] int _labelIndex;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _score;

    private void Start() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.GameOver);
        Events.Gameplay.RegisterListener(this, GameplayEventType.RefreshRanking);
        Events.Gameplay.RegisterListener(this, GameplayEventType.SaveRankings);
    }

    private void OnEnable() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.GameOver);
        Events.Gameplay.RegisterListener(this, GameplayEventType.RefreshRanking);
        Events.Gameplay.RegisterListener(this, GameplayEventType.SaveRankings);
    }


    public void OnGameEvent(GameplayEvent gameplayEvent){
        //if(enabled == false) return;
        if(gameplayEvent.type == GameplayEventType.GameOver) SetLabel();
        else if(gameplayEvent.type == GameplayEventType.RefreshRanking) SetLabel();
        else if(gameplayEvent.type == GameplayEventType.SaveRankings) SetLabel();
    }

    private void SetLabel(){
        KeyValuePair<int, string> rankingEntry = HighScoreRanking.GetScore(_labelIndex);
        _name.text = rankingEntry.Value;
        _score.text = FormatScore(rankingEntry.Key);

        Debug.Log("Setting up : " + rankingEntry.Value + " " + FormatScore(rankingEntry.Key));
    }

    virtual protected string FormatScore(int score){
        return Fromater.FormatToPoints(score);
    }
}
