using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DD_BrickController : CMonoBehaviour
{
    [SerializeField] GridLayoutGroup _layout;
    [SerializeField] GameObject[] _digPoints;
    [SerializeField] Transform _brickFrame;
    [SerializeField] Transform _brickCenter;
    [SerializeField] Transform _rock;
    [SerializeField] public TextMeshProUGUI _uiGui;
    


    void Start()
    {
        CallInTime( 1, () => {
            _layout.enabled = false;
        });
    }

    public void Setup(){
        EnableChildren(_brickCenter);
        EnableChildren(_brickFrame);
        SwitchBoxColliders(_brickCenter, true);
    }

    private void EnableChildren(Transform parent){
        for(int i = 0; i < parent.childCount; i++) {
            Transform child = parent.GetChild(i);
            EnableChildren(child);
        }

        parent.gameObject.SetActive(true);
    }

    public void DisableCenter(){
        _brickCenter.gameObject.SetActive(false);
    }

    public void Disable(bool up, bool right, bool center){
        _brickCenter.gameObject.SetActive(center);
        Transform frame = _brickFrame;  

        frame.GetChild(0).gameObject.SetActive(right);
        frame.GetChild(1).gameObject.SetActive(up);
    }

    public void MakeRock(){
        SwitchBoxColliders(_brickCenter, false);
        transform.GetChild(2).gameObject.SetActive(true);
    }

    public void MakeEnemy(int id){
        transform.GetChild(3 + id).gameObject.SetActive(true);
    }


    public void SwitchBoxColliders(Transform parent, bool value){
        for(int i = 0; i < parent.childCount; i++) {
            Transform child = parent.GetChild(i);
            SwitchBoxColliders(child, value);
        }

        BoxCollider2D box = parent.GetComponent<BoxCollider2D>();
        if(Guard.IsValid(box)){
            box.enabled = value;
        }
    }

    public float GetWalkWeight(){
        float value = 0;

        for(int i = 0; i < _brickFrame.childCount; i++) {
            value += _brickFrame.GetChild(i).gameObject.activeInHierarchy ? 1 : 0;
        }

        for(int i = 0; i < _brickCenter.childCount; i++) {
            value += _brickCenter.GetChild(i).gameObject.activeInHierarchy ? 1 : 0;
        }

        value += transform.GetChild(2).gameObject.activeInHierarchy ? 1000 : 0;

        return value;
    }




    public Vector3 GetClosetDigPoint(){

        float distance = 999999;
        Vector3 closestPoint = new Vector3();

        for(int i = 0; i < _digPoints.Length; i++) {
            float distance2 = Vector3.Distance(closestPoint, _digPoints[i].transform.position);
            if(distance > distance2){
                distance = distance2;
                closestPoint = _digPoints[i].transform.position;
            }
        }

        return closestPoint;
    }

    public Vector2 GetMininingPoint(ESM.AnimationSide side){
        switch(side){
            case ESM.AnimationSide.Right  : return _digPoints[1].transform.position;
            case ESM.AnimationSide.Top    : return _digPoints[0].transform.position;
            case ESM.AnimationSide.Left   : return _digPoints[3].transform.position;
            case ESM.AnimationSide.Bottom : return _digPoints[2].transform.position;
        }

        return transform.position;
    }
}
