using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class ScreenAnimation : MonoBehaviour{

    [Serializable]
    protected class Screen{
        [SerializeField] public Image _image;
        [SerializeField] public Color _color;
        [SerializeField] public bool _autoContinue;
        [SerializeField] public float _showTimeDuration = 3f;
        [SerializeField] public float _hideTimeDuration = 1.5f;
        [SerializeField] public float _waitingTimeDuration = 3f;
    }

    protected enum State{
        Showing,
        Waiting,
        Hiding,
        SceneSwitch,
        Inactive,
    }

    [SerializeField][NonReorderable] private List<Screen> _screens;

    [SerializeField] private SceneLoader _loader;

    int _currentIndex = 0;
    float _elapsedTime = 0;
    float _elapsedTime_max = 0;
    
    protected State CurrentState = State.Showing;
    protected Screen ActiveAnimation;


    void Start(){
        for(int i = 0; i < _screens.Count; i++) {
            _screens[i]._image.gameObject.SetActive(false);
        }

        _currentIndex = 0;
        ActiveAnimation = _screens[_currentIndex];
        ActiveAnimation._image.gameObject.SetActive(true);
        SetState(State.Showing, ActiveAnimation._showTimeDuration);
        OnStart();
    }

    void Update()
    {
        _elapsedTime -= Time.deltaTime;
        OnStateUpdate();

        switch(CurrentState){
            //Showing picture
            case State.Showing: 
                if(_elapsedTime <= 0){
                    SetState(State.Waiting, ActiveAnimation._waitingTimeDuration);
                }
                break;
            case State.Waiting:
                if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)){
                    SetState(State.Hiding, ActiveAnimation._hideTimeDuration);
                    return;
                }
                if(_elapsedTime <= 0){
                    if(_screens[_currentIndex]._autoContinue) SetState(State.Hiding, ActiveAnimation._hideTimeDuration);
                }
                break;
            //Show curtain
            case State.Hiding: 
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
            CurrentState = State.Inactive;
            return;
        }
        ActiveAnimation._image.gameObject.SetActive(false);
        ActiveAnimation = _screens[_currentIndex];
        ActiveAnimation._image.gameObject.SetActive(true);
        SetState(State.Showing, ActiveAnimation._showTimeDuration);
    }

    private void SetState(State nextState, float time){
        OnStateExit(CurrentState);
        CurrentState = nextState;
        _elapsedTime     = time;
        _elapsedTime_max = time;
        OnStateEnter(CurrentState);
    }

    protected float GetTimeRate(){
        return Mathf.Max( 0, Mathf.Min(_elapsedTime/_elapsedTime_max));
    }

    protected abstract void OnStateEnter(State nextState);
    protected abstract void OnStateExit(State nextState);
    protected abstract void OnStart();
    protected abstract void OnStateUpdate();
}