using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CButtonSelector : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] CButton[] _buttons;
    [SerializeField] int _numberInRow = 1;
    [SerializeField] bool _isVertical = true;
    [SerializeField] bool _isHorizontal = true;
    [SerializeField] bool _forceActivationOnStart = false;

    int _activeButton;
    float _reReadTime = 0.2f;
    float _elapsedTime = 0;
    float _elapsedTime1 = 0;

    enum EDirection{
        None,
        Vertical,
        Horizontal
    }

    EDirection _directon = EDirection.None;

    public void OnGameEvent(GameplayEvent gameplayEvent){

        if(gameplayEvent.type == GameplayEventType.ButtonOvervieved){
            CButton button = gameplayEvent.parameter as CButton;
            if(Guard.IsValid(button)){

                int currentActiveButton = _activeButton;

                for(int i = 0; i < _buttons.Length; i++) {
                    if(button == _buttons[i]) _activeButton = i;
                }

//                Debug.Log(currentActiveButton + " " + _activeButton);

                if(currentActiveButton != _activeButton) AudioSystem.Instance.PlayEffect("ButtonChange", 1);
            }
        }
    }

    private void RemoveInactiveButtons(){
        List<CButton> activeButtons = new List<CButton>();
        for(int i = 0; i < _buttons.Length; i++) {
            if(_buttons[i].gameObject.activeSelf) activeButtons.Add(_buttons[i]);
        }
        _buttons = activeButtons.ToArray();
    }

    void Start()
    {
        _activeButton = 0;
        Events.Gameplay.RegisterListener(this, GameplayEventType.ButtonOvervieved);
        
        RemoveInactiveButtons();
        if(_forceActivationOnStart) TimersManager.Instance.FireAfter( 0.3f, Enable );
    }

    public void Enable(){
        if(Guard.IsValid(this)){
            Events.Gameplay.RiseEvent(
                new GameplayEvent(
                    GameplayEventType.ButtonOvervieved, 
                    _buttons[_activeButton]));
        }
    }

    private void ProcessTransverseMove(){
        if(_buttons.Length == 1) {
            if(Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("Horizontal") != 0){
                Events.Gameplay.RiseEvent(
                    new GameplayEvent(
                        GameplayEventType.ButtonOvervieved, 
                        _buttons[0]));
            }
            return;
        }

        float verticalChange = Input.GetAxisRaw("Vertical");
        if(_elapsedTime <= 0 && Mathf.Abs(verticalChange) > 0.3f && _isVertical){
            _elapsedTime = _reReadTime;

            int currentActiveButton = _activeButton;

            _activeButton = (
                _activeButton - 
                ((int)Mathf.Sign(verticalChange) * _numberInRow) + _buttons.Length)%(_buttons.Length);
            
            Events.Gameplay.RiseEvent(
                new GameplayEvent(
                    GameplayEventType.ButtonOvervieved, 
                    _buttons[_activeButton]));


            if(currentActiveButton != _activeButton) AudioSystem.Instance.PlayEffect("ButtonChange", 1);
            return;
        }

        float horizontalChange = Input.GetAxisRaw("Horizontal");
        if(_elapsedTime <= 0 && Mathf.Abs(horizontalChange) > 0.3f && _isHorizontal){
            _elapsedTime = _reReadTime;

            int currentActiveButton = _activeButton;

            _activeButton = (
                _activeButton + 
                ((int)Mathf.Sign(horizontalChange))+ _buttons.Length)%(_buttons.Length);


            Events.Gameplay.RiseEvent(
                new GameplayEvent(
                    GameplayEventType.ButtonOvervieved, 
                    _buttons[_activeButton]));

            if(currentActiveButton != _activeButton) AudioSystem.Instance.PlayEffect("ButtonChange", 1);
        }
    }

    void Update()
    {
        _elapsedTime  -= Time.deltaTime;
        _elapsedTime1 -= Time.deltaTime;
        ProcessTransverseMove();

        if(_elapsedTime1 > 0) return;
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
//            Debug.Log(_buttons[_activeButton].name + " PointDown");
            _buttons[_activeButton].OnPointerDown(null);
            _elapsedTime1 = _reReadTime;

            TimersManager.Instance.FireAfter( 0.2f, () => {
                if(Guard.IsValid(this)){
                    CButton active = _buttons[_activeButton];
                    if(Guard.IsValid(active)) active.ForceReset();
                }
            });
        }
    }
}
