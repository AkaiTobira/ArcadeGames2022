using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreTimeLabel : HighScoreLabel
{
    [SerializeField] bool SwitchToPoints;

    override protected string FormatScore(int score){
        if(SwitchToPoints) return base.FormatScore(score);
        return Fromater.FormatToTime(score);
    }
}
