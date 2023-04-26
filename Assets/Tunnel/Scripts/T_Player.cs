using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class T_Player : MonoBehaviour, IListenToGameplayEvents
{

    [SerializeField] SceneLoader _nextScene;
    [SerializeField] float _maxRotationSpeed = 150f;
    [SerializeField] float _rotationFriction = 0.1f;
    [SerializeField] float _accelerationRotate = 0.5f;
    [SerializeField] float _moveSpeed = 4f;


    [SerializeField] float calcRotationSpeed = 0;
    [SerializeField] Vector3 movePoint;
    [SerializeField] Vector3 forcedOffset = new Vector3();

    public static int NumberOfMissles = 0;


    protected float _movePenalty = 1;
    private Transform _currentSegment;

    private float pointProgress = 0;

    private void Awake() {
        HighScoreRanking.LoadRanking(GameType.Tunnel);
        Events.Gameplay.RegisterListener(this, GameplayEventType.RecolorPlayer);
        PointsCounter.Score = 0;
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
            PointsCounter.Score += 1;
            pointProgress -= 1;
        }
    }

    private void Update() {
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
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical   = Input.GetAxisRaw("Vertical");

        ProcessMove_Horizontal(horizontal);
    }


    private void ProcessMove_Horizontal(float horizontal){
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
            T_Segment.Stop = true;

            transform.GetChild(0).GetComponent<Image>().enabled = false;
            AudioSystem.PlaySample("Tunnel_Explode");
            HighScoreRanking.LoadRanking(GameType.Tunnel);
            HighScoreRanking.TryAddNewRecord(PointsCounter.Score);
            TimersManager.Instance.FireAfter(3f, () => {
                _nextScene.OnSceneLoad();
                T_Segment.Stop = false;
                T_SegmentSpawner.MULTIPLER = 1.0f;
            });
        }
    }
}
