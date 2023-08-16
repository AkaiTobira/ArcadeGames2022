using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreLabelInput : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] int _labelIndex;
    [SerializeField] TextMeshProUGUI _score;

    [SerializeField] TextMeshProUGUI _scoreLabel;
    [SerializeField] TextMeshProUGUI _timeLabel;


    [SerializeField] GameObject _NewHighScore;
    [SerializeField] GameObject _NoNewHighScore;
    [SerializeField] GameObject _windows;
    [SerializeField] GameObject _horizontalArrows;
    [SerializeField] GameObject[] _markers;
    [SerializeField] TextMeshProUGUI[] _letters;
    [SerializeField] GameType _gameType;

    [SerializeField] bool _ignoreSetup;

    int[] _letterIndexes = {1,1,1,0,0,0,0,0,0,0};

    char[] _possibleLetters = {
        ' ','A','B','C','D','E','F','G','H','I','J',
        'K','L','M','N','O','P','Q','R','S','T','U',
        'V','W','X','Y','Z','1','2','3','4','5','6',
        '7','8','9','0'
    };

    private void Awake() {
        HighScoreRanking.LoadRanking(_gameType);
        SetPoints();

    }

    private void Start() {
        SetPoints();

        if(_ignoreSetup) {
            //HighScoreRanking.LoadRanking(_gameType);
            SetPoints();
            return;
        }

        Events.Gameplay.RegisterListener(this, GameplayEventType.GameOver);
        Events.Gameplay.RegisterListener(this, GameplayEventType.RefreshRanking);
        Events.Gameplay.RegisterListener(this, GameplayEventType.SaveRankings);

        _windows.SetActive(true);
        _horizontalArrows.SetActive(true);

        
        _labelIndex = HighScoreRanking.HasNewRecord();
        if(_labelIndex == -1) {

            _windows.SetActive(false);
            _horizontalArrows.SetActive(false);
            _NoNewHighScore.SetActive(true);
            _NewHighScore.SetActive(false);

            return;
        }

        HandleWindowsInput();
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){
        if(enabled == false || _ignoreSetup){
            
        //    Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.RefreshRanking));
            return;
        }

        if(gameplayEvent.type == GameplayEventType.SaveRankings){
            if(_labelIndex != -1){
                string fullName = "";

                for(int i = 0; i < _letterIndexes.Length; i++){
                    fullName += _possibleLetters[_letterIndexes[i]].ToString();
                }

                HighScoreRanking.SaveName(_labelIndex, fullName);
                HighScoreRanking.SaveRanking();
            }

            enabled = false;
            SetPoints();
            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.RefreshRanking));
        }

        if( gameplayEvent.type == GameplayEventType.GameOver ||
            gameplayEvent.type == GameplayEventType.RefreshRanking ){

            _labelIndex = HighScoreRanking.HasNewRecord();
            if(_labelIndex == -1) return;

            SetPoints();
            HandleWindowsInput();
        }
    }

    int stringIndex = 0;


//    #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

    float _elapsedTime1 = 0;
    float _elapsedTime2 = 0;


    private void Update() {
        if(_ignoreSetup) return;

        _elapsedTime1 -= Time.deltaTime;
        _elapsedTime2 -= Time.deltaTime;
        
        float verticalChange   = Input.GetAxisRaw("Vertical");
        float horizontalChange = Input.GetAxisRaw("Horizontal");

        if(_elapsedTime1 < 0 && Mathf.Abs(verticalChange) > 0.3f){
            _elapsedTime1 = 0.2f;
            OnButtonVertical((int) Mathf.Sign(verticalChange));
        }

        if(_elapsedTime2 < 0 && Mathf.Abs(horizontalChange) > 0.3f){
            _elapsedTime2 = 0.2f;
            OnButtonHorizontal((int) Mathf.Sign(horizontalChange));
        }
    }
//    #endif


    private void SetPoints(){
// /        Debug.LogWarning("SetPoints");
        bool IsTimed =  HighScoreRanking.IsTimed(_gameType);

        int points = IsTimed ? TimerCount.ElapsedTime : PointsCounter.Score;
        if(Guard.IsValid(_score)){
            _score.text = IsTimed ? Fromater.FormatToTime(points) : Fromater.FormatToPoints(points);
        }
        if(Guard.IsValid(_scoreLabel)) _scoreLabel.gameObject.SetActive(!IsTimed);
        if(Guard.IsValid(_timeLabel)) _timeLabel.gameObject.SetActive(IsTimed);
    }

    private void HandleWindowsInput(){
        _windows.SetActive(true);
        _horizontalArrows.SetActive(true);

        SetPoints();

        ResetLetters();
        LoadLetters();
        SetupMarkers();
    }

    private void ResetLetters(){
        _letterIndexes[0] = 1;
        _letterIndexes[1] = 1;
        _letterIndexes[2] = 1;

        _letterIndexes[3] = 0;
        _letterIndexes[4] = 0;
        _letterIndexes[5] = 0;
        _letterIndexes[6] = 0;
        _letterIndexes[7] = 0;
        _letterIndexes[8] = 0;
        _letterIndexes[9] = 0;
    }

    private void SetupMarkers(){
        for(int i = 0; i < _letterIndexes.Length; i++){
            _markers[i].SetActive(stringIndex == i);
        }
    }

    private void LoadLetters(){
        for(int i = 0; i < _letterIndexes.Length; i++){
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
