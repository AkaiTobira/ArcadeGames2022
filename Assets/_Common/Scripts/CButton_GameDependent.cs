using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CButton_GameDependent : CButton
{

    [Serializable]
    class PictureVersion{
        public GameType Type;
        public Sprite Active;
        public Sprite Inactive;
        public Sprite Hover;
        public Sprite Pressed;
        public Sprite Frame;
    }

    private static GameType ActiveGameType = GameType.NotLoaded;
    [SerializeField] private List<PictureVersion> _versions = new List<PictureVersion>();

    public static void SetActiveGameType(GameType type) { 
        ActiveGameType = type; 
        Events.Gameplay.RiseEvent(GameplayEventType.UpdateButtonGraphics);
    }

    protected override void Awake()
    {
        base.Awake();

        Events.Gameplay.RegisterListener(this, GameplayEventType.UpdateButtonGraphics);
    }

    protected override void Start()
    {
        for(int i = 0; i < _versions.Count; i++) {
            if(_versions[i].Type == ActiveGameType){
                _active   = _versions[i].Active;
                _hover    = _versions[i].Hover;
                _inactive = _versions[i].Inactive;
                _pressed  = _versions[i].Pressed;

                _overwievedFrame.GetComponent<Image>().sprite = _versions[i].Frame;
            }
        }
        
        base.Start();


    }

    public override void OnGameEvent(GameplayEvent gameEvent)
    {
        base.OnGameEvent(gameEvent);

        if(gameEvent.type == GameplayEventType.UpdateButtonGraphics){
            for(int i = 0; i < _versions.Count; i++) {
                if(_versions[i].Type == ActiveGameType){
                    _active   = _versions[i].Active;
                    _hover    = _versions[i].Hover;
                    _inactive = _versions[i].Inactive;
                    _pressed  = _versions[i].Pressed;

                    _overwievedFrame.GetComponent<Image>().sprite = _versions[i].Frame;
                    Start();
                }
            }
        }
    }
}
