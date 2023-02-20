using System.Collections.Generic;
using UnityEngine;

public class CUpdateManger : MonoBehaviour
{
    [SerializeField] private List<CUpdateMonoBehaviour> _registerdMonos = new List<CUpdateMonoBehaviour>();
    List<CUpdateMonoBehaviour> _unvalidMonos = new List<CUpdateMonoBehaviour>();
    private static CUpdateManger _instance;

    void Awake() {
        if(_instance == null){
            _instance = this;
            DontDestroyOnLoad(this);
        }else{
            Destroy(gameObject);
        }
    }

    public static void Register(CUpdateMonoBehaviour mono){
        if(Guard.IsValid(_instance)){
            _instance._registerdMonos.Add(mono);
        }
    }

    private void Update() {
        for(int i = 0; i < _registerdMonos.Count; i++) {
            CUpdateMonoBehaviour mono = _registerdMonos[i];
            if(!Guard.IsValid(mono)){
                _unvalidMonos.Add(mono);
                continue;
            }
            if(mono.gameObject.activeSelf) mono.CUpdate();
        }

        ClearList();
    }

    private void LateUpdate() {
        for(int i = 0; i < _registerdMonos.Count; i++) {
            CUpdateMonoBehaviour mono = _registerdMonos[i];
            if(!Guard.IsValid(mono)) continue;
            if(mono.gameObject.activeSelf) mono.CLateUpdate();
        }
    }

    private void FixedUpdate() {
        for(int i = 0; i < _registerdMonos.Count; i++) {
            CUpdateMonoBehaviour mono = _registerdMonos[i];
            if(!Guard.IsValid(mono)) continue;
            if(mono.gameObject.activeSelf) mono.CFixedUpdate();
        }
    }

    private void ClearList(){
        for(int i = 0; i < _unvalidMonos.Count; i++) {
            _registerdMonos.Remove(_unvalidMonos[i]);
        }
        if(_unvalidMonos.Count > 0) _unvalidMonos.Clear();
    }
}
