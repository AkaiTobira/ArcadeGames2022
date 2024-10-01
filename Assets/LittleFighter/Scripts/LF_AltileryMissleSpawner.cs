using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LF_AltileryMissleSpawner : CMonoBehaviour, IDealDamage
{
    [SerializeField] protected LF_EnemyStats stats;

    [SerializeField] Sprite[] _loadingSprites;
    [SerializeField] Sprite[] _afterHitSprites;

    [SerializeField] float _missleLandingTime;
    [SerializeField] float _explosionTime;
    [SerializeField] Transform _missleSpawnPoint;
    [SerializeField] GameObject missle;
    [SerializeField] string _missleStart;
    [SerializeField] string _missleLanding;

    private float _existTime;
    private float _timeStep;
    private int   _step;
    private Image _image;
    private GameObject _spawnedMissle = null;
    private bool missleLanded;

    protected override void Awake(){
        _image = GetComponent<Image>();
        base.Awake();
    }

    private void OnEnable() {
        _existTime = 0;
        _timeStep =  _missleLandingTime/_loadingSprites.Length;
        _image.sprite = _loadingSprites[0];

        AudioSystem.PlaySample(_missleStart);
    }

    protected void Update() {
        _existTime += Time.deltaTime;
        if(_spawnedMissle != null){

            if( _spawnedMissle.transform.position.y < transform.position.y + 1){
                _spawnedMissle.GetComponent<CircleCollider2D>().enabled = true;
            }

            if( _spawnedMissle.transform.position.y < transform.position.y)
            {
                missleLanded = true;
                Destroy(_spawnedMissle);
                _spawnedMissle = null;

                _step = 0;
                _existTime = 0;

                _timeStep =  _explosionTime/_afterHitSprites.Length;
                _image.sprite = _afterHitSprites[0];

                AudioSystem.PlaySample(_missleLanding);
            }
        }else if(missleLanded){
            if(_timeStep * (_step + 1) < _existTime){
                if(_step == _afterHitSprites.Length){
                    enabled = false;
                    Destroy(gameObject);
                }else{
                    _image.sprite = _afterHitSprites[_step];
                    _step++;

                    
                } 
            }
        }else{
            if(_timeStep * (_step + 1) < _existTime){
                if(_step == _loadingSprites.Length){
                    _spawnedMissle = Instantiate( missle, _missleSpawnPoint.position, Quaternion.identity, transform.parent);
                    LF_ColliderSide side = _spawnedMissle.GetComponent<LF_ColliderSide>();

                    side.SetParent(this);
                    side.GetComponent<BS_Missle>().Setup(Vector3.down);
                    _spawnedMissle.GetComponent<CircleCollider2D>().enabled = false;
                }else{
                    _image.sprite = _loadingSprites[_step];
                    _step++;
                } 
            }
        }
    }

    public float GetDamage()
    {
        return stats.Damage;
    }
}
