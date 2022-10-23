using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreLabelInput : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] int _labelIndex;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _score;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] TextMeshProUGUI _preview;

    [SerializeField] GameObject _NewHighScore;
    [SerializeField] GameObject _NoNewHighScore;
    [SerializeField] GameObject _windows;
    [SerializeField] GameObject _horizontalArrows;
    [SerializeField] GameObject[] _markers;
    [SerializeField] TextMeshProUGUI[] _letters;
    [SerializeField] GameType _gameType;

    int[] _letterIndexes = {1,1,1,0,0,0,0,0,0,0};

    char[] _possibleLetters = {
        ' ','A','B','C','D','E','F','G','H','I','J',
        'K','L','M','N','O','P','Q','R','S','T','U',
        'V','W','X','Y','Z','1','2','3','4','5','6',
        '7','8','9','0'
    };

    private void Awake() {
        HighScoreRanking.LoadRanking(_gameType);
    }

    private void Start() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.GameOver);
        Events.Gameplay.RegisterListener(this, GameplayEventType.RefreshRanking);
        Events.Gameplay.RegisterListener(this, GameplayEventType.SaveRankings);

        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

        _inputField.gameObject.SetActive(false);
        _name.gameObject.SetActive(false);
        _windows.SetActive(true);
        _horizontalArrows.SetActive(true);

        #endif

        _labelIndex = HighScoreRanking.HasNewRecord();
        if(_labelIndex == -1) {

            _windows.SetActive(false);
            _horizontalArrows.SetActive(false);
            _inputField.gameObject.SetActive(false);
            _NoNewHighScore.SetActive(true);
            _NewHighScore.SetActive(false);

            return;
        }

        HandleWindowsInput();

        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            HandleWindowsInput();
        #else
            HandleMobileInput();
        #endif


    }

    public void OnGameEvent(GameplayEvent gameplayEvent){
        if(enabled == false) return;

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
            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.RefreshRanking));
        }

        if( gameplayEvent.type == GameplayEventType.GameOver ||
            gameplayEvent.type == GameplayEventType.RefreshRanking ){

            _labelIndex = HighScoreRanking.HasNewRecord();
            if(_labelIndex == -1) return;

        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            HandleWindowsInput();
        #else
            HandleMobileInput();
        #endif

        }
    }

    #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

    int stringIndex = 0;

    string _playerName;

    float _elapsedTime1 = 0;
    float _elapsedTime2 = 0;


    private void Update() {
        _elapsedTime1 -= Time.deltaTime;
        _elapsedTime2 -= Time.deltaTime;
        
        float verticalChange   = Input.GetAxisRaw("Vertical");
        float horizontalChange = Input.GetAxisRaw("Horizontal");

        if(_elapsedTime1 < 0 && Mathf.Abs(verticalChange) > 0.3f){
            _elapsedTime1 = 0.2f;

            _letterIndexes[stringIndex] = (
                _letterIndexes[stringIndex] + 
                (int) Mathf.Sign(verticalChange) + 
                _possibleLetters.Length ) % _possibleLetters.Length;

            LoadLetters();
        }

        if(_elapsedTime2 < 0 && Mathf.Abs(horizontalChange) > 0.3f){
            _elapsedTime2 = 0.2f;

            stringIndex = (
                stringIndex + 
                (int) Mathf.Sign(horizontalChange) +
                _letterIndexes.Length) % _letterIndexes.Length;
            SetupMarkers();
        }
    }
    #endif

    private void HandleMobileInput(){

        int points = (_gameType == GameType.DigDug) ? TimerCount.ElapsedTime : PointsCounter.Score;
        _score.text = (_gameType == GameType.DigDug) ? FormatScoreTime(points) : FormatScorePoints(points);

        _inputField.enabled = true;
        _windows.SetActive(false);
        _horizontalArrows.SetActive(false);
    }

    private void HandleWindowsInput(){
        _inputField.enabled = false;
        _windows.SetActive(true);
        _horizontalArrows.SetActive(true);
        
        int points = (_gameType == GameType.DigDug) ? TimerCount.ElapsedTime : PointsCounter.Score;
        _score.text = (_gameType == GameType.DigDug) ? FormatScoreTime(points) : FormatScorePoints(points);


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

    public void OnTextEnter(string s){}

    public void OnEditEnd(string s){
        _inputField.enabled = false;
        _name.text = s;

        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

        #else
            HighScoreRanking.SaveName(_labelIndex, s);
            HighScoreRanking.SaveRanking();
        #endif
    }

    protected string FormatScoreTime(int score){
        string time = (score / 3600).ToString().PadLeft(2, '0') + ":";
        time += ((score % 3600)  / 60).ToString().PadLeft(2, '0') + ":";
        time += (score % 60).ToString().PadLeft(2, '0');
        return time;
    }

    protected string FormatScorePoints(int score){
        return score.ToString().PadLeft(8, '0');
    }
}
