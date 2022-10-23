using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreTimeLabel : HighScoreLabel
{
    override protected string FormatScore(int score){
        string time = (score / 3600).ToString().PadLeft(2, '0') + ":";
        time += (score / 60).ToString().PadLeft(2, '0') + ":";
        time += (score % 60).ToString().PadLeft(2, '0');

        return time;
    }
}
