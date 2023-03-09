using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIsTrackable{
    bool ShouldBeTracked();
}


public class Indicators : MonoBehaviour
{
    [SerializeField] MonoBehaviour[] _trackedObjects;
    [SerializeField] GameObject[]    _indicators;
    [SerializeField] GameObject      _player;

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
        }
        for(int i = 0; i < _trackable.Count; i++) {
            if(_trackable[i].ShouldBeTracked()) EnableIndicator(_trackedObjects[i]); 
        }

        for(int i=0; i < _trackable.Count; i++){
            if(_trackable[i].ShouldBeTracked()) return;
        }

        enabled = false;
        Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.GameOver, GameOver.Victory));
    }

    private void EnableIndicator(MonoBehaviour mb){
        if((_player.transform.position - mb.transform.position).sqrMagnitude < 200) return;


        GameObject closesetIndicator = GetClosestIndicator(mb.transform.position);
        closesetIndicator.SetActive(true);
    }

    private GameObject GetClosestIndicator(Vector3 pos){
        GameObject go = _indicators[0];

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
