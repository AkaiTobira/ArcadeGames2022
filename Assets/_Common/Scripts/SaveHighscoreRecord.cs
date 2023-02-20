using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveHighscoreRecord : MonoBehaviour
{
    public void SaveRecord(){
        HighScoreRanking.TryAddNewRecord(TimerCount.ElapsedTime);
    }
}
