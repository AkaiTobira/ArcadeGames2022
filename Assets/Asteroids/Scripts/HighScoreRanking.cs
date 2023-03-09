using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameType{
    NotLoaded,
    Asteroids,
    DigDug,
    Frogger,
    Berzerk,
    LittleFighter,
    SpaceBase
}

public static class HighScoreRanking{

    private static GameType _currentlyLoaded;
    private static int newIndex;

    private const int MAX_TIME   = 359999;
    private const int ZERO_POINT = 0;

    private static Dictionary<GameType, bool> IsTimed = new Dictionary<GameType, bool>{
        {GameType.NotLoaded, false},
        {GameType.Asteroids, false},
        {GameType.DigDug, true},
        {GameType.Frogger, true},
        {GameType.Berzerk, false},
        {GameType.LittleFighter, false},
        {GameType.SpaceBase, false},
    };

    private static List<KeyValuePair<int, string>> _ranking = new List<KeyValuePair<int, string>>();

    public static void LoadRanking( GameType game ){
        if(_currentlyLoaded == game) return;
        _currentlyLoaded = game;
        _ranking.Clear();


        for(int i = 0; i < 8; i++){
            string playerName = PlayerPrefs.GetString( game.ToString() + "Rank_Name" + i.ToString(), "..........");
            int playerScore   = PlayerPrefs.GetInt   ( 
                game.ToString() + "Rank_Score" + i.ToString(), 
                IsTimed[_currentlyLoaded] ? MAX_TIME : ZERO_POINT);

            _ranking.Add(new KeyValuePair<int, string>(playerScore, playerName));
//            Debug.Log(_ranking[i].Key + " " + _ranking[i].Value);
        }
    }

    public static void SaveRanking(){
        for(int i = 0; i < 8; i++){
            PlayerPrefs.SetString( _currentlyLoaded.ToString() + "Rank_Name" + i.ToString(), _ranking[i].Value);
            PlayerPrefs.SetInt   ( _currentlyLoaded.ToString() + "Rank_Score" + i.ToString(), _ranking[i].Key);
        }
    }

    public static void TryAddNewRecord(int newScore){
        _ranking.Add( new KeyValuePair<int, string>(newScore, ""));

        SortRanking();
        _ranking.RemoveAt(_ranking.Count-1);

        //string a = "";
        //for(int i = 0; i < _ranking.Count; i++) {
        //    a += _ranking[i].Key + " " + _ranking[i].Value + "\n";
        //}
        //Debug.LogWarning(a);
    }

    private static void SortRanking(){
        if(IsTimed[_currentlyLoaded]) _ranking.Sort( (x, y) => {return x.Key - y.Key;});
        else _ranking.Sort( (x, y) => {return y.Key - x.Key;});
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

    public static int HasNewRecord(){
        for(int i = 0; i < 8; i++) {
            if(IsNew(i)) return i;
        }

        return -1;
    }
}
