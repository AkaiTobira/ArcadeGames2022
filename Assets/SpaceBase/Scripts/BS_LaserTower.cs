using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BS_LaserTower : BS_MainTower
{
    [SerializeField] float _loadingTimeMax = 3f;
    [SerializeField] Image _image;
    [SerializeField] Color[] _colors;
    [SerializeField] LineRenderer _renderer;
    [SerializeField] Transform[] _sides;

    private float _elapsedTime;

    private bool _needCooldown;

    public override void Shoot(Vector3 direction, MonoBehaviour spawner, bool canShoot, bool canShootContinuesly, bool ignoreShoot = true){
        if(canShootContinuesly && !_needCooldown) {
            _elapsedTime += Time.deltaTime;
            if(_elapsedTime >= _loadingTimeMax){ _needCooldown = true; }
        } else {
            _elapsedTime = Mathf.Max(_elapsedTime - Time.deltaTime, 0 );
            if(_elapsedTime <= 0){ _needCooldown = false; }    
        }
        
        Debug.Log(_elapsedTime + " " + canShootContinuesly + " " + _needCooldown);

        if(_elapsedTime/_loadingTimeMax >  0.0) _image.color = _colors[0];
        if(_elapsedTime/_loadingTimeMax > 0.33) _image.color = _colors[1];
        if(_elapsedTime/_loadingTimeMax > 0.66) _image.color = _colors[2];
        if(_needCooldown) _image.color = _colors[3];
        
        if(_needCooldown || !canShootContinuesly){
            _renderer.enabled = false;
        }else if(canShootContinuesly){
            _renderer.enabled = true;

            float distance = Vector2.Distance(_missleSpawnPoint[1].position, _missleSpawnPoint[0].position);
            Vector3 vector = _missleSpawnPoint[1].position;

            for(int j = 0; j < _sides.Length; j++){
                Vector3 dir = (_sides[j].position - _missleSpawnPoint[0].position).normalized;
                RaycastHit2D[] hits = 
                    Physics2D.RaycastAll(
                        _missleSpawnPoint[0].position, 
                        dir, 
                        distance, 
                        _EnemylayerMask);
                
                bool found = false;
                for(int i = 0; i < hits.Length; i++){
                    if(hits[i].collider.tag != "HitBox") continue;
                    MonoBehaviour parent = hits[i].collider.transform.
                        GetComponent<LF_ColliderSide>().GetParent();
                    if(parent == spawner) continue;


                    ITakeDamage side = parent.GetComponent<ITakeDamage>();
                    Debug.LogWarning(
                        side + " " + 
                        hits[i].collider + " " + 
                        hits[i].collider.transform.parent.parent);

                    if(side != null){
                        Debug.DrawLine(
                            _missleSpawnPoint[0].position, 
                            hits[i].point, 
                            Color.red);
                        vector = hits[i].point;
                        IDealDamage dealDamage = spawner.GetComponent<IDealDamage>();
                        if(dealDamage != null) side.TakeDamage(dealDamage.GetDamage(), spawner);
                        found = true;
                        break;
                    }

                    Debug.DrawLine(
                            _missleSpawnPoint[0].position, 
                            _missleSpawnPoint[0].position + dir * distance, 
                            Color.red, 10);
                }
                if(found) break;
            }

            _renderer.SetPosition(0, _missleSpawnPoint[0].position);
            _renderer.SetPosition(1, vector);
        }else{
            _renderer.enabled = false;
        }
        
        _image.fillAmount = _elapsedTime / _loadingTimeMax; 
    }
}
