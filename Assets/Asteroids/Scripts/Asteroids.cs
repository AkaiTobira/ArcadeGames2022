using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WallThrought : MonoBehaviour{

    [SerializeField] protected bool EnableWallTeleport = true;

    virtual protected void Update() {
        if(!EnableWallTeleport) return;
        if(!TeleportationBorders.Left) return;

        if(transform.position.x < TeleportationBorders.Left.position.x){
            transform.position = new Vector3(TeleportationBorders.Right.transform.position.x, transform.position.y, transform.position.z);
        }
        else if(transform.position.x > TeleportationBorders.Right.transform.position.x){
            transform.position = new Vector3(TeleportationBorders.Left.position.x, transform.position.y, transform.position.z);
        }

        if(transform.position.y < TeleportationBorders.Bottom.position.y){
            transform.position = new Vector3(transform.position.x, TeleportationBorders.Top.transform.position.y, transform.position.z);
        }
        else if(transform.position.y > TeleportationBorders.Top.transform.position.y){
            transform.position = new Vector3(transform.position.x, TeleportationBorders.Bottom.position.y, transform.position.z);
        }
    }
}




public class Asteroids : WallThrought
{
    public bool DoNotRespawn;

    bool isMoving;

    bool isDead;
    //float deadTime = 3f;
    //float elapsedDeadTime = 0.0f;
    float FRAME_DISTANCE = 1.25f;


    [SerializeField] float _maxSpeed = 10f;
    [SerializeField] float _maxRotationSpeed = 150f;

    [SerializeField] float _spaceMoveFriction = 0.1f;
    [SerializeField] float _spaceRotationFriction = 0.1f;

    [SerializeField] float _accelerationMove = 0.5f;
    [SerializeField] float _accelerationRotate = 0.5f;


    [SerializeField] float calcMoveSpeed = 0;
    [SerializeField] float calcRotationSpeed = 0;

    [SerializeField] Vector3 _savedThrustVector;

    [SerializeField] GameObject _missle;
    [SerializeField] Transform _shipHead;

    [SerializeField] LayerMask obstacles; 
    [SerializeField] Sprite[] animations;


    protected float _movePenalty = 1;

    public static int NumberOfMissles = 0;
    Camera _camera;

    void Start(){
        _camera = Camera.main;
    }


    override protected void Update()
    {
        base.Update();
        ProcessMove();
        ProccesShooting();
    }

    private void ProcessMove(){
        float horizontal = -Input.GetAxisRaw("Horizontal");
        float vertical   = Input.GetAxisRaw("Vertical");

        ProcessMove_Horizontal(horizontal);
        ProcessMove_Vertical(vertical);
    }


    private void ProcessMove_Horizontal(float horizontal){
        if(horizontal != 0) 
            calcRotationSpeed = calcRotationSpeed + (Mathf.Sign(horizontal) *  _accelerationRotate * Time.deltaTime);
            if(Mathf.Abs(calcRotationSpeed) > _maxRotationSpeed) calcRotationSpeed = Mathf.Sign(calcRotationSpeed) * _maxRotationSpeed;

        else if(calcRotationSpeed != 0)
            calcRotationSpeed = 
                (calcRotationSpeed > 0) ?
                    Mathf.Max(calcRotationSpeed - (_spaceRotationFriction * Time.deltaTime), 0) :
                    Mathf.Min(calcRotationSpeed + (_spaceRotationFriction * Time.deltaTime), 0);

        transform.Rotate(new Vector3(0,0, _movePenalty*calcRotationSpeed * Time.deltaTime ));
    }

    private void ProcessMove_Vertical(float vertical){
        if(vertical > 0){
            _savedThrustVector = transform.up;
            calcMoveSpeed = Mathf.Min(calcMoveSpeed +  _movePenalty*(_accelerationMove * Time.deltaTime), _movePenalty*_maxSpeed);
        }
        else 
            calcMoveSpeed = Mathf.Max(calcMoveSpeed - _movePenalty*(_spaceMoveFriction * Time.deltaTime), 0);

        transform.position += _savedThrustVector * calcMoveSpeed * Time.deltaTime * FRAME_DISTANCE;
    }

    private void ProccesShooting(){
        if(Input.GetKeyDown(KeyCode.N)){
            Shoot();
        }
    }

    private void Shoot(){
        if(NumberOfMissles < 5){
            
            Missle missle = Instantiate(_missle, transform.position + transform.up, Quaternion.identity).GetComponent<Missle>();
            missle.Setup(transform.up);
            (missle.transform as RectTransform).SetParent(transform.parent);

            transform.position -= transform.up * 15 * Time.deltaTime * FRAME_DISTANCE;
        }
    }

    public void OnLeftButtonPressed(){
        if(enabled){
            ProcessMove_Horizontal(1);
        }
    }

    public void OnRightButtonPressed(){
        if(enabled){
            ProcessMove_Horizontal(-1);
        }
    }

    public void OnThrustPressed(){
        if(enabled){
            ProcessMove_Vertical(1);
        }
    }

    public void OnShootPressed(){
        if(enabled){
            Shoot();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if(other.tag.Contains("cle")){
        //    Debug.LogWarning("Shit");
        //    Debug.Log(other.tag);
        }
    }
}
