using System;
using UnityEngine;

public class DD2_StartLevel : CUpdateMonoBehaviour
{
    [SerializeField] RectTransform _start;
    [SerializeField] RectTransform _end;
    [SerializeField] RectTransform _core;
    [SerializeField] LevelManager _levelManager;
    [SerializeField] SceneLoader _sceneLoader;
    [SerializeField] SoundPlay _soundPlay;
    [SerializeField] AutoTranslatorUnit _autoTranslator;
    [SerializeField] CButtonSelector _buttonSelector;

    bool _isSeen = true;
    bool _inProgress = false;

    protected override void Start() {
        base.Start();

        LevelController.StopGame = true;
        ChangeVisisbility(true);
        _buttonSelector.gameObject.SetActive(false);
    }

    public override void CUpdate()
    {
        base.CUpdate();

        if(LevelController.StopGame && InputHandler.GetKey(InputKey.Confirm) && !_buttonSelector.gameObject.activeInHierarchy){
            ChangeVisisbility(false);
            _levelManager.Setup(); 
        }
    }

    public void ChangeVisisbility(bool isSeen){
        Debug.LogWarning(isSeen + " : CALLED");
        if(isSeen && !_isSeen)
        {
            _isSeen = true;
            TweenManager.Instance.TweenTo(_core, _end, 0.2f, OnShowEnd);
            _inProgress = true;
            _buttonSelector.gameObject.SetActive(true);
        }
        else if(!isSeen && _isSeen)
        {
            _isSeen = false;
            TweenManager.Instance.TweenTo(_core, _start, 0.2f, OnHideEnd);
            _inProgress = true;
            _soundPlay.PlaySound();
            _buttonSelector.gameObject.SetActive(false);
        }
    }

    private void OnShowEnd()
    {
        _inProgress = false; 
        LevelController.StopGame = true;
    }

    private void OnHideEnd()
    {
        _inProgress = false; 

        LevelController.StopGame = false; 

        if(!delayDeparture){
            GameOverActivator.isGameOverReach = false;
        }

        if(DigDugPlayedMaps.IsFull()) _autoTranslator.SetTag("NoMorePlanets");
        else _autoTranslator.SetTag("NextPlanetAsk2");
    }

    bool delayDeparture = false;
    public void OnDelayDeparture(){
        if(_inProgress) return;
        delayDeparture = true;
        ChangeVisisbility(false);

        TimersManager.Instance.FireAfter(10, () => { delayDeparture = false; ChangeVisisbility(true); });
    }

    public void OnConitnue(){
        if(_inProgress) return;
        if(LevelController.StopGame && !DigDugPlayedMaps.IsFull()){
            ChangeVisisbility(false);
            _levelManager.Setup();
        }
        if(LevelController.StopGame && DigDugPlayedMaps.IsFull()){
            _sceneLoader.OnSceneLoadAsync();
            _soundPlay.PlaySound();
        }
    }
}
