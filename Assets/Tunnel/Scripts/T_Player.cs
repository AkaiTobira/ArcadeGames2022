using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class T_Player : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] SceneLoader _nextScene;
    [SerializeField] GameObject _endAnimation;
    [SerializeField] float _maxRotationSpeed = 150f;
    [SerializeField] float _rotationFriction = 0.1f;
    [SerializeField] float _accelerationRotate = 0.5f;
    [SerializeField] float _moveSpeed = 4f;

    [SerializeField] float calcRotationSpeed = 0;
    [SerializeField] Vector3 movePoint;
    [SerializeField] GameObject _shield;
    [SerializeField] GameObject _deathSpirit;

    public static int NumberOfMissles = 0;


    protected float _movePenalty = 1;
    private Transform _currentSegment;
    bool _invincibleSkip = false;
    private float pointProgress = 0;

    private void Awake() {
        HighScoreRanking.LoadRanking(GameType.Tunnel);
        Events.Gameplay.RegisterListener(this, GameplayEventType.RecolorPlayer);
        PointsCounter.Reset();
    }

    public void OnGameEvent(GameplayEvent gEvent){
        if(gEvent.type == GameplayEventType.RecolorPlayer){
            KeyValuePair<Color,Transform> paras =  (KeyValuePair<Color,Transform>) gEvent.parameter;
            transform.GetChild(0).GetComponent<Image>().color = paras.Key;
            _currentSegment = paras.Value;
        }
    }

    private void AddPoints(){
        pointProgress += Time.deltaTime * 6 * T_SegmentSpawner.MULTIPLER;
        if(pointProgress > 1){
            PointsCounter.AddPoints(PlayerIndex.Player1, 1);
            pointProgress -= 1;
        }
    }

    protected virtual void Update() {
        if(T_Segment.Stop) return;

        AddPoints();
        ProcessMove();
        AdjustRingPosition();
    }

    private void AdjustRingPosition(){
        if(Guard.IsValid(_currentSegment)){
            Vector3 distance  = (_currentSegment.position - transform.position);
            distance.z = 0;

            Vector3 direction = distance.normalized * Time.deltaTime * _moveSpeed;
            if(direction.sqrMagnitude > distance.sqrMagnitude){
                direction = distance;
            }

            transform.position += direction;
        }
    }

    private void ProcessMove(){
        float horizontal = InputHandler.GetHorizontal();
    // /    float vertical   = InputHandler.GetVertical();

        ProcessMove_Horizontal(horizontal);
    }


    protected virtual void ProcessMove_Horizontal(float horizontal){
        if(horizontal != 0) 
            calcRotationSpeed = calcRotationSpeed + (Mathf.Sign(horizontal) *  _accelerationRotate * Time.deltaTime);
            if(Mathf.Abs(calcRotationSpeed) > _maxRotationSpeed) calcRotationSpeed = Mathf.Sign(calcRotationSpeed) * _maxRotationSpeed;

        else if(calcRotationSpeed != 0)
            calcRotationSpeed = 
                (calcRotationSpeed > 0) ?
                    Mathf.Max(calcRotationSpeed - (_rotationFriction * Time.deltaTime), 0) :
                    Mathf.Min(calcRotationSpeed + (_rotationFriction * Time.deltaTime), 0);

        transform.Rotate(new Vector3(0,0, _movePenalty*calcRotationSpeed * Time.deltaTime ));
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag.Contains("Enemy")){
            if(_invincibleSkip){
                _invincibleSkip = false;
                _shield.SetActive(false);
                return;
            }

            T_Segment.Stop = true;



            other.gameObject.SetActive(false);
            transform.GetChild(0).GetComponent<Image>().enabled = false;
            transform.GetChild(2).gameObject.SetActive(true);
            _endAnimation.SetActive(true);
            AudioSystem.PlaySample("Tunnel_Explode");
            HighScoreRanking.LoadRanking(GameType.Tunnel);

            TimersManager.Instance.FireAfter(1f, () => {
                transform.GetChild(2).gameObject.SetActive(false);
                _deathSpirit.SetActive(true);
            });




            TimersManager.Instance.FireAfter(6f, () => {
                _nextScene.OnSceneLoad();
                T_Segment.Stop = false;
                T_SegmentSpawner.MULTIPLER = 1.0f;
            });
        }

        if(other.tag.Contains("Pickup")){
            T_BonusType type = other.GetComponent<T_BonusAnimator>().GetBonusType();
            switch(type){
                case T_BonusType.Slower: 
                    T_SegmentSpawner.MULTIPLER = Mathf.Max(T_SegmentSpawner.MULTIPLER - 0.25f, 1f);

                break;
                case T_BonusType.Faster: 
                    T_SegmentSpawner.MULTIPLER = Mathf.Min(T_SegmentSpawner.MULTIPLER + 0.25f, 3f);

                break;
                case T_BonusType.Protect:
                    _invincibleSkip = true;
                    _shield.SetActive(true);
                break;
            }

            Destroy(other.gameObject);
        }
    }
}
