using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [System.Serializable]
    public class KeyValuePairs{
        public KeyValuePairs(){}
        public KeyValuePairs(bool key, float value){
            enable = key;
            position = value;
        }
        public bool enable;
        public float position;
    }

    [SerializeField] private Vector3 centerOfCamera      = new Vector3(0,0,0);
    [SerializeField] private float zoomOfCamera          = 0;
    [SerializeField] private Transform followedObject    = null;
    [SerializeField] public  KeyValuePairs LeftClamping  = new KeyValuePairs(false, 0);
    [SerializeField] public KeyValuePairs RightClamping = new KeyValuePairs(false, 0);
    [SerializeField] public KeyValuePairs TopClamping   = new KeyValuePairs(false, 0);
    [SerializeField] public KeyValuePairs DownClamping  = new KeyValuePairs(false, 0);
    [SerializeField] private float m_smoothTime = 10.0f;
    [SerializeField] private float m_smoothCenterMoveTime = 10.0f;
    [SerializeField] private float m_smoothZoomTime = 10.0f;
    Vector3 velocity  = Vector3.zero;
    Vector3 velocity2  = Vector3.zero;
    float zoomSpeed  = 0;

    private Vector3 additonalModificator = new Vector3();
    public float defaultSize = 0;

    [HideInInspector] public float additionalCameraSmoothTime = 0;

    Vector3 targetCenter = Vector3.zero;

    public static CameraFollow Instance;


    void Start() {
        Instance = GetComponent<CameraFollow>();
        
        targetCenter = centerOfCamera;
        zoomOfCamera = GetComponent<Camera>().orthographicSize;
        defaultSize  = GetComponent<Camera>().orthographicSize;
    }

    public void SetValues( KeyValuePairs left, KeyValuePairs right, KeyValuePairs up, KeyValuePairs down, Vector3 centerOfCamera = new Vector3() ,  float zoom = 0 ){
        LeftClamping  = left;
        RightClamping = right;
        TopClamping   = up;
        DownClamping  = down;

        if( !centerOfCamera.Equals( new Vector3() ) ) targetCenter = centerOfCamera; 
        if( zoom != 0) zoomOfCamera = zoom;
    }

    public void SetNewFollowable( Transform obj){
        followedObject = obj;
    }

    float GetXPosition(){
        float basePosition = followedObject.position.x - centerOfCamera.x;
        float minValue = (LeftClamping.enable)  ? LeftClamping.position  : basePosition;
        float maxValue = (RightClamping.enable) ? RightClamping.position : basePosition;
        return Mathf.Clamp( basePosition, minValue, maxValue);
    }

    float GetYPosition(){
        float basePosition = followedObject.position.y - centerOfCamera.y;
        float minValue = (DownClamping.enable) ? DownClamping.position : basePosition;
        float maxValue = (TopClamping.enable)  ? TopClamping.position  : basePosition;
        return Mathf.Clamp( basePosition, minValue, maxValue);
    }

    void DragCenterOfCamera(){
        centerOfCamera = Vector3.SmoothDamp( centerOfCamera, targetCenter, ref velocity2, m_smoothCenterMoveTime );
    }

    void DragZoomOfCamera(){
        GetComponent<Camera>().orthographicSize   = Mathf.SmoothDamp( GetComponent<Camera>().orthographicSize, zoomOfCamera, ref zoomSpeed, m_smoothZoomTime);
    }


    void Update(){
        DragCenterOfCamera();
        DragZoomOfCamera();

        Vector3 targetPosition = followedObject.position;
        targetPosition.z   = -20;//transform.position.z + centerOfCamera.z;
        targetPosition.x   = GetXPosition();
        targetPosition.y   = GetYPosition();

//        targetPosition    += additonalModificator;

        transform.position = Vector3.SmoothDamp( transform.position, targetPosition + additonalModificator, ref velocity, m_smoothTime+ additionalCameraSmoothTime);
    }
    public void SetAdditionalModification( Vector3 modif){
        additonalModificator = modif;
    }

    private bool holdAdditionalSmooth = false;

    public void EnableMoreSmooth(float additionalSmooth){
        additionalCameraSmoothTime = additionalSmooth;
        holdAdditionalSmooth = true;
    }

    public void DisableMoreSmooth(){
        holdAdditionalSmooth = false;
        StartCoroutine(ClearAddtionalSmooth());
    }

    IEnumerator ClearAddtionalSmooth(){
        yield return new WaitForSeconds( (m_smoothTime + additionalCameraSmoothTime) * 2 );
        if( holdAdditionalSmooth ) yield break;
        additionalCameraSmoothTime = 0;
    }


    public static void SetupSize(float size){
        Instance.zoomOfCamera = size;
    }

    public static void Clear(){
        Instance.zoomOfCamera = Instance.defaultSize;
    }

}
