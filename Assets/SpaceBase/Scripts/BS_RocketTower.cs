using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BS_RocketTower : BS_MainTower
{

    [SerializeField] float _loadingTimeMax = 2f;
    [SerializeField] Image _image;

    
    bool isShooting = false;
    private void ShootRacket(MonoBehaviour spawner, Transform target, int i){
        if(!Guard.IsValid(target)) return;
        Vector2 aimDir = _missleSpawnPoint[i].up;
        Transform aimedTarget = target;
        LF_ColliderSide side = 
            Instantiate(
                _missle, 
                _missleSpawnPoint[i].position, 
                Quaternion.identity, 
                BS_Instances.Inst.transform
            ).GetComponent<LF_ColliderSide>();
        side.SetParent(spawner);
        side.GetComponent<BS_HomingMissle>().Setup(aimDir, aimedTarget);
    }

    HashSet<Transform> _lockedTargets = new HashSet<Transform>();
    Dictionary<Transform, float> _trackedDuration = new Dictionary<Transform, float>();

    public override void Shoot(Vector3 direction, MonoBehaviour spawner, bool canShoot, bool canShootContinuesly, bool ignoreShoot = true){
        if(canShootContinuesly) { 

            List<Transform> _targets = BS_RocketTowerTracker.GetTargets();
            HashSet<Transform> _targets2 = BS_RocketTowerTracker.GetTarget2s();

            foreach (Transform t in _lockedTargets) {
                if (_targets2.Contains(t)) continue;

                _trackedDuration.Remove(t);
            }

            int smallerLenght = _missleSpawnPoint.Length < _targets.Count ? _missleSpawnPoint.Length : _targets.Count;
            for(int i = 0; i < smallerLenght; i++){
                if(_lockedTargets.Contains(_targets[i])) {
                    _trackedDuration[_targets[i]] += Time.deltaTime;
                    continue;
                }

                _lockedTargets.Add(_targets[i]);
                _trackedDuration[_targets[i]] = 0;
            }

            List<GameObject> games = BS_RocketTowerTracker.GetTracers();
            int k = 0;

            List<Transform> toRemove = new List<Transform>();

            foreach (Transform t in _lockedTargets){
                BS_TargetTracker tracker = games[k++].GetComponent<BS_TargetTracker>();
                if(Guard.IsValid(t) && _trackedDuration.ContainsKey(t)){
                    tracker.Setup(t, _trackedDuration[t], _loadingTimeMax);
                }else{
                    toRemove.Add(t);
                    tracker.TurnOff();
                }
            }

            foreach (Transform t in toRemove) {
                _lockedTargets.Remove(t);
                _trackedDuration.Remove(t);
            }

            for(; k< games.Count; k++) games[k].GetComponent<BS_TargetTracker>().TurnOff();

            isShooting = true;
        }else{
            if(isShooting){
                isShooting = false;
                List<GameObject> games = BS_RocketTowerTracker.GetTracers();
                for(int j = 0; j < games.Count; j++){
                    BS_TargetTracker tracker = games[j].GetComponent<BS_TargetTracker>();
                    tracker.TurnOff();
                }

                int i = 0;
                foreach(KeyValuePair<Transform, float> pairs in _trackedDuration){
                    if(pairs.Value > _loadingTimeMax) ShootRacket(spawner, pairs.Key, i++);
                }

                _lockedTargets.Clear();
                _trackedDuration.Clear();
            }
        }
    }
}
