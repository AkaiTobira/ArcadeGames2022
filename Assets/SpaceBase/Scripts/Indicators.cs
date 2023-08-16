using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IIsTrackable{
    bool ShouldBeTracked();
}


public class Indicators : MonoBehaviour
{
    [SerializeField] MonoBehaviour[] _trackedObjects;
    [SerializeField] GameObject[]    _indicators;
    [SerializeField] GameObject      _player;
    [SerializeField] Color[]         _colors;

    List<IIsTrackable> _trackable = new List<IIsTrackable>();

    private void Awake(){
        for(int i = 0; i < _indicators.Length; i++) {
            _indicators[i].SetActive(false);
        }
        for(int i = 0; i < _trackedObjects.Length; i++) {
            IIsTrackable trackable = _trackedObjects[i].GetComponent<IIsTrackable>();
            if(trackable != null) _trackable.Add(trackable);
        }
    }

    private void Update() {
        for(int i = 0; i < _indicators.Length; i++) {
            _indicators[i].SetActive(false);
            _indicators[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            _indicators[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            _indicators[i].transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        }
        for(int i = 0; i < _trackable.Count; i++) {
            if(_trackable[i].ShouldBeTracked()) EnableIndicator(_trackedObjects[i], i); 
        }

        for(int i=0; i < _trackable.Count; i++){
            if(_trackable[i].ShouldBeTracked()) return;
        }

        enabled = false;
        Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.GameOver, GameOver.Victory));
        AudioSystem.PlaySample("Frogger_Victory", 1);
    }

    private void EnableIndicator(MonoBehaviour mb, int ObjectIndex){
        if((_player.transform.position - mb.transform.position).sqrMagnitude < 300) return;


        GameObject closesetIndicator = GetClosestIndicator(mb.transform.position);
        closesetIndicator.SetActive(true);
        closesetIndicator.transform.GetChild(0).GetChild(ObjectIndex).gameObject.SetActive(true);
        closesetIndicator.GetComponent<Image>().color = _colors[ObjectIndex];
    }

    private GameObject GetClosestIndicator(Vector3 pos){
        GameObject go  = _indicators[0];

        float distance = 999999999;
        for(int i = 0; i < _indicators.Length; i++) {
            
            float dis = (_indicators[i].transform.position - pos).sqrMagnitude;
            if(dis < distance){
                distance = dis;
                go = _indicators[i];
            }
        }

        return go;
    }
}
