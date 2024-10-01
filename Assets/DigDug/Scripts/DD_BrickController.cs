using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DigDug;

public enum DD_BrickState{
    Enemy1,
    Enemy2,
    Enemy3,
    Empty,
    Full,
    Rock,
    PlayerPosition
}

public class DD_BrickController : DD_NavPoint
{
    enum BrickComponent{
        Frame,
        Center,
        Rock,
        Enemy1,
        Enemy2,
        Enemy3,
        BlockingCollider,
    }


    [SerializeField] GridLayoutGroup _layout;
    [SerializeField] Transform _brickFrame;
    [SerializeField] Transform _brickCenter;
    [SerializeField] public TextMeshProUGUI _uiGui;
    [SerializeField] public Color[] _colors;     

    protected override void Awake()
    {
        base.Awake();
        CacheChildren();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void SwitchChildren(Transform parent, bool enable){
        for(int i = 0; i < parent.childCount; i++) {
            Transform child = parent.GetChild(i);
            SwitchChildren(child, enable);
        }

        parent.gameObject.SetActive(enable);
    }

    private void SwitchFrameObj(bool enableRight, bool enableTop){
        _brickFrame.gameObject.SetActive(enableRight || enableTop);
        SwitchChildren(_brickFrame.GetChild(0), enableRight);
        SwitchChildren(_brickFrame.GetChild(1), enableTop);
    }
    private void SwitchFrameBox(bool enableRight, bool enableTop){
        SwitchBoxColliders(_brickFrame.GetChild(0), enableRight);
        SwitchBoxColliders(_brickFrame.GetChild(1), enableTop);
    }
    public void Setup(DD_BrickState brickState, bool rightEnabled, bool topEnabled){
        //Debug.Log(name + " " + brickState);
        for(int i = 0; i< transform.childCount; i++){ transform.GetChild(i).gameObject.SetActive(false); }

        //_uiGui.gameObject.SetActive(true);
        //transform.GetChild((int)BrickComponent.BlockingCollider).gameObject.SetActive(true);
        SwitchFrameObj(rightEnabled, topEnabled);
        SwitchFrameBox(rightEnabled, topEnabled);
        _layout.enabled = true;

        switch(brickState){
            case DD_BrickState.Empty:
                SwitchBoxColliders(_brickCenter, false);
                SwitchChildren    (_brickCenter, false);
            break;
            case DD_BrickState.Full: 
                SwitchBoxColliders(_brickCenter, true);
                SwitchChildren    (_brickCenter, true);
            break;
            case DD_BrickState.Rock:
                SwitchBoxColliders(_brickCenter, false);
                SwitchChildren    (_brickCenter, true);
                MakeRock(); 
            break;
            case DD_BrickState.Enemy1:
            case DD_BrickState.Enemy2:
            case DD_BrickState.Enemy3: 
                SwitchBoxColliders(_brickCenter, false);
                SwitchChildren(_brickCenter, false);
            
                MakeEnemy((int)brickState); 
            break;
            case DD_BrickState.PlayerPosition:
                SwitchBoxColliders(_brickCenter, false);
                SwitchChildren    (_brickCenter, false);
                
                DD_Player3.Instance.transform.position = transform.position;
            break;
        }

        //for(int i = 2; i < transform.childCount; i++){
        //    transform.GetChild(i).gameObject.SetActive(false);
        //}

        TimersManager.Instance.FireAfter(0.01f, () => _layout.enabled = false);
    }

    [SerializeField] GameObject[] _brickChildren = null;

    private void CacheChildren(){
        _brickChildren = new GameObject[_brickCenter.childCount];
        for(int j = 0; j < _brickCenter.childCount; j++) _brickChildren[j] = _brickCenter.GetChild(j).gameObject;
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

    public void MakeRock(){
        transform.GetChild(2).position = transform.position;
        transform.GetChild(2).gameObject.SetActive(true);

        DD_RockHandler rock = transform.GetChild(2).GetComponent<DD_RockHandler>();
        rock.Setup();
    }

    public void MakeEnemy(int id){

        DD_GameController.NumberOfEnemies ++;
        transform.GetChild(3 + id).position = transform.position;
        transform.GetChild(3 + id).gameObject.SetActive(true);
        DD_Enemy1 enemy = transform.GetChild(3+id).GetComponent<DD_Enemy1>();
        enemy.Setup();
    }

    public void SwitchBoxColliders(Transform parent, bool value){
        for(int i = 0; i < parent.childCount; i++) {
            SwitchBoxColliders(parent.GetChild(i), value);
        }

        BoxCollider2D box = parent.GetComponent<BoxCollider2D>();
        if(Guard.IsValid(box)){ box.enabled = value; }
    }

    public override float GetWalkWeight(){
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

    public void Recolor(int id, int verticalSize, int horizontalSize){
        if(id < horizontalSize) return;

        int spheres = verticalSize/_colors.Length;
        int alpha   = 0;

        for(int i = horizontalSize; i <= verticalSize*horizontalSize; i += horizontalSize){
            if(id<i){
                int colorID = Mathf.Min((int)(alpha/spheres), _colors.Length-1);

                for(int j = 0; j < _brickFrame.childCount; j++){
                    for(int k = 0; k < _brickFrame.GetChild(j).childCount; k++){
                        _brickFrame.GetChild(j).GetChild(k).GetComponent<Image>().color = _colors[colorID];
                    }
                }

                for(int j = 0; j < _brickCenter.childCount; j++){
                    _brickCenter.GetChild(j).GetComponent<Image>().color = _colors[colorID];
                }

                return;   
            }
            alpha += 1;
        }
    }

    public override void CUpdate() {
        bool shouldBeActiveMoveBlock = false;
        for(int j = 0; j < _brickChildren.Length; j++){
            shouldBeActiveMoveBlock |= _brickChildren[j].activeInHierarchy;
        }
        
    //    transform.GetChild(6).gameObject.SetActive(shouldBeActiveMoveBlock);
    }
}
