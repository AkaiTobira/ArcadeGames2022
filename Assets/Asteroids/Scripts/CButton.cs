using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using TMPro;

public abstract class CButtonHandler : CMonoBehaviour, IPointerDownHandler, IPointerUpHandler, IListenToGameplayEvents
{
    [SerializeField] GameObject _overwievedFrame;
    private bool _overview;

    protected virtual void Awake() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.ButtonOvervieved);
    }

    public virtual void OnGameEvent(GameplayEvent gameEvent){
        if(!Guard.IsValid(this) || !gameObject.activeSelf) return;

        
        if(gameEvent.type == GameplayEventType.ButtonOvervieved){
            CButtonHandler activeButton = gameEvent.parameter as CButtonHandler;

            if(activeButton == null) {
                Debug.Log(name);
            }

            Overwiev = Guard.IsValid(activeButton) && activeButton == this;
        }
        
    }
    
    public virtual bool Overwiev{
        get => _overview;
        set{
            _overview = value; //&& !PlatformDetector.IsMobile();
            if(Guard.IsValid(_overwievedFrame)) _overwievedFrame.SetActive(_overview);
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData){
    //    Events.UI.RiseEvent(new UIGameEvent(UIEventType.ButtonOvervieved, this));
    }

    public virtual void OnPointerUp(PointerEventData eventData){

    }

    protected virtual void OnDestroy() {
    //    Events.UI.DeregisterListener(this, UIEventType.ButtonOvervieved);
    }
}

public abstract class CButtonHoverableHandler : CButtonHandler,  IPointerEnterHandler, IPointerExitHandler {

    protected Action _onPointerEnter;
    protected Action _onPointerExit;

    private bool isInside;

    public virtual void OnPointerEnter(PointerEventData eventData){
        if(isInside) return;
        isInside = true;
        Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.ButtonOvervieved, this));
        _onPointerEnter?.Invoke();
    }

    public virtual void OnPointerExit(PointerEventData eventData){
        if(!isInside) return;
        isInside = false;
        _onPointerExit?.Invoke();
    }
}



public class CButton : CButtonHoverableHandler, IPointerUpHandler, IListenToGameplayEvents
{
    [Serializable]
    private class TextColor{
        [SerializeField] public ButtonState State;
        [SerializeField] public Color Color;
    }

    [SerializeField] private bool _interactable;
    [SerializeField] private string _buttonText;
    [SerializeField] string _audioTap;
    [SerializeField] Sprite _pressed;
    [SerializeField] Sprite _hover;
    [SerializeField] Sprite _inactive;
    [SerializeField] Sprite _active;
    [SerializeField] TextMeshProUGUI _buttonName;
    [NonReorderable][SerializeField] private TextColor[] _textColorTilt;

    public enum ButtonState{
        Active,
        Inactive,
        Hovered,
        Pressed
    }

    private ButtonState _currentState = ButtonState.Pressed;
    [NonReorderable][SerializeField] Image[] _images = new Image[4];
    [SerializeField] private UnityEvent _OnClick;
    [SerializeField] private UnityEvent _OnClickUp;
    [SerializeField] private UnityEvent _OnHoldPress;

    private Color[] _textColors = new Color[4];

    [SerializeField] ButtonState _activeState;

    private bool _isPressed;
    private bool _isInside;


    void Start() {
        ButtonText = _buttonText;
        Overwiev = false;

        _images[(int)ButtonState.Active  ].sprite = _active;
        _images[(int)ButtonState.Inactive].sprite = _inactive;
        _images[(int)ButtonState.Hovered ].sprite = _hover;
        _images[(int)ButtonState.Pressed ].sprite = _pressed;

        _images[(int)ButtonState.Active  ].enabled = _active   != null;
        _images[(int)ButtonState.Inactive].enabled = _inactive != null;
        _images[(int)ButtonState.Hovered ].enabled = _hover    != null;
        _images[(int)ButtonState.Pressed ].enabled = _pressed  != null;

    
        if(Guard.IsValid(_buttonName) && !string.IsNullOrEmpty(_buttonText)){
            _textColors[(int)ButtonState.Active  ] = GetColorFromSetting(ButtonState.Active);
            _textColors[(int)ButtonState.Inactive] = GetColorFromSetting(ButtonState.Inactive);
            _textColors[(int)ButtonState.Hovered ] = GetColorFromSetting(ButtonState.Hovered);
            _textColors[(int)ButtonState.Pressed ] = GetColorFromSetting(ButtonState.Pressed);
        }

        SetState( _interactable ? ButtonState.Active : ButtonState.Inactive);
    }

    protected override void Awake()
    {
        Events.Gameplay.RegisterListener(this, GameplayEventType.LocalizationUpdate);
        base.Awake();
    }

    
    public override void OnGameEvent(GameplayEvent gameEvent)
    {
        if(gameEvent.type == GameplayEventType.LocalizationUpdate){
            ButtonText = ButtonText;
        }

        base.OnGameEvent(gameEvent);
    }
    

    private void Update() {
        if(_isPressed) _OnHoldPress?.Invoke();
    }

    private Color GetColorFromSetting(ButtonState state){
        for(int i = 0; i < _textColorTilt.Length; i++) {
            if(state == _textColorTilt[i].State) return _textColorTilt[i].Color;
        }

        if(Guard.IsValid(_buttonName)) return _buttonName.color;
        return Color.white;
    }

    public string ButtonText{
        get => _buttonText;
        set{
            _buttonText = value;
            if(Guard.IsValid(_buttonName)) _buttonName.text = AutoTranslator.Translate(value);
        }
    }

    public void ForceState(ButtonState state){
        SetState(state);
    }

    private void SetState(ButtonState state){
        if(_currentState == state || Guard.IsValid(this) == false) return;

        _currentState = state;
        _activeState = _currentState;
        for(ButtonState buttonState = ButtonState.Active; 
            buttonState <= ButtonState.Pressed; 
            buttonState++) {
                _images[(int)buttonState].gameObject.SetActive(buttonState == state);
        }
        
        if(Guard.IsValid(_buttonName) && !string.IsNullOrEmpty(_buttonText)){
            ButtonText = _buttonText;
            _buttonName.color = _textColors[(int)state];
        }
    }

    public bool Interactable{
        get => _interactable;
        set{
            _interactable = value;
            SetState( _interactable ? ButtonState.Active : ButtonState.Inactive);
        }
    } 

    public override void OnPointerDown(PointerEventData eventData){
        if(!_interactable) return;

        if(_currentState != ButtonState.Pressed){
            CallAfterFixedUpdate(() => { _OnClick?.Invoke();});
            base.OnPointerDown(eventData);
        }

        _isPressed = true;

        SetState(ButtonState.Pressed);
    }

    public override void OnPointerUp(PointerEventData eventData){
        if(!_interactable) return;

        CallAfterFixedUpdate(
            ()=> CallAfterFixedUpdate(
                ()=> {
                    if(!Guard.IsValid(this)) return;
                    if(_currentState == ButtonState.Pressed) SetState(ButtonState.Active);
                    _OnClickUp?.Invoke();
                    }));

        _isPressed = false;

    //    SetState(ButtonState.Active);
    }

    public override void OnPointerEnter(PointerEventData eventData){
        if(!_interactable) return;
        
        if(_currentState != ButtonState.Hovered){
            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.ButtonOvervieved, this));
        }

        SetState(ButtonState.Hovered);
    }

    public override void OnPointerExit(PointerEventData eventData){
        if(!_interactable) return;
        SetState(ButtonState.Active);
    }
}
