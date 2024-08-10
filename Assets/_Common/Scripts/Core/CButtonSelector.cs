using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

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

    private bool SelectButton(CButton button){
        int currentActiveButton = _activeButton;

        for(int i = 0; i < _buttons.Length; i++) {
            if(button == _buttons[i]) _activeButton = i;
        }

        return currentActiveButton != _activeButton;
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){

        if(gameplayEvent.type == GameplayEventType.ButtonOvervieved){
            CButton button = gameplayEvent.parameter as CButton;
            if(Guard.IsValid(button)){
                if(SelectButton(button)) 
                {
                    AudioSystem.Instance.PlayEffect("ButtonChange", 1);
                    Debug.LogWarning("Soruce3");
                }
            }
        }
        else if(gameplayEvent.type == GameplayEventType.ButtonOvervieved_Silent){
            CButton button = gameplayEvent.parameter as CButton;
            if(Guard.IsValid(button)) SelectButton(button);
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
        Events.Gameplay.RegisterListener(this, GameplayEventType.ButtonOvervieved_Silent);
        
        RemoveInactiveButtons();
        if(_forceActivationOnStart) TimersManager.Instance.FireAfter( 0.3f, Enable );
    }

    public void Enable(){
        if(Guard.IsValid(this)){
            Events.Gameplay.RiseEvent(
                new GameplayEvent(
                    GameplayEventType.ButtonOvervieved_Silent, 
                    _buttons[_activeButton]));
        }
    }

    private void ProcessTransverseMove(){
        if(_buttons.Length == 1) {
            if(InputHandler.GetVertical() + InputHandler.GetHorizontal() != 0){
                Events.Gameplay.RiseEvent(
                    new GameplayEvent(
                        GameplayEventType.ButtonOvervieved, 
                        _buttons[0]));
            }
            return;
        }

        float verticalChange = InputHandler.GetVertical();
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


            if(currentActiveButton != _activeButton) {
                AudioSystem.Instance.PlayEffect("ButtonChange", 1);
                Debug.LogWarning("Soruce1");
            }
            return;
        }

        float horizontalChange = InputHandler.GetHorizontal();
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

            if(currentActiveButton != _activeButton) {
                AudioSystem.Instance.PlayEffect("ButtonChange", 1);
                Debug.LogWarning("Soruce2");
            }
        }
    }

    void Update()
    {
        _elapsedTime  -= Time.deltaTime;
        _elapsedTime1 -= Time.deltaTime;
        ProcessTransverseMove();

        if(_elapsedTime1 > 0) return;
        if(InputHandler.GetKey(InputKey.Confirm)){
            Debug.Log(_buttons[_activeButton].name + " PointDown");
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
