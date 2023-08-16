using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AutoTranslatorUnit : AutoTranslatorUnitBase{

    [SerializeField] string textTag;
    TextMeshProUGUI _text;

    protected override void Initialize(){
        _text = GetComponent<TextMeshProUGUI>();
    }

    protected override void Refresh(){
        _text.font = TextAssets.GetFont();
        _text.text = AutoTranslator.Translate(textTag);
        _text.ForceMeshUpdate(false, true);
    }
}

public abstract class AutoTranslatorUnitBase : CMonoBehaviour, IListenToGameplayEvents
{

    protected abstract void Initialize();

    void Awake() {
        Initialize();
        Events.Gameplay.RegisterListener(this, GameplayEventType.LocalizationUpdate);
    }

    private void OnEnable() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.LocalizationUpdate);
        
        CallNextFrame( ()=>
            OnGameEvent(new GameplayEvent(GameplayEventType.LocalizationUpdate))
        );
    }

    public void OnGameEvent(GameplayEvent gameEvent){
        if(!Guard.IsValid(gameObject)) return;

        if(gameEvent.type == GameplayEventType.LocalizationUpdate) Refresh();
    }

    protected abstract void Refresh();
}
