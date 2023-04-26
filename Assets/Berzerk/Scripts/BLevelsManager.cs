using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Must align with inspector
public enum BExitIndex{
    Left,
    Right,
    Top,
    Bottom,
    None_Max,
}

public class BLevelsManager : MonoBehaviour, IListenToGameplayEvents
{
    public static int CurrentLevel = 1;
    public static float Timer = 0;
    public static float TimerAddtional = 0;
    public static int Points = 0;

    private static Dictionary<BExitIndex, BExitIndex> _reverse = new Dictionary<BExitIndex, BExitIndex>{
        {BExitIndex.Bottom, BExitIndex.Top},
        {BExitIndex.Left,   BExitIndex.Right},
        {BExitIndex.Right,  BExitIndex.Left},
        {BExitIndex.Top,    BExitIndex.Bottom},
    };

    private static BLevelsManager _instance;
    private int _lives = 2;

    [SerializeField] private GameObject[] _exits;
    [SerializeField] private Transform[] _playerStaringPoints;
    [SerializeField] private Transform[] _followerStaringPoints;
    [SerializeField] private BEnemySpawnerManager _enemies;
    [SerializeField] private BLevel _level;
    [SerializeField] private BFollower _follower;
    [SerializeField] private GameObject[] _playerLives;
    [SerializeField] private Image _background;
    [SerializeField] private Sprite[] _backgrounds;
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private SceneLoader _outro;

    private BExitIndex _followerStart = BExitIndex.Left;

    private int _backgroundID = -1;

    public void OnGameEvent(GameplayEvent gEvent){
        if(gEvent.type == GameplayEventType.SpawnFollower){
            _follower.transform.position = _followerStaringPoints[(int)_followerStart].transform.position;
            _follower.SetActive(true);
        }
    }

    private void Awake() {
        _instance = this;
        CurrentLevel = 1;
        PointsCounter.Score = 0;

        Points = 0;
        HighScoreRanking.LoadRanking(GameType.Berzerk);

        LockExit(BExitIndex.None_Max);
        _follower.SetActive(false);
        Events.Gameplay.RegisterListener(this, GameplayEventType.SpawnFollower);
        UpdateLives();
    }

    private void LockExit(BExitIndex index){
        for(int i = 0; i < _exits.Length; i++) {
            _exits[i].SetActive(i == (int)index);
        }
    }

    private void Update() {
        Timer += Time.deltaTime;
        _score.text = Points.ToString();
    }

    public static void PlayerDied(){
        if(Guard.IsValid(_instance)) _instance.PlayerDiedInternal();
    }

    private void PlayerDiedInternal(){
        if(_lives >= 0){
            ChangeLevelInternal(BExitIndex.Left);
            _lives --;
            UpdateLives();
            return;
        }

        PointsCounter.Score = Points;
        HighScoreRanking.TryAddNewRecord(Points);
        _outro.OnSceneLoad();
    }

    private void UpdateLives(){
        for(int i = 0; i < _playerLives.Length; i++){
            _playerLives[i].SetActive(i <= _lives);
        }
    }

    private void ChangeLevelInternal(BExitIndex walkedBy){
        CurrentLevel++;
        TimerAddtional = Mathf.Min(CurrentLevel/2.0f, 5f); 
        Timer = TimerAddtional;
        _level.SelectRandomLevel();

        LockExit(_reverse[walkedBy]);
        Berzerk.Instance.transform.position = _playerStaringPoints[(int)_reverse[walkedBy]].transform.position;
        _followerStart = _reverse[walkedBy];
        _follower.SetActive(false);

        int bakcgrounId = Random.Range(0, _backgrounds.Length);
        while(bakcgrounId == _backgroundID) bakcgrounId = Random.Range(0, _backgrounds.Length);
        _background.sprite = _backgrounds[bakcgrounId];
        _backgroundID = bakcgrounId;

        _enemies.SpawnEnemies();
        BGeneralBoxController.Instance.Setup();
    }

    public static void ChangeLevel(BExitIndex walkedBy){
        if(Guard.IsValid(_instance)) _instance.ChangeLevelInternal(walkedBy);
    }
}
