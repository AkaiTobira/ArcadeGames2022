using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class HighScoreRanking{

    private static int newIndex;

    private static List<KeyValuePair<int, string>> _ranking = new List<KeyValuePair<int, string>>();

    public static void LoadRanking(){
        for(int i = 0; i < 8; i++){
            string playerName = PlayerPrefs.GetString("Rank_Name" + i.ToString(), "..........");
            int playerScore   = PlayerPrefs.GetInt   ("Rank_Score" + i.ToString(), 0);
            _ranking.Add(new KeyValuePair<int, string>(playerScore, playerName));
            Debug.Log(_ranking[i].Key + " " + _ranking[i].Value);
        }
    }

    public static void SaveRanking(){
        for(int i = 0; i < 8; i++){
            PlayerPrefs.SetString("Rank_Name" + i.ToString(), _ranking[i].Value);
            PlayerPrefs.SetInt   ("Rank_Score" + i.ToString(), _ranking[i].Key);
        }
    }

    public static void TryAddNewRecord(int newScore){
        _ranking.Add( new KeyValuePair<int, string>(newScore, ""));
        _ranking.Sort( (x, y) => {return y.Key - x.Key;});

        _ranking.RemoveAt(_ranking.Count-1);
    }

    public static KeyValuePair<int, string> GetScore(int index){
        if(index < 8){
            return _ranking[index];
        }

        return new KeyValuePair<int, string>(0,"");
    }

    public static void SaveName(int index, string name){
        if(name == "") name = "No Name";
        _ranking[index] = new KeyValuePair<int, string>(_ranking[index].Key, name);
    }

    public static bool IsNew(int index){
        return _ranking[index].Value == "";
    }
}

public class HighScoreLabel : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] int _labelIndex;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _score;
    [SerializeField] TMP_InputField _inputField;
    
    [SerializeField] TextMeshProUGUI _preview;

    private void Start() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.GameOver);
        Events.Gameplay.RegisterListener(this, GameplayEventType.RefreshRanking);
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){
        //if(enabled == false) return;
        if(gameplayEvent.type == GameplayEventType.GameOver){
            KeyValuePair<int, string> rankingEntry = HighScoreRanking.GetScore(_labelIndex);

            _inputField.enabled = HighScoreRanking.IsNew(_labelIndex);

            _inputField.text = rankingEntry.Value;
            _name.text = rankingEntry.Value;
            _score.text = rankingEntry.Key.ToString().PadLeft(8, '0');
        }

        if(gameplayEvent.type == GameplayEventType.RefreshRanking){
            if(_inputField.enabled) _inputField.ActivateInputField();
            else{

                _preview.gameObject.SetActive(false);

                KeyValuePair<int, string> rankingEntry = HighScoreRanking.GetScore(_labelIndex);
                _inputField.text = rankingEntry.Value;
                _name.text = rankingEntry.Value;
            }
        }
    }

    public void OnTextEnter(string s){}

    public void OnEditEnd(string s){
        _inputField.enabled = false;
        _name.text = s;
        HighScoreRanking.SaveName(_labelIndex, s);
        HighScoreRanking.SaveRanking();
    }
}
