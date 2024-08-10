using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingRepeater : MonoBehaviour
{
    [SerializeField] List<int> _screenIndexes = new List<int>();
    [SerializeField] ScreenAnimation _screenAnimation;
    [SerializeField] GameType type;

    private void Awake(){
        HighScoreRanking.LoadRanking(type);
        List<KeyValuePair<int,string>> ranking = HighScoreRanking.GetRankingCopy();

        for(int i = 0; i < _screenIndexes.Count; i++){
            ranking.Add(new KeyValuePair<int,string>(PointsCounter.GetScore((PlayerIndex)i), "-" + i));
            _screenAnimation.IgnoreScreen(_screenIndexes[i], true);
        }

        if(HighScoreRanking.IsTimed(type)) ranking.Sort((x, y) => {return x.Key - y.Key;});
        else ranking.Sort((x, y) => {return y.Key - x.Key;});

        CUtils.PrintContainer(ranking);

        bool hasAnyEnabled = false;
        for(int i = 0; i < HighScoreRanking.NUMBER_OF_RECORDS; i++)
        {
            for(int j = 0; j < _screenIndexes.Count; j++){
                if(ranking[i].Value == "-" + j){
                    _screenAnimation.IgnoreScreen(_screenIndexes[j], false);
                    hasAnyEnabled = true;

                    Debug.Log( "-" + j);
                }
            }
        }

        if(!hasAnyEnabled) _screenAnimation.IgnoreScreen(_screenIndexes[0], false);
    }
}
