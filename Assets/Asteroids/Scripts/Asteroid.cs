using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class Animations{
    [SerializeField][NonReorderable] public Sprites[] Animation;
}


[Serializable]
public class Sprites{
    [SerializeField][NonReorderable] public Sprite[] Frames;
}

public class Asteroid : WallThrought, IListenToGameplayEvents
{

    public enum EAsteroidSize{
        EAS_HUGE,
        EAS_NORMAL,
        EAS_SMALL
    }


    [SerializeField][NonReorderable] Animations[] animations;
    [SerializeField] Image    _frame;

    [SerializeField] Image _image;
    [SerializeField] float[] _speed;
    [SerializeField] float[] _speedAccelerations;
    
    [SerializeField] EAsteroidSize _size;

    [SerializeField] GameObject _prefab;

    


    public EAsteroidSize GetSize() => _size;

    WaitForSeconds ANIM_TIMER = new WaitForSeconds(0.3f);
    Sprites _usedSprites;
    int _activeFrame;

    int _generation;

    Vector3 _forwardDirection;
    Vector3 _startScale;

    float creapyInsideScale;


    private void Awake(){
        _startScale = transform.localScale;
    }

    private void Start() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.ResizeAsteroids);

    }

    public bool IsShootedDown(){
        return _frame.enabled;
    }


    public void OnGameEvent(GameplayEvent gameplayEvent){

        if(enabled == false) return;
        if(_frame.enabled) return;

        if(gameplayEvent.type == GameplayEventType.ResizeAsteroids){

            ValidSizes types = (ValidSizes)gameplayEvent.parameter;

            if(types.Sizes.Count == 0) return;

            if(types.InvalidSizes.Contains(_size)){

                    creapyInsideScale = 2; // transform.localScale.x / _startScale.x;

                    ScaleManager.Instance.ScaleTo(transform, new Vector3(0.01f,0.01f,1f), 0.2f, ()=>{


                    _size = types.Sizes[ UnityEngine.Random.Range(0, types.Sizes.Count)];
                    SetupIcon();

                    Vector3 newScale = (_startScale * creapyInsideScale) / ((float)_size + 1.0f ); 

                    ScaleManager.Instance.ScaleTo(transform, newScale, 0.2f, () => {
                        _startScale = newScale;
                    });
                });
            }
        }
    }

    private IEnumerator Animation(){
        while(Guard.IsValid(this)){
            yield return ANIM_TIMER;

            if(_usedSprites != null){
                _activeFrame = (_activeFrame + 1)%_usedSprites.Frames.Length;
                _image.sprite = _usedSprites.Frames[_activeFrame];
            }
        }
    }

    private void SetupIcon(){

        int sizeIndex          = (int)_size;
        int numberOfAnimations = animations[sizeIndex].Animation.Length;
        _usedSprites = animations[sizeIndex].Animation[UnityEngine.Random.Range(0, numberOfAnimations)];
        _activeFrame = UnityEngine.Random.Range(0, _usedSprites.Frames.Length);

        _image.sprite = _usedSprites.Frames[_activeFrame];
    }


    public void Setup(Vector3 direction, EAsteroidSize size, int generation){
        _forwardDirection = direction;
        _size = size;



        transform.localScale = _startScale / ((float)_size + 1.0f );

        AsteroidSpawner.AsteroidCount += 1;

        SetupIcon();

        _frame.enabled = false;
        StartCoroutine(Animation());
        _generation = generation;
    }

    override protected void Update()
    {
        base.Update();
        
        Vector3 speed = _forwardDirection * (_speed[(int)_size] + (_generation * _speedAccelerations[(int)_size]))  * Time.deltaTime;
        transform.position += speed;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag.Contains("ssle")){


            AudioSystem.Instance.PlayEffect("Asteroid_EnemyHit", 1, true);

            Destroy(GetComponent<CircleCollider2D>());
            _frame.enabled = true;
            EnableWallTeleport = false;
            _forwardDirection = new Vector3(0,1,0);

            PointsCounter.Score += (500 + _generation * 100) * ((int)_size + 1);


            Destroy(other.gameObject, 10);

            AsteroidSpawner.AsteroidCount -= 1;
        }
    }

}
