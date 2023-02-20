using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AutoTranslatorUnit : CMonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] string textTag;
    TextMeshProUGUI _text;

    void Awake() {
        _text = GetComponent<TextMeshProUGUI>();
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

        if(gameEvent.type == GameplayEventType.LocalizationUpdate){
            _text.font = TextAssets.GetFont();
            _text.text = AutoTranslator.Translate(textTag);
            _text.ForceMeshUpdate(false, true);
        }
    }
}
