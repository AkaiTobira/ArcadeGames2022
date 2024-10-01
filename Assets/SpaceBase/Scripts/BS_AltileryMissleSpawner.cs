using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BS_AltileryMissleSpawner : CMonoBehaviour//, IDealDamage
{
    [SerializeField] Sprite[] _loadingSprites;
    [SerializeField] Sprite[] _afterHitSprites;
    [SerializeField] CircleCollider2D _hitBox;
    [SerializeField] LF_ColliderSide _side;

    [SerializeField] float _missleLandingTime;
    [SerializeField] float _explosionTime;
    [SerializeField] string _shotSound = "Asteroid_Shoot";
    [SerializeField] string _landedSound = "Asteroid_Shoot";


    private float _existTime;
    private float _timeStep;
    private int   _step;
    private Image _image;
    private bool missleLanded = false;


    protected override void Awake(){
        _image = GetComponent<Image>();
        base.Awake();
    }

    public void SetupParent(MonoBehaviour behaviour){
        _side.SetParent(behaviour);
    }

    private void OnEnable() {
        _existTime = 0;
        _timeStep =  _missleLandingTime/_loadingSprites.Length;
        _image.sprite = _loadingSprites[0];

        AudioSystem.Instance.PlayEffect(_shotSound, 1, true);
    }

    protected void Update() {
        _existTime += Time.deltaTime;
        if(missleLanded){
            float percent = _existTime/_explosionTime;
            if(percent > 1){
                Destroy(gameObject);
                enabled = false;
            }else{
                _image.sprite = _afterHitSprites[(int)(percent * _afterHitSprites.Length)];
                _hitBox.enabled = (int)(percent * _afterHitSprites.Length) < _afterHitSprites.Length -2;
            }
        }else{
            float percent = _existTime/_missleLandingTime;
            if(percent > 1){
                missleLanded = true;
                AudioSystem.Instance.PlayEffect(_landedSound, 1, true);
                _existTime = 0;
            }else{
                _image.sprite = _loadingSprites[(int)(percent * _loadingSprites.Length)];
            }
        }
    }
}
