using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AlphaManipulationAnimation : MonoBehaviour
{
    [Serializable]
    class ScreenAnimation{
        [SerializeField] public Image _image;
        [SerializeField] public Color _color;
        [SerializeField] public bool _autoContinue;
        [SerializeField] public float _showTimeDuration = 3f;
        [SerializeField] public float _hideTimeDuration = 1.5f;
        [SerializeField] public float _waitingTimeDuration = 3f;
    }

    private enum State{
        Showing,
        Waiting,
        Hiding,
        SceneSwitch,
    }

    [SerializeField][NonReorderable] private List<ScreenAnimation> _screens;
    [SerializeField] private Image _curtain; 
    [SerializeField] private SceneLoader _loader;

    int _currentIndex = 0;
    float _elapsedTime = 0;
    State _curremtState = State.Showing;
    ScreenAnimation _activeAnimation;

    void Start(){
        for(int i = 0; i < _screens.Count; i++) {
            _screens[i]._image.gameObject.SetActive(false);
        }

        _currentIndex = 0;
        _activeAnimation = _screens[_currentIndex];

        _activeAnimation._image.gameObject.SetActive(true);
        SetState(State.Showing, _activeAnimation._showTimeDuration);
        SetColorRate(1.0f);

    }

    void Update()
    {
        _elapsedTime -= Time.deltaTime;

        switch(_curremtState){
            //Showing picture
            case State.Showing: 

                SetColorRate(Mathf.Max((_elapsedTime/_activeAnimation._showTimeDuration), 0));

                if(_elapsedTime <= 0){
                    _curtain.color = _screens[_currentIndex]._color;
                    SetState(State.Waiting, _activeAnimation._waitingTimeDuration);
                }
                break;
            case State.Waiting: 

                if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)){
                    SetState(State.Hiding, _activeAnimation._hideTimeDuration);
                    return;
                }

                if(_elapsedTime <= 0){
                    if(_screens[_currentIndex]._autoContinue) SetState(State.Hiding, _activeAnimation._hideTimeDuration);
                }
                break;
            //Show curtain
            case State.Hiding: 

                SetColorRate(1.0f - (_elapsedTime/_activeAnimation._hideTimeDuration));

                if(_elapsedTime <= 0){
                    SetState(State.SceneSwitch, 0);
                }
                break;


            case State.SceneSwitch:
                ChangeScreen();
                break;
        }
    }

    private void ChangeScreen(){
        _currentIndex++;
        if(_currentIndex >= _screens.Count){
            _loader.OnSceneLoadAsync();
            return;
        }
        _activeAnimation._image.gameObject.SetActive(false);
        _activeAnimation = _screens[_currentIndex];
        _activeAnimation._image.gameObject.SetActive(true);
        SetState(State.Showing, _activeAnimation._showTimeDuration);
    }

    private void SetColorRate(float rate){
        Color color = _curtain.color;
        color.a = rate;
        _curtain.color = color;
    }

    private void SetState(State nextState, float time){
        _curremtState = nextState;
        _elapsedTime = time;
    }
}
