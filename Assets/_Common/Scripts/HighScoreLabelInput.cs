using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreLabelInput : AnimationBoard
{
    [SerializeField] PlayerIndex _index = PlayerIndex.Player1;
    [SerializeField] GameType _gameType;


    [SerializeField] TextMeshProUGUI _scoreLabel;
    [SerializeField] TextMeshProUGUI _timeLabel;

    [SerializeField] GameObject _NewHighScore;
    [SerializeField] GameObject _NoNewHighScore;

    [SerializeField] int _labelIndex;
    [SerializeField] TextMeshProUGUI _score;

    
    [SerializeField] GameObject _windows;
    [SerializeField] GameObject _horizontalArrows;
    [SerializeField] GameObject[] _markers;
    [SerializeField] TextMeshProUGUI[] _letters;

    int[] _letterIndexes = {1,1,1,0,0,0,0,0,0,0};

    char[] _possibleLetters = {
        ' ','A','B','C','D','E','F','G','H','I','J',
        'K','L','M','N','O','P','Q','R','S','T','U',
        'V','W','X','Y','Z','1','2','3','4','5','6',
        '7','8','9','0'
    };

    public override void Disable()
    {
        base.Disable();

        if(_labelIndex != -1){
            string fullName = "";
            for(int i = 0; i < _letterIndexes.Length; i++){
                fullName += _possibleLetters[_letterIndexes[i]].ToString();
            }

            HighScoreRanking.SaveName(_labelIndex, fullName);
        }

        HighScoreRanking.SaveRanking();

        Events.Gameplay.RiseEvent(GameplayEventType.RefreshRanking);
    }

    public override void Enable()
    {
        base.Enable();

        Debug.Log("Enable");

        HighScoreRanking.LoadRanking(_gameType);

        bool IsTimed =  HighScoreRanking.IsTimed(_gameType);
        if(IsTimed) HighScoreRanking.TryAddNewRecord(TimerCount.ElapsedTime);
        else HighScoreRanking.TryAddNewRecord(PointsCounter.GetScore(_index));

        _labelIndex = HighScoreRanking.HasNewRecord();

        SetPoints();

        bool hasNewRecord = _labelIndex != -1;

        if(Guard.IsValid(_NoNewHighScore)) _NoNewHighScore.SetActive(!hasNewRecord);

        if(Guard.IsValid(_NewHighScore)) _NewHighScore.SetActive(hasNewRecord);
        if(Guard.IsValid(_windows)) _windows.SetActive(hasNewRecord);
        if(Guard.IsValid(_horizontalArrows)) _horizontalArrows.SetActive(hasNewRecord);

        HandleWindowsInput();
    }

    int stringIndex = 0;


    float _elapsedTime1 = 0;
    float _elapsedTime2 = 0;


    private void Update() {
        //if(_ignoreSetup) return;

        _elapsedTime1 -= Time.deltaTime;
        _elapsedTime2 -= Time.deltaTime;
        
        float verticalChange   = InputHandler.GetVertical(_index);
        float horizontalChange = InputHandler.GetHorizontal(_index);

        if(_elapsedTime1 < 0 && Mathf.Abs(verticalChange) > 0.3f){
            _elapsedTime1 = 0.2f;
            OnButtonVertical((int) Mathf.Sign(verticalChange));
        }

        if(_elapsedTime2 < 0 && Mathf.Abs(horizontalChange) > 0.3f){
            _elapsedTime2 = 0.2f;
            OnButtonHorizontal((int) Mathf.Sign(horizontalChange));
        }
    }

    private void SetPoints(){
// /        Debug.LogWarning("SetPoints");
        bool IsTimed =  HighScoreRanking.IsTimed(_gameType);

        int points = IsTimed ? TimerCount.ElapsedTime : PointsCounter.GetScore(_index);
        if(Guard.IsValid(_score)){
            _score.text = IsTimed ? Fromater.FormatToTime(points) : Fromater.FormatToPoints(points);
        }
        if(Guard.IsValid(_scoreLabel)) _scoreLabel.gameObject.SetActive(!IsTimed);
        if(Guard.IsValid(_timeLabel)) _timeLabel.gameObject.SetActive(IsTimed);
    }

    private void HandleWindowsInput(){
        ResetLetters();
        LoadLetters();
        SetupMarkers();
    }

    private void ResetLetters(){
        for(int i = 0; i <_letterIndexes.Length; i++)
        {
            if(i < 3) _letterIndexes[i] = 1;
            else _letterIndexes[i] = 0;
        }
    }

    private void SetupMarkers(){
        for(int i = 0; i < _markers.Length; i++){
            _markers[i].SetActive(stringIndex == i);
        }
    }

    private void LoadLetters(){
        for(int i = 0; i < _letters.Length; i++){
            _letters[i].text = _possibleLetters[_letterIndexes[i]].ToString();
        }
    }

    public void OnButtonVertical(int direction){

        _letterIndexes[stringIndex] = (
            _letterIndexes[stringIndex] + 
            direction + 
            _possibleLetters.Length ) % _possibleLetters.Length;

        LoadLetters();
    }

    public void OnButtonHorizontal(int direction){
        stringIndex = (
            stringIndex + 
            direction +
            _letterIndexes.Length) % _letterIndexes.Length;

        SetupMarkers();
    }


}
