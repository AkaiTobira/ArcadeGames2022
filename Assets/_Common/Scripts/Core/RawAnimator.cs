using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class RawAnimator : MonoBehaviour{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private int _keepRelative; 

    [SerializeField] private bool _fireAtEveryActivation;

    [SerializeField] private float _oneFrameDuration = 0.2f;
    [SerializeField] private bool  _looped;
    [SerializeField] private bool _playRewersed;

    int _currentFrame = 0;
    float _elapsedTimeSinceLastFrame = 0;
    bool _isPlaying = false;

    protected virtual void Start(){
        if(Guard.IsValid(_canvas)) CanvasSorter.AddCanvas(_canvas, _keepRelative);
    }

    protected virtual void Awake() {
        if(_fireAtEveryActivation) Restart();
    }

    protected virtual void OnEnable() {
        if(_fireAtEveryActivation) Restart();
    }

    public void PlayRewersedIfPlaying(){
        if(_currentFrame > 0) PlayRewersed();
    }

    public void PlayRewersed(){
        _currentFrame = GetFramesCount()-1;
        _isPlaying = true;
        _playRewersed = true;

        UpdateAnimation(_currentFrame);
        _elapsedTimeSinceLastFrame = _oneFrameDuration;
    }

    public bool IsPlaying(){
        return _isPlaying;
    }

    public void Play(){
        if(_isPlaying) return;
        _currentFrame = 0;
        _isPlaying = true;
        _playRewersed = false;
        UpdateAnimation(_currentFrame);

        _elapsedTimeSinceLastFrame = _oneFrameDuration;
    }

    public void Stop(){
        _isPlaying = false;
    }

    public void Restart(){
        Stop();
        Play();
    }

    void Update()
    {
        if(_isPlaying){
            _elapsedTimeSinceLastFrame -= Time.deltaTime;
            if(_elapsedTimeSinceLastFrame > 0) return;
            _elapsedTimeSinceLastFrame += _oneFrameDuration;

            _currentFrame += (_playRewersed) ? -1 : 1;
            if(_looped) _currentFrame = (_currentFrame + GetFramesCount())% GetFramesCount();
            if(_currentFrame >= GetFramesCount()) {return;}
            if(_currentFrame < 0) {
                _isPlaying = false;
                return;
            }
            UpdateAnimation(_currentFrame);
        }
    }

    protected abstract void UpdateAnimation(int frame);

    protected abstract int GetFramesCount();

}

