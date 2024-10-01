using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;
using UnityEngine.UI;

public enum G_RandomEvents{
    InverseMovement,
    MorePlanties,
    Max,
    CodeBreak,
}


public class G_GameController : CUpdateMonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] G_PlantInstance[] planties;
    [SerializeField] G_AlertDisplayer displayer;
    [SerializeField] GameObject[] hpIcons;
    [SerializeField] float degradationTime;
    [SerializeField] float randomEventBreak;
    [SerializeField] float inverseMovementDuration;
    [SerializeField] float codeMovementBreakDuration;
    [SerializeField] SceneLoader sceneLoader;


    private float timeElapsed;
    private float timeElapsedFromStart;
    private float timeToRandomEvent;
    private G_RandomEvents _lastEvent;

    private List<G_PlantInstance> _activePlanties = new List<G_PlantInstance>();
    private List<G_PlantInstance> _inActivePlanties = new List<G_PlantInstance>();
    
    private int _hpBarNumber = 2;

    protected override void Awake()
    {
        base.Awake();
        _activePlanties = new List<G_PlantInstance>{
            planties[0], planties[2], planties[4], planties[6]
        };
        _inActivePlanties = new List<G_PlantInstance>{
            planties[1], planties[3], planties[5], planties[7]
        };

        CUtils.Shuffle(_inActivePlanties);

        Events.Gameplay.RegisterListener(this, GameplayEventType.PlayerDied);
    }
    public void OnGameEvent(GameplayEvent gameEvent){
        if(gameEvent.type == GameplayEventType.PlayerDied){
            G_PlantInstance plantie = gameEvent.parameter as G_PlantInstance;
            if(Guard.IsValid(plantie) && _hpBarNumber >= 0){
                _hpBarNumber--;
                for(int i = 0; i < hpIcons.Length; i++) hpIcons[i].SetActive(_hpBarNumber >= i);
                _activePlanties.Remove(plantie);
                _inActivePlanties.Add(plantie);
            }else if(_hpBarNumber < 0){
                sceneLoader.OnSceneLoadAsync();
            }
        }
    }

    public override void CUpdate()
    {
        base.CUpdate();

        timeElapsed -= Time.deltaTime;
        timeElapsedFromStart += Time.deltaTime;
        timeToRandomEvent  += Time.deltaTime;


        if(timeToRandomEvent > randomEventBreak){
            timeToRandomEvent -= randomEventBreak;

            G_RandomEvents @event = (G_RandomEvents)CUtils.Rand((int)G_RandomEvents.Max);
            if(_lastEvent != @event){
                ProcessRandomEvent(@event);
                _lastEvent = @event;
            }else{
                timeToRandomEvent = 0;
            }
        }

        if(timeElapsed < 0){
            timeElapsed = degradationTime - (timeElapsedFromStart * 0.01f);
            timeElapsed = MathF.Max(timeElapsed, 0.5f);

            CUtils.Shuffle(_activePlanties);
            for(int i = 0; i < _activePlanties.Count; i++) 
                if(_activePlanties[i].CanDegradate()) {
                    _activePlanties[i].Degradate();
                    return;
                }

            //Game Is Over
        }
    }

    private void ProcessRandomEvent(G_RandomEvents @event)
    {
        Debug.Log(@event);

        switch(@event){
            case G_RandomEvents.MorePlanties:
                if(_inActivePlanties.Count != 0){
                    _activePlanties.Add(_inActivePlanties[0]);
                    _inActivePlanties[0].Setup();
                    _inActivePlanties.RemoveAt(0);

                    displayer.Show(@event);
                }
            break;
            case G_RandomEvents.InverseMovement:
                G_Player.InverseMovement = true;
                TimersManager.Instance.FireAfter(inverseMovementDuration, () => G_Player.InverseMovement = false);
                displayer.Show(@event, inverseMovementDuration);
            break;
            case G_RandomEvents.CodeBreak:
                TimersManager.Instance.FireAfter(codeMovementBreakDuration, () => Events.Gameplay.RiseEvent(GameplayEventType.EndGenerateKeys));
                Events.Gameplay.RiseEvent(GameplayEventType.GenerateKeys);
                displayer.Show(@event, codeMovementBreakDuration);
            break;
        }
    }
}
