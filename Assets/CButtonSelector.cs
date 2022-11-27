using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CButtonSelector : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] CButton[] _buttons;
    [SerializeField] bool _isVertical = true;

    int _activeButton;
    float _reReadTime = 0.2f;
    float _elapsedTime = 0;
    float _elapsedTime1 = 0;

    public void OnGameEvent(GameplayEvent gameplayEvent){

        if(gameplayEvent.type == GameplayEventType.ButtonOvervieved){
            CButton button = gameplayEvent.parameter as CButton;
            if(Guard.IsValid(button)){

                for(int i = 0; i < _buttons.Length; i++) {
                    if(button == _buttons[i]) _activeButton = i;
                }

                AudioSystem.Instance.PlayEffect("ButtonChange", 1);
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

        TimersManager.Instance.FireAfter( 0.3f, () => {
            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.ButtonOvervieved, _buttons[_activeButton]));

    //        _buttons[0].ForceState(CButton.ButtonState.Hovered);
        });

    }

    private float GetDirection(){
        return  _isVertical ? Input.GetAxisRaw("Vertical") : Input.GetAxisRaw("Horizontal");
    }



    void Update()
    {
        float verticalChange = GetDirection();
        _elapsedTime  -= Time.deltaTime;
        _elapsedTime1 -= Time.deltaTime;
        if(_elapsedTime <= 0 && Mathf.Abs(verticalChange) > 0.3f) {
            _elapsedTime = _reReadTime;
            _activeButton = (_activeButton - ((int)Mathf.Sign(verticalChange)) + _buttons.Length)%(_buttons.Length);
            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.ButtonOvervieved, _buttons[_activeButton]));
        }

        if(_elapsedTime1 > 0) return;
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            _buttons[_activeButton].OnPointerDown(null);
            _elapsedTime1 = _reReadTime;
        }
    }
}
