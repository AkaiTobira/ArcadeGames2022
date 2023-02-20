using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class ScreenAnimation : MonoBehaviour, IListenToGameplayEvents{

    [Serializable]
    protected class Screen{
        [SerializeField] public Image _image;
        [SerializeField] public Color _color;
        [SerializeField] public bool _autoContinue;
        [SerializeField] public bool _hasCustomContinue;
        [SerializeField] public Component _activateComponent;
        [SerializeField] public float _showTimeDuration = 3f;
        [SerializeField] public float _hideTimeDuration = 1.5f;
        [SerializeField] public float _waitingTimeDuration = 3f;
        [SerializeField] public bool _ignore;
    }

    protected enum State{
        Showing,
        Waiting,
        Hiding,
        SceneSwitch,
        Inactive,
    }

    [SerializeField][NonReorderable] protected List<Screen> _screens;

    [SerializeField] private SceneLoader _loader;

    int _currentIndex = 0;
    float _elapsedTime = 0;
    float _elapsedTime_max = 0;
    
    protected State CurrentState = State.Showing;
    protected Screen ActiveAnimation;

    public virtual void OnGameEvent(GameplayEvent gameplayEvent){
        if(gameplayEvent.type == GameplayEventType.ContinueAnimation){
            if(CurrentState == State.Waiting) SetState(State.Hiding, ActiveAnimation._hideTimeDuration);
        }
    }

    void Start(){

        Events.Gameplay.RegisterListener(this, GameplayEventType.ContinueAnimation);

        Vector3 position = _screens[0]._image.transform.position;

        for(int i = 0; i < _screens.Count; i++) {
            _screens[i]._image.gameObject.SetActive(false);
            _screens[i]._image.transform.position = position;
        }

        OnStart();
    }

    void Initialiaze(){
        _currentIndex = 0;
        ActiveAnimation = _screens[_currentIndex];
        ActiveAnimation._image.gameObject.SetActive(true);
        SetState(State.Showing, ActiveAnimation._showTimeDuration);
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

                #if !UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN
                    if(ActiveAnimation._hasCustomContinue){
                        return;
                    }
                #endif

                if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.touchCount > 0){
                    SetState(State.Hiding, ActiveAnimation._hideTimeDuration);
//                    Debug.Log("Next Event");
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
        while(_currentIndex < _screens.Count ) {
            _currentIndex++;
            if(_currentIndex >= _screens.Count) break;
            if(!_screens[_currentIndex]._ignore) break;
        }
        
        if(_currentIndex >= _screens.Count){
            _loader?.OnSceneLoadAsync();
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
    protected virtual void OnStart(){
        Initialiaze();
    }
    protected abstract void OnStateUpdate();
}