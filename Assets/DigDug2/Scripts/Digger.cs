using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Digger : MonoBehaviour, IListenToGameplayEvents
{

    class SideCorelations{
        public DiggersSpawnPositions Tile1;
        public NeighbourSide LockSideForTile1;
        public DiggersSpawnPositions Tile2;
        public NeighbourSide LockSideForTile2;
    }

    static Dictionary<NeighbourSide, SideCorelations> constSiteValues = new Dictionary<NeighbourSide, SideCorelations>{
        { NeighbourSide.NS_Left, 
            new SideCorelations{
                Tile1 = DiggersSpawnPositions.DSP_Right_Up, 
                Tile2 = DiggersSpawnPositions.DSP_Right_Down, 
                LockSideForTile1 = NeighbourSide.NS_Bottom, 
                LockSideForTile2 = NeighbourSide.NS_Top}},
        { NeighbourSide.NS_Right, 
            new SideCorelations{
                Tile1 = DiggersSpawnPositions.DSP_Left_Up, 
                Tile2 = DiggersSpawnPositions.DSP_Left_Down, 
                LockSideForTile1 = NeighbourSide.NS_Bottom, 
                LockSideForTile2 = NeighbourSide.NS_Top}},
        { NeighbourSide.NS_Top, 
            new SideCorelations{
                Tile1 = DiggersSpawnPositions.DSP_Left_Down, 
                Tile2 = DiggersSpawnPositions.DSP_Right_Down, 
                LockSideForTile1 = NeighbourSide.NS_Right, 
                LockSideForTile2 = NeighbourSide.NS_Left}},
        { NeighbourSide.NS_Bottom, 
            new SideCorelations{
                Tile1 = DiggersSpawnPositions.DSP_Left_Up, 
                Tile2 = DiggersSpawnPositions.DSP_Right_Up, 
                LockSideForTile1 = NeighbourSide.NS_Right, 
                LockSideForTile2 = NeighbourSide.NS_Left}},
        
    };


    private Dictionary<DiggersSpawnPositions, Floor2> _corresponingTiles = new Dictionary<DiggersSpawnPositions, Floor2>(); 
    bool _isPlayerInside = false;
    DigDugger _player = null;



    void Start(){
        Events.Gameplay.RegisterListener(this, GameplayEventType.RecalculateDiggers);
    }

    public void Setup(Floor2 parentTile){
        if(!gameObject.activeInHierarchy) return;

        if(!parentTile.Neighbours.ContainsKey(NeighbourSide.NS_Top) || !parentTile.Neighbours.ContainsKey(NeighbourSide.NS_Left)){
            gameObject.SetActive(false);
            return;
        }

        _corresponingTiles[DiggersSpawnPositions.DSP_Right_Down] = parentTile;
        _corresponingTiles[DiggersSpawnPositions.DSP_Right_Up]   = parentTile.Neighbours[NeighbourSide.NS_Top];
        _corresponingTiles[DiggersSpawnPositions.DSP_Left_Down]  = parentTile.Neighbours[NeighbourSide.NS_Left];
        _corresponingTiles[DiggersSpawnPositions.DSP_Left_Up]    = parentTile.Neighbours[NeighbourSide.NS_Left]?.Neighbours[NeighbourSide.NS_Top];
    }

    void Update()
    {
        Dig();
    }

    private void Dig(){
        if(_isPlayerInside){
            if(Guard.IsValid(_player)){
                if(_player.IsDigging()){
                    DigNewBreak(ReverseSide(_player.GetFacingDirection()));
                    _player.SetupFixedDigging(true, transform as RectTransform);

                //    TimersManager.Instance.FireAfter(CONSTS.DIGGING_TIME, () => FinishDigging());
            //    }
                }
            }
        }
    }

    private void FinishDigging(){
        if(_isPlayerInside){
            if(Guard.IsValid(_player)) _player.SetupFixedDigging(false, transform as RectTransform);
        }
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){
        if(!gameObject.activeSelf) return; 

        if( gameplayEvent.type == GameplayEventType.RecalculateDiggers){
            DisableIfAnyTileIsEmpty();
        }
    }

    public void DisableIfAnyTileIsEmpty(){
        for(DiggersSpawnPositions i = DiggersSpawnPositions.DSP_Left_Down; i <= DiggersSpawnPositions.DSP_Right_Up ; i++) {
            if(_corresponingTiles.TryGetValue(i, out Floor2 value)){
                if(Guard.IsValid(value)){
                    if(value.IsSolid()) continue;
                }
            }
            gameObject.SetActive(false);
            return;
        }
    }

    private void OnEnable() {
        DiggersCounter.ActiveDiggers += 1;
    }

    private void OnDisable() {
        if(_isPlayerInside) FinishDigging();
        DiggersCounter.ActiveDiggers -= 1;
    }

    private void DigNewBreak(NeighbourSide side){

        SideCorelations corelations = constSiteValues[side];

        Floor2 tile1 = _corresponingTiles[corelations.Tile1];
        Floor2 tile2 = _corresponingTiles[corelations.Tile2];

        if(Guard.IsValid(tile1) && Guard.IsValid(tile2)){
            if( !tile1.IsSideLocked(corelations.LockSideForTile1) && 
                !tile2.IsSideLocked(corelations.LockSideForTile2)){

                tile1.LockSide(corelations.LockSideForTile1, side);
                tile2.LockSide(corelations.LockSideForTile2, side);

                TimersManager.Instance.FireAfter(CONSTS.DIGGING_TIME, () => 
                    {
                        Events.Gameplay.RiseEvent(
                        new GameplayEvent(
                            GameplayEventType.RecalculateTerrain,  
                            new List<Floor2>{tile1, tile2}));
                    }
                );
            }
        }
    }

    private NeighbourSide CalculateSide(){

        float horizontal = transform.position.x - _player.transform.position.x;
        float vertical   = transform.position.y - _player.transform.position.y;

        if(Mathf.Abs(horizontal) > Mathf.Abs(vertical)){
            return horizontal > 0 ? NeighbourSide.NS_Left : NeighbourSide.NS_Right;
        }

        return vertical < 0 ? NeighbourSide.NS_Top : NeighbourSide.NS_Bottom;
    }

    NeighbourSide ReverseSide(NeighbourSide side){
        switch (side) {
            case NeighbourSide.NS_Left:   return NeighbourSide.NS_Right;
            case NeighbourSide.NS_Right:  return NeighbourSide.NS_Left;
            case NeighbourSide.NS_Top:    return NeighbourSide.NS_Bottom;
            case NeighbourSide.NS_Bottom: return NeighbourSide.NS_Top;
        }
    
        return side;
    }

    private void OnTriggerEnter2D(Collider2D other) {
//        Debug.Log(other.name);
        if(other.gameObject.tag.Contains("yer")){
            _isPlayerInside = true;
            _player = other.GetComponent<DigDugger>();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag.Contains("yer")){
            _isPlayerInside = false;
            _player = other.GetComponent<DigDugger>();
        }
    }


}
