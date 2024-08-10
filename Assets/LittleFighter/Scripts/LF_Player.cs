using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Xml.Linq;


public enum LFPlayerState{
    Idle,
    Move,
    Kick,
    Punch,
    Defend,
    SuperAttack,
    Hurt,
    Dead,
}

public static class LF_PlayerInput{

    private static List<(InputKey, long)> _scanned = new List<(InputKey, long)>();
    private static List<InputKey> _keys = new List<InputKey>{
        InputKey.ActionX,
        InputKey.ActionZ,
        InputKey.ActionV,
        InputKey.ActionB,
    };

    private static long GetCurrentTime(){
        System.DateTime val = System.DateTime.Now;
        
        long current = val.Millisecond;
        current += val.Second * 1000;
        current += val.Minute * 60000;
        current += val.Hour   * 3600000;
        return current;
    }


    public static void Scan(){
        long current = GetCurrentTime();
        for(int i = 0; i < _keys.Count; i++) {
            bool found = false;
            for(int j = 0; j < _scanned.Count; j++) {
                if(_scanned[j].Item1 == _keys[i]) {
                    found = true;
                    break;
                }
            }
            if(found) continue;
            if(InputHandler.GetKey(_keys[i], false)) _scanned.Add((_keys[i], current));
        }

//        string ss = "";
//        foreach((KeyCode, int) sca in _scanned){
//            ss += sca.Item1.ToString() + " " + sca.Item2 + " " + (current - sca.Item2);
//        }
//        Debug.Log(ss);

        ReomveUnused();
    }


    private static void ReomveUnused(){
        long current = GetCurrentTime();
        List<InputKey> toRemove = new List<InputKey>();

        for( int j = 0; j < _scanned.Count; j++){
            if(current - _scanned[j].Item2 > 1000){
                toRemove.Add(_scanned[j].Item1);
            }
        }

        for(int i = 0; i < toRemove.Count; i++) {
            RemoveFromScanned(toRemove[i]);
        }
    }

    private static void RemoveFromScanned(InputKey code){
        for(int i = 0; i < _scanned.Count; i++){
            if(_scanned[i].Item1 == code){
                _scanned.RemoveAt(i);
                break;
            }
        }
    }

    public static bool GetPressed(InputKey active){
        long current = GetCurrentTime();
        int pressed = 0;

        List<InputKey> toRemove = new List<InputKey>();


        for( int j = 0; j < _scanned.Count; j++){
            if(_scanned[j].Item1 == active && (current - _scanned[j].Item2) > 100){
                toRemove.Add(active);
                pressed++;
            }
        }

        if(pressed == 1){
            for(int i = 0; i < toRemove.Count; i++) {
                RemoveFromScanned(toRemove[i]);
            }
            return true;
        }

        return false;
    }


    public static bool GetPressed(InputKey[] active){
        long current = GetCurrentTime();
        int pressed = 0;

        List<InputKey> toRemove = new List<InputKey>();


        for(int i = 0; i < active.Length; i++) {
            for( int j = 0; j < _scanned.Count; j++){
                if(_scanned[j].Item1 == active[i]){
                    toRemove.Add(active[i]);
                    pressed++;
                }
            }
        }

        if(pressed == active.Length){
            for(int i = 0; i < toRemove.Count; i++) {
                RemoveFromScanned(toRemove[i]);
            }
            return true;
        }

        return false;
    }
}

public class LF_Player : ESM.SMC_2D<LFPlayerState>,
    ITakeDamage,
    IDealDamage,
    IUserControlled
{

    private const float VERTICAL_SLOW = 0.5f;

    [SerializeField] private LF_PlayerHPBar _PlayerHPBar;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private SceneLoader _endScene;
    [SerializeField] private PlayerIndex _playerIndex;
    [SerializeField] public bool IsAwake;

    enum RayPoints{
        Right,
        Left,
        Bottom,
        Top,
        RightBottom,
        RightUp,
        LeftUp,
        LeftBottom,
    }

    private float _startDelay = 8f;

    class HitParameters{
        public RayPoints[] RelatedDirections;
        public Vector2     Direction;
        public LayerMask   Layer;
        public string[]    ColliderName;
    }

    Dictionary<RayPoints, HitParameters> _hitParameters = new Dictionary<RayPoints, HitParameters>{
        {RayPoints.Left, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Right, RayPoints.RightUp, RayPoints.RightBottom},
            Direction = new Vector2(-1, 0),
            Layer = 64,
            ColliderName = new string[]{"Player_Obstacle","Enviroment_Obstacle"}}},
        {RayPoints.Right, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Right, RayPoints.RightUp, RayPoints.RightBottom},
            Direction = new Vector2(1, 0),
            Layer = 64,
            ColliderName = new string[]{"Player_Obstacle","Enviroment_Obstacle"}}},
        {RayPoints.Top, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Top, RayPoints.LeftUp, RayPoints.RightUp},
            Direction = new Vector2(0, 1),
            Layer = 64,
            ColliderName = new string[]{"Player_Obstacle","Enviroment_Obstacle"}}},
        {RayPoints.Bottom, new HitParameters{ 
            RelatedDirections = new  RayPoints[]{RayPoints.Bottom, RayPoints.LeftBottom, RayPoints.RightBottom},
            Direction = new Vector2(0, -1),
            Layer = 64,
            ColliderName = new string[]{"Player_Obstacle","Enviroment_Obstacle"}}},
    };



    [SerializeField] private float _MaxHealthPoints = 30f;
    [SerializeField] private LayerMask _fieldOfView;
    [SerializeField] private string _obstacleTag;
    [SerializeField] private GameObject[] _points;

    [SerializeField] private BoxCollider2D _attackPunchBox;
    [SerializeField] private BoxCollider2D _attackKickBox;
    [SerializeField] private BoxCollider2D _attackSpecialBox;
    [SerializeField] private BoxCollider2D _hitBox;

    private float _healthPoints = 30f;
    public bool IsDead(){ return _healthPoints <= 0;}
    private bool HasBeenHurt(){ return _ishurt;}

    private Vector2 _inputs = new Vector2();
    private bool _canPunch = false;
    private bool _canKick = false;
    private bool _canSpecial = false;
    private bool _ishurt;

    const float ATTACK_PUNCH_TIME = 0.45f;
    const float ATTACK_KICK_TIME  = 0.55f;
    const float ATTACK_SPECIAL_TIME  = 0.7f;
    const float HURT_TIME = 0.2f;


    private float _attackPunchTime = ATTACK_PUNCH_TIME;
    private float _attackKickTime  = ATTACK_KICK_TIME;
    private float _attackSpecialTime = ATTACK_SPECIAL_TIME;
    private float _hurtTime = HURT_TIME;

    const string SOUND_PLAYER_PUNCH = "LittleFighter_PlayerPunch";
    const string SOUND_PLAYER_HURT1 = "LittleFighter_PlayerHurt1";
    const string SOUND_PLAYER_HURT2 = "LittleFighter_PlayerHurt2";
    const string SOUND_PLAYER_KICK = "LittleFighter_PlayerKick";
    const string SOUND_PLAYER_SPECIAL = "LittleFighter_PlayerSpecial";
    const string SOUND_PLAYER_HEAL = "LittleFighter_Heal";

    protected override void Awake() {
        base.Awake();
        ForceState(LFPlayerState.Idle, true);
        _healthPoints = _MaxHealthPoints;
        _PlayerHPBar.SetupHp(_healthPoints/_MaxHealthPoints);
        PointsCounter.Reset();

        PlayerList<LF_Player>.Register(_playerIndex, this);
    }

    public PlayerIndex GetPlayerIndex() { return _playerIndex; }

    protected override void UpdateState()
    {
        switch(ActiveState){
            case LFPlayerState.Idle: break;
            case LFPlayerState.Move: ProcessMove(_inputs); break;
            case LFPlayerState.Kick: 
                _attackKickTime -= Time.deltaTime;
                if(_attackKickTime < ATTACK_KICK_TIME/2.0f) _attackKickBox.gameObject.SetActive(true);
            break;
            case LFPlayerState.Punch: 
                _attackPunchTime -= Time.deltaTime;
                if(_attackPunchTime < ATTACK_PUNCH_TIME/2.0f) _attackPunchBox.gameObject.SetActive(true);
            break;
            case LFPlayerState.SuperAttack:
                _attackSpecialTime -= Time.deltaTime;
                if(_attackSpecialTime < ATTACK_PUNCH_TIME/2.0f) _attackSpecialBox.gameObject.SetActive(true);
            break;
            case LFPlayerState.Defend: break;
            case LFPlayerState.Hurt: 
                _hurtTime -= Time.deltaTime;
            break;
            case LFPlayerState.Dead: break;
        }

        ProcessInputs();

    }

    protected override void OnStateEnter(LFPlayerState enteredState)
    {
        switch(ActiveState){
            case LFPlayerState.Idle:
                _attackSpecialBox.gameObject.SetActive(false);
                _attackPunchBox.gameObject.SetActive(false);
                _attackKickBox.gameObject.SetActive(false);
                _hitBox.gameObject.SetActive(true);

            break;
            case LFPlayerState.Move: break;
            case LFPlayerState.Kick: 
            AudioSystem.PlaySample(SOUND_PLAYER_KICK, 1, true);
                _attackKickTime = ATTACK_KICK_TIME;
                break;
            case LFPlayerState.Punch: 
                AudioSystem.PlaySample(SOUND_PLAYER_PUNCH, 1, true);
                _attackPunchTime = ATTACK_PUNCH_TIME;
                break;
            case LFPlayerState.SuperAttack:
                AudioSystem.PlaySample(SOUND_PLAYER_SPECIAL, 1, true);
                _attackSpecialTime = ATTACK_SPECIAL_TIME;
                _healthPoints -= 2;
                _PlayerHPBar.SetupHp(_healthPoints/_MaxHealthPoints);
                break;
            case LFPlayerState.Defend: break;
            case LFPlayerState.Hurt: 
                _hurtTime = HURT_TIME;
                _hitBox.gameObject.SetActive(false);
            break;
            case LFPlayerState.Dead: 
                RequestDisable(1f);
            //    _gameOverText.gameObject.SetActive(true);
            //    _PlayerHPBar.gameObject.SetActive(false);

                CameraFollow.Instance.RemoveFollowable( transform );

                if(HasOtherPlayerActive()){
                    LF_IntroTexts.ShowGameOver();
                    AudioSystem.PlaySample("LittleFighter_GameOver", 1);
                    HighScoreRanking.LoadRanking(GameType.LittleFighter);
                    TimersManager.Instance.FireAfter(5, () => {
                        _endScene.OnSceneLoadAsync();
                    });
                }
            break;
        }
    }

    private bool HasOtherPlayerActive(){
        for(int i = 0; i < (int)PlayerIndex.None; i++){
            LF_Player player = PlayerList<LF_Player>.Get((PlayerIndex)i);
            if(!Guard.IsValid(player)) continue;
            if(!player.IsDead() && player.IsAwake) return false;
        }

        return true;
    }

    protected override void OnStateExit(LFPlayerState exitedState)
    {
        switch(ActiveState){
            case LFPlayerState.Idle: break;
            case LFPlayerState.Move: break;
            case LFPlayerState.Kick: break;
            case LFPlayerState.Punch: break;
            case LFPlayerState.Defend: break;
            case LFPlayerState.Hurt: 
                _ishurt = false;
            break;
            case LFPlayerState.Dead: break;
        }
    }

    protected override LFPlayerState CheckStateTransitions()
    {
        switch(ActiveState){
            case LFPlayerState.Idle:
                if(IsDead()) return LFPlayerState.Dead;
                else if(HasBeenHurt()) return LFPlayerState.Hurt;
                else if(_canSpecial) return LFPlayerState.SuperAttack;
                else if(_canPunch) return LFPlayerState.Punch;
                else if(_canKick) return LFPlayerState.Kick;
                else if(_inputs.magnitude > 0) return LFPlayerState.Move;
            break;
            case LFPlayerState.Move: 
                if(IsDead()) return LFPlayerState.Dead;
                else if(HasBeenHurt()) return LFPlayerState.Hurt;
                else if(_canSpecial) return LFPlayerState.SuperAttack;
                else if(_canPunch) return LFPlayerState.Punch;
                else if(_canKick) return LFPlayerState.Kick;
                else if(_inputs.magnitude <= 0.1f) return LFPlayerState.Idle;
            break;
            case LFPlayerState.Kick: 
                if(IsDead()) return LFPlayerState.Dead;
                else if(HasBeenHurt()) return LFPlayerState.Hurt;
                else if(_attackKickTime < 0) return LFPlayerState.Idle;
            break;
            case LFPlayerState.Punch: 
                if(IsDead()) return LFPlayerState.Dead;
                else if(HasBeenHurt()) return LFPlayerState.Hurt;
                else if(_attackPunchTime < 0) return LFPlayerState.Idle;
            break;
            case LFPlayerState.SuperAttack:
                if(IsDead()) return LFPlayerState.Dead;
                else if(HasBeenHurt()) return LFPlayerState.Hurt;
                else if(_attackSpecialTime < 0) return LFPlayerState.Idle;
            break;
            case LFPlayerState.Defend: break;
            case LFPlayerState.Hurt: 
                if(IsDead()) return LFPlayerState.Dead;
                else if(_hurtTime < 0) return LFPlayerState.Idle;
            break;
            case LFPlayerState.Dead: break;
        }

        return ActiveState;
    }

    public void TakeDamage(float amount, MonoBehaviour source = null){

        if( !_ishurt 
            && (ActiveState != LFPlayerState.Hurt
            ||  ActiveState != LFPlayerState.Dead)
            ){
            _healthPoints -= amount;
            if(amount > 0) _ishurt = true;
            
            
            AudioSystem.PlaySample(
                (amount < 0) ?
                    SOUND_PLAYER_HEAL :
                    ((UnityEngine.Random.Range(0,2) == 0) ?
                        SOUND_PLAYER_HURT1 :
                        SOUND_PLAYER_HURT2
                    )
                , 1, true);


            _healthPoints = Math.Max( Mathf.Min(_healthPoints, _MaxHealthPoints), -1);

            _PlayerHPBar.SetupHp(_healthPoints/_MaxHealthPoints);
        }
    }

    public float GetDamage(){
        switch(ActiveState){
            case LFPlayerState.Punch: return 1;
            case LFPlayerState.SuperAttack: return 5;
            case LFPlayerState.Kick: return 2;
        }

        return 0;
    }

    private void ProcessInputs(){

        _startDelay -= Time.deltaTime;
        if(_startDelay > 0) return;


        ProcessInputsMove();
        ProcessInputsAttack();
    }

    private void ProcessInputsMove(){
        _inputs.x = InputHandler.GetHorizontal(_playerIndex); //+ _mobileInputs.x;
        _inputs.y = InputHandler.GetVertical(_playerIndex)  * VERTICAL_SLOW; //+ _mobileInputs.y;

        bool ResetX = false;

        if(GetFacingDirection() == ESM.AnimationSide.Right){
            if(_inputs.x > 0) ResetX = CheckHit(RayPoints.Right, 1);}

        if(GetFacingDirection() == ESM.AnimationSide.Left){
            if(_inputs.x < 0) ResetX = CheckHit(RayPoints.Left, 1);}

        if(ResetX) _inputs.x = 0;

        bool ResetY = false;
        if(_inputs.y > 0) ResetY = CheckHit(RayPoints.Top,    0.3f);
        if(_inputs.y < 0) ResetY = CheckHit(RayPoints.Bottom, 0.3f);
        if(ResetY) _inputs.y = 0;
    }

    private bool CheckHit(RayPoints point, float distance){
        bool returnValue = false;
        HitParameters parameters = _hitParameters[point];
        for(int i = 0; i < parameters.RelatedDirections.Length; i++) {
            returnValue |= IsHit(
                parameters.RelatedDirections[i], 
                parameters.Direction,
                parameters.Layer, 
                parameters.ColliderName,
                distance) != null;
        }

        return returnValue;
    }

    private void ProcessInputsAttack(){
        LF_PlayerInput.Scan();

        if(_playerIndex == PlayerIndex.Player1){
            _canSpecial = LF_PlayerInput.GetPressed(new InputKey[] { InputKey.ActionX, InputKey.ActionZ });
            _canPunch   = LF_PlayerInput.GetPressed(InputKey.ActionX);
            _canKick    = LF_PlayerInput.GetPressed(InputKey.ActionZ);
        }
        else if(_playerIndex == PlayerIndex.Player2){
            _canSpecial = LF_PlayerInput.GetPressed(new InputKey[] { InputKey.ActionV, InputKey.ActionB });
            _canPunch   = LF_PlayerInput.GetPressed(InputKey.ActionV);
            _canKick    = LF_PlayerInput.GetPressed(InputKey.ActionB);    
        }
    }

    private GameObject IsHit(
        RayPoints point, 
        Vector2 direction, 
        LayerMask layer,
        string[] colliderName,
        float distance = 1){

        RaycastHit2D hit1 = Physics2D.Raycast(
            _points[(int)point].transform.position, direction, distance, layer);
        Debug.DrawLine(
            (Vector2)_points[(int)point].transform.position,
            (Vector2)_points[(int)point].transform.position + (direction * distance),
            Color.red
        );

        if(hit1){
            for(int i = 0; i < colliderName.Length; i++) {
                GameObject t1 = CUtils.FindObjectByName(hit1.transform.gameObject, ref colliderName[i]);
                if(Guard.IsValid(t1) ) return t1;
            }
        }

        return null;
    }


    protected override void ProcessMove(Vector2 directions)
    {
        base.ProcessMove(directions);
    }
}
