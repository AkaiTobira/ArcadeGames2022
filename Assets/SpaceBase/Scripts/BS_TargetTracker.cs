using UnityEngine;
using UnityEngine.UI;

public class BS_TargetTracker : CUpdateMonoBehaviour
{
    [SerializeField] Sprite[] _loadingSprites;
    [SerializeField] Sprite[] _lockedSprites;
    [SerializeField] Image _image;

    Transform _target;
    float _duration = 99999;
    float _elapsedTime;

    float _lockedFrame = 0.2f;

    int _loadedFrame;

    public bool IsLocked() { return _elapsedTime > _duration; }

    protected override void Awake()
    {
        base.Awake();
        SetActive(false);
    }

    public void Setup(Transform t, float elapsedTime, float duration)
    {
        _target = t;
        _duration = duration;
        _elapsedTime = elapsedTime;
        _loadedFrame = 0;


        gameObject.SetActive(true);
        //_image.sprite = _loadingSprites[0];
    }

    public void TurnOff(){ gameObject.SetActive(false);}
    public float GetElapsedTime() { return gameObject.activeSelf ? _elapsedTime : 0; }

    public override void CUpdate()
    {
        base.CUpdate();

        transform.position = _target.GetComponent<ICanBeRocketTarget>().GetTargetPoint();
        ///_elapsedTime += Time.deltaTime;
        float percent = _elapsedTime/_duration;
        if(percent < 1.0f){
            _image.sprite = _loadingSprites[(int)(percent * _loadingSprites.Length)];
        }else{
            float percent2 = _elapsedTime/_lockedFrame;
           _image.sprite = _loadingSprites[(int)(percent2 * _lockedSprites.Length)%_lockedSprites.Length];
        }
    }
}
