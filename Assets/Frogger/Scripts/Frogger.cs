using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Frogger : MonoBehaviour
{
    public bool DoNotRespawn;

    bool isMoving;
    float movingTime = 0.2f;
    float elapsedTime = 0.0f;

    bool isDead;
    float deadTime = 3f;
    float elapsedDeadTime = 0.0f;


    bool isAlreadyHandled;
    Vector3 startInstancePosition;
    Vector3 startMovePosition;
    float FRAME_DISTANCE = 1.25f;


    [SerializeField] LayerMask obstacles; 
    [SerializeField] Sprite[] animations;

    [SerializeField] Transform LD;
    [SerializeField] Transform RU;

    Camera _camera;

    void Start(){
        _camera = Camera.main;
        startInstancePosition = transform.position;
    }


    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical   = Input.GetAxisRaw("Vertical");
        if(isMoving || isDead) return;

        if(Mathf.Abs(horizontal) > 0.05f){
            Vector3 moveDirection =  new Vector3(Mathf.Sign(horizontal), 0,0);
            MoveInternal(moveDirection);

        }else if(Mathf.Abs(vertical) > 0.05f){
            Vector3 moveDirection =  new Vector3(0, Mathf.Sign(vertical),0);
            MoveInternal(moveDirection);
        }

        if(Input.touchCount > 0) ProcessMobileInput();
    }

    private void MoveInternal(Vector3 moveDirection){
        if(isMoving || isDead) return;
        if(AdditionalWallRock(moveDirection)) return;

        if(Physics2D.Raycast(transform.position, moveDirection, FRAME_DISTANCE, obstacles))return;


        StartCoroutine(Move(moveDirection));
        RotateDirection(moveDirection);
    }

    public void MoveHorizontal(int sign){
        Vector3 moveDirection =  new Vector3(sign,0,0);
        MoveInternal(moveDirection);
    }

    public void MoveVeritcal(int sign){
        Vector3 moveDirection =  new Vector3(0, sign,0);
        MoveInternal(moveDirection);
    }

    private bool AdditionalWallRock(Vector3 direction){
        Vector3 landingPosition = transform.position + (direction * FRAME_DISTANCE);
        landingPosition = _camera.WorldToViewportPoint(landingPosition);

        if(landingPosition.x < _camera.WorldToViewportPoint(LD.position).x) return true;
        if(landingPosition.x > _camera.WorldToViewportPoint(RU.position).x) return true;
        if(landingPosition.y < _camera.WorldToViewportPoint(LD.position).y) return true;
        if(landingPosition.y > _camera.WorldToViewportPoint(RU.position).y) return true;

        return false;
    }

    private void ProcessMobileInput(){
        
        Vector3 touchPosition = _camera.ScreenToViewportPoint(Input.touches[0].position);
        Vector3 currPosition  = new Vector3(0.5f,0.5f); //_camera.WorldToViewportPoint(transform.position);

        Vector3 change        = touchPosition - currPosition;
        Vector3 moveDirection = new Vector3(
                    (Mathf.Abs(change.x) > Mathf.Abs(change.y)) ? Mathf.Sign(change.x) : 0,
                    (Mathf.Abs(change.x) < Mathf.Abs(change.y)) ? Mathf.Sign(change.y) : 0, 
                    0
                );

        MoveInternal(moveDirection);
    }

    private void RotateDirection(Vector3 direction){
            if(direction.x < -0.05f)      RotationManager.Instance.RotateTo(transform, new Vector3(0,0, 90), 0);
            else if(direction.x > 0.05f)  RotationManager.Instance.RotateTo(transform, new Vector3(0,0,270), 0);
            else if(direction.y < -0.05f) RotationManager.Instance.RotateTo(transform, new Vector3(0,0,180), 0);
            else if(direction.y > 0.05f)  RotationManager.Instance.RotateTo(transform, new Vector3(0,0,  0), 0);
    }


    IEnumerator Move(Vector3 direction){
        isMoving      = true;
        startMovePosition = transform.position;
        elapsedTime   = 0;

        if(!isDead) GetComponent<Image>().sprite = animations[4];
        AudioSystem.Instance.PlayEffect("Frogger_Move", 1, true);

        while(elapsedTime < movingTime){
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;

            transform.position = startMovePosition + (direction * FRAME_DISTANCE * elapsedTime / movingTime); 
        }


        if(!isDead) GetComponent<Image>().sprite = animations[0];
        transform.position = startMovePosition + direction * FRAME_DISTANCE;
        isMoving = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag.Contains("bstacle")){
            if(!isDead) StartCoroutine(Dead());
        }else if(other.tag.Contains("nd")){
            if(isAlreadyHandled) return;
            bool values = other.GetComponent<Target>().OnReach();
            if(values) {
                StartCoroutine(Reset());
                AudioSystem.Instance.PlayEffect("Frogger_Landing", 1, true);
            }
        }
    }

    IEnumerator Dead(){
        Events.Gameplay.RiseEvent( new GameplayEvent(GameplayEventType.PlayerDied));
        
        isDead          = true;
        elapsedDeadTime   = 0;
        AudioSystem.Instance.PlayEffect("Frogger_Dead", 1, true);

        GetComponent<Image>().sprite = animations[1];
        yield return new WaitForSeconds(0.1f);
        GetComponent<Image>().sprite = animations[2];
        yield return new WaitForSeconds(0.1f);
        GetComponent<Image>().sprite = animations[3];

        while(elapsedDeadTime < deadTime){
            yield return new WaitForEndOfFrame();
            elapsedDeadTime += Time.deltaTime;
        }
        

        transform.position = startInstancePosition;
        isDead = false;
        GetComponent<Image>().sprite = animations[0];
        AudioSystem.Instance.PlayEffect("Frogger_Lost", 1, true);
    }

    IEnumerator Reset(){
        isAlreadyHandled = true;
        while(isMoving || isDead) yield return null;
        if(DoNotRespawn) yield break;
        transform.position = startInstancePosition;
        AudioSystem.Instance.PlayEffect("Frogger_Lost", 1, true);
        isAlreadyHandled = false;
    }


}
