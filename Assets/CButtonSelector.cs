using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CButtonSelector : MonoBehaviour
{
    [SerializeField] CButton[] _buttons;

    int _activeButton;
    float _reReadTime = 0.2f;
    float _elapsedTime = 0;

    void Start()
    {
        _activeButton = 0;
        Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.ButtonOvervieved, _buttons[_activeButton]));
    }

    void Update()
    {
        float verticalChange = Input.GetAxisRaw("Vertical");
        _elapsedTime -= Time.deltaTime;
        if(_elapsedTime <= 0 && Mathf.Abs(verticalChange) > 0.3f) {
            _elapsedTime = _reReadTime;

            _activeButton = (_activeButton - ((int)Mathf.Sign(verticalChange)) + _buttons.Length)%(_buttons.Length);
            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.ButtonOvervieved, _buttons[_activeButton]));
        }

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)) _buttons[_activeButton].OnPointerDown(null);
    }
}
