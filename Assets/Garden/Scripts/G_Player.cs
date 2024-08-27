using UnityEngine;
using UnityEngine.UI;

public class G_Player : CUpdateMonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] GameObject[] _playerPositions;
    [SerializeField] Transform[] _shootingPositions;
    [SerializeField] G_PlantInstance[] _planties;
    [SerializeField] G_WaterMissle _missle;
    [SerializeField] Sprite _image;
    [SerializeField] PlayerIndex _index;
    [SerializeField] G_KeyLocker _locker;

    private int _position = 0;
    private bool _blockMovement = false;

    public static bool InverseMovement = false;

    public static int[] _takenPosition = new int[2] { -1, -1};

    protected override void Awake() {
        for(int i = 0; i < _playerPositions.Length; i++) _playerPositions[i].GetComponent<Image>().sprite = _image;
        MoveToPosition();

        InverseMovement = false;
        Events.Gameplay.RegisterListener(this, GameplayEventType.GenerateKeys);
        Events.Gameplay.RegisterListener(this, GameplayEventType.EndGenerateKeys);
    }

    private void MoveToPosition(){
        for(int i = 0; i < _playerPositions.Length; i++) _playerPositions[i].SetActive(false);
        _playerPositions[_position].SetActive(true);
    }

    private int GetPosition(int vertical, int horizontal){
        switch(vertical){
            case -1: 
                switch(horizontal){
                    case -1: return 5;
                    case  0: return 4;
                    case  1: return 3;
                }
            break;
            case  0: 
                switch(horizontal){
                    case -1: return 6;
                    case  0: return _position;
                    case  1: return 2;
                }
            break;
            case  1: 
                switch(horizontal){
                    case -1: return 7;
                    case  0: return 0;
                    case  1: return 1;
                }
            break;
        }

        return _position; 
    }

    private int convertToFull(float val){
        if(Mathf.Abs(val) < 0.2f) return 0;
        return (int)Mathf.Sign(val);
    }

    float _moveDelay;
    float _shotDelay;

    private void ProcessMovement(){
        _moveDelay -= Time.deltaTime;
        _shotDelay -= Time.deltaTime;

        float vertical   = InputHandler.GetVertical(_index);
        float horizontal = InputHandler.GetHorizontal(_index);

        if(InverseMovement){ vertical = -vertical; horizontal = -horizontal;}

        if(_moveDelay < 0 && Mathf.Abs(horizontal) > 0.2f){

            

            _takenPosition[(int)_index] = -1;
            _position = (_position + (int)Mathf.Sign(horizontal) + _playerPositions.Length) % _playerPositions.Length;
            for(int i = 0; i < _takenPosition.Length; i++){
                if(_position == _takenPosition[i]) 
                    _position = (_position + (int)Mathf.Sign(horizontal) + _playerPositions.Length) % _playerPositions.Length;
            }
            _takenPosition[(int)_index] = _position;

            //GetPosition(convertToFull(vertical), convertToFull(horizontal));
            MoveToPosition();
            _moveDelay = 0.2f;
        }
        
        bool shot = false;
        switch(_index){
            case PlayerIndex.Player1: shot = InputHandler.GetKey(InputKey.Action_Any_Player1); break;
            case PlayerIndex.Player2: shot = InputHandler.GetKey(InputKey.Action_Any_Player2); break;
        }

        if(shot && _shotDelay < 0) {
            SpawnMissle();
             _moveDelay = 0.1f;
        }
    }

    private void SpawnMissle(){
        G_WaterMissle missle = Instantiate(_missle,
            _shootingPositions[_position].position,
            Quaternion.identity).GetComponent<G_WaterMissle>();
        missle.Setup(_planties[_position], _index);
        (missle.transform as RectTransform).SetParent(transform.parent);
    }

    public override void CUpdate()
    {
        base.CUpdate();

        if(!_blockMovement){
            ProcessMovement();
        }else{
            _locker.ReadInput(_index);
            _blockMovement = _locker.IsLocked();
        }
    }

    public void OnGameEvent(GameplayEvent gameEvent)
    {
        if(gameEvent.type == GameplayEventType.GenerateKeys){
            _blockMovement = true;
            if(gameObject.activeSelf) _locker.LockPlayer();
        }
        if(gameEvent.type == GameplayEventType.EndGenerateKeys){
            _blockMovement = false;
            if(gameObject.activeSelf) _locker.Hide();
        }
    }
}
