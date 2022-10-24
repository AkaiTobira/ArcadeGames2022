using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreTimeLabel : HighScoreLabel
{
    override protected string FormatScore(int score){
        return Fromater.FormatToTime(score);
    }
}
