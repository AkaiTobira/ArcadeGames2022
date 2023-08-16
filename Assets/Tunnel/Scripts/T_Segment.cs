using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class T_Segment : MonoBehaviour
{
    [SerializeField] Vector3 scalePerTime = new Vector3(0.1f, 0.1f, 0);
    [SerializeField] float time = 1f;
    [SerializeField] float timeToCenter = 5f;
    [SerializeField] GameObject[] _colliders1;
    [SerializeField] GameObject[] _colliders2;
    [SerializeField] GameObject[] _colliders3;
    [SerializeField] float rotationSpeed = 60f;
    [SerializeField] float scaleIncreaseCoef = 1.03f;
    bool _isScaling = false;
    Vector3 _beginScale;

    float _moveToCenterTimer = 0f;
    float _scaleIncreaseValue = 1f;


    bool _setupPlayerInfo;

    public static bool Stop = false;
    public static float RotationSpeed = 0;

    int _activeScaleAtionID = -1;


    void Awake(){
        _beginScale = scalePerTime;
        RotationSpeed = rotationSpeed;
    }

    void Update()
    {
        if(Stop) {
            ScaleManager.Instance.InvalidateAction(_activeScaleAtionID);
            return;
        }

        Update_ScaleAndMove();
        transform.Rotate(new Vector3(0,0, rotationSpeed * (T_SegmentSpawner.MULTIPLER - 1.0f) * Time.deltaTime));

        Update_SetPlayerInfo();
        Update_LifeEnd();
    }

    private void Update_ScaleAndMove(){
        _moveToCenterTimer -= Time.deltaTime;
        if(!_isScaling){
            _activeScaleAtionID = ScaleManager.Instance.ScaleBy(transform, scalePerTime, time, ReFire);

            if(_moveToCenterTimer > 0){

                float numberOf_scaleIncreaseValue = timeToCenter/time;
                Vector3 move = - transform.position/numberOf_scaleIncreaseValue;
                if(time >= _moveToCenterTimer) move = - transform.position;

                TweenManager.Instance.TweenBy(transform, move, time);
            }else{
                transform.position = new Vector3();
            }

            _isScaling = true;
        }
    }

    private void Update_SetPlayerInfo(){
        if(transform.localScale.x > 25 && !_setupPlayerInfo){
            _setupPlayerInfo = true;

            Events.Gameplay.RiseEvent( 
                new GameplayEvent(
                    GameplayEventType.RecolorPlayer, 
                new KeyValuePair<Color, Transform>(
                    GetComponent<Image>().color*0.5f + Color.white*0.5f, 
                    transform)
                )
            );
        }
    }

    private void Update_LifeEnd(){
        if(transform.localScale.x > 110) {
            Destroy(gameObject);
        }
    }


    private void ReFire(){
        _isScaling = false;

        scalePerTime += new Vector3(_scaleIncreaseValue, _scaleIncreaseValue, 0);
        _scaleIncreaseValue *= scaleIncreaseCoef * T_SegmentSpawner.MULTIPLER;
    }

    public void Setup(Vector3 newPosition, float rotation, Color color, string enemiesPattern){
        transform.localScale = new Vector3(1,1,1);
        transform.Rotate(new Vector3(0,0, rotation));
        _moveToCenterTimer = timeToCenter;
        transform.position = newPosition;
        scalePerTime = _beginScale;
        _scaleIncreaseValue = 1.0f;

        GetComponent<Image>().color = color;
        SetupEnemies(enemiesPattern, color);
    }

    void SetupEnemies(string enemies, Color color){

        color = color*0.35f + Color.white*0.65f;
        for(int i =0 ; i< _colliders1.Length; i++){
            _colliders1[i].GetComponent<Image>().color = color;
            _colliders1[i].SetActive(enemies[i] == '1');
        }
        for(int i =0 ; i< _colliders2.Length; i++){
            _colliders2[i].GetComponent<Image>().color = color;
            _colliders2[i].SetActive(enemies[i] == '2');
        }
        for(int i =0 ; i< _colliders3.Length; i++){
            _colliders3[i].GetComponent<Image>().color = color;
            _colliders3[i].SetActive(enemies[i] == '3');
        }
    }
}
