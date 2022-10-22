using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour, IListenToGameplayEvents
{

    private enum WorldType{
        Grassland,
        Magmaland,
        RedWeedLand,
        MixedLand,
    }

    private static LevelController _instance;

    [SerializeField] List<Sprite> _solidTiles1;
    [SerializeField] List<Sprite> _solidTiles2;
    [SerializeField] List<Sprite> _solidTiles3;

    private int _numberOfGroundTiles = 0;
    private Floor2[] _allFloorTiles;
    private Vector3 _distances = new Vector3(3.75f, 3.75f, 0);

    [SerializeField] WorldType type;
    private void Awake() {
        _instance = this;
        _allFloorTiles = GetComponentsInChildren<Floor2>();
    }

    void Start()
    {
        Events.Gameplay.RegisterListener(this, GameplayEventType.RecalculateTerrain);

        for(int i = 0; i < _allFloorTiles.Length; i ++) SetupNeighbours(_allFloorTiles[i]);
        SetupDiggersFloors();
        CountSolidTiles();
    }

    private void CountSolidTiles(){
        for(int i = 0; i < _allFloorTiles.Length; i++) {
            if(_allFloorTiles[i].IsSolid()) _numberOfGroundTiles++;
        }
    }

    private void SetupDiggersFloors(){
        for(int i = 0; i < _allFloorTiles.Length; i++) {
            
            _allFloorTiles[i]._spritesSolid = GetSolidTiles();
            _allFloorTiles[i].SetupDiggersFloors();
            _allFloorTiles[i].RefreshSideAndDigger();
        }
    }

    private List<Sprite> GetSolidTiles(){
        switch (type) {
            case WorldType.Grassland: return _solidTiles1;
            case WorldType.Magmaland: return _solidTiles2;
            case WorldType.RedWeedLand: return _solidTiles3;
        }

        return _solidTiles1;
    }


    private void SetupNeighbours(Floor2 current){
        Dictionary<NeighbourSide, Floor2> neighbours = new Dictionary<NeighbourSide, Floor2>();

        for(int j = 0; j < _allFloorTiles.Length; j++){
            Floor2 neighbour = _allFloorTiles[j];

            if(!neighbour.IsSolid()) continue;
            if(_distances.magnitude * 1.2f < Vector3.Distance(current.transform.position, neighbour.transform.position)) continue;

            bool canBeRight = ( Mathf.Abs((current.transform.position.x - neighbour.transform.position.x) + _distances.x) < 0.01f);
            bool canBeLeft  = ( Mathf.Abs((current.transform.position.x - neighbour.transform.position.x) - _distances.x) < 0.01f);
            bool canBeUp    = ( Mathf.Abs((current.transform.position.y - neighbour.transform.position.y) + _distances.y) < 0.01f);
            bool canBeDown  = ( Mathf.Abs((current.transform.position.y - neighbour.transform.position.y) - _distances.y) < 0.01f);

            if(canBeLeft  && !canBeUp    && !canBeDown){ neighbours[NeighbourSide.NS_Left]   = neighbour;}
            if(canBeRight && !canBeUp    && !canBeDown){ neighbours[NeighbourSide.NS_Right]  = neighbour;}
            if(!canBeLeft && !canBeRight && canBeUp   ){ neighbours[NeighbourSide.NS_Top]    = neighbour;}
            if(!canBeLeft && !canBeRight && canBeDown ){ neighbours[NeighbourSide.NS_Bottom] = neighbour;}
        }

        current.SetNeighbours(neighbours);
    }

    private bool AreStillConnected(List<Floor2> tiles, out HashSet<Floor2> connectedToFirstTile){
        connectedToFirstTile = new HashSet<Floor2>();
        List<Floor2> toCheck = new List<Floor2>{tiles[0]};

        while(toCheck.Count != 0){
            Floor2 current = toCheck[0];
            toCheck.RemoveAt(0);
            if(connectedToFirstTile.Contains(current)) continue;
            if(current == tiles[1]) return true;
            connectedToFirstTile.Add(current);

            for(NeighbourSide i = NeighbourSide.NS_Left; i <= NeighbourSide.NS_Bottom; i++) {
                if(current.IsSideLocked(i)) continue;
                if(current.Neighbours.TryGetValue(i, out Floor2 nextToCheck)){
                    if(Guard.IsValid(nextToCheck)) toCheck.Add(nextToCheck);
                }
            }
        }

        return false;
    }

    private void RecalculateTerrain(List<Floor2> ends){
        if(!AreStillConnected(ends, out HashSet<Floor2> connectedToFirstTile)){
            int firstPartTilesCount = connectedToFirstTile.Count;
            int secondPartTilesCount = _numberOfGroundTiles - connectedToFirstTile.Count;

            bool isSmallerPart = firstPartTilesCount < secondPartTilesCount;
            _numberOfGroundTiles = (isSmallerPart) ? secondPartTilesCount : firstPartTilesCount;

            for(int i = 0; i < _allFloorTiles.Length; i++) {
                Floor2 tile = _allFloorTiles[i];
                if(isSmallerPart && connectedToFirstTile.Contains(tile)) tile.ConvertToEmptyTile();
                if(!isSmallerPart && !connectedToFirstTile.Contains(tile)) tile.ConvertToEmptyTile();
            }
            Events.Gameplay.RiseEvent( new GameplayEvent(GameplayEventType.RecalculateDiggers));
            Events.Gameplay.RiseEvent( new GameplayEvent(GameplayEventType.RefreshConections));
            
            AudioSystem.Instance.PlayEffect("DigDug_Terraforming", 1);
        }
    }


    public void OnGameEvent(GameplayEvent gameplayEvent){
        if(gameplayEvent.type == GameplayEventType.RecalculateTerrain){
            RecalculateTerrain((List<Floor2>)gameplayEvent.parameter);
        }
    }

    public static Floor2 GetClosestFloor(Vector3 position){
        if(Guard.IsValid(_instance)){
            Floor2 closest = _instance._allFloorTiles[0];
            float distance = 99999;

            for(int i = 0; i < _instance._allFloorTiles.Length; i++){
                Floor2 ccc = _instance._allFloorTiles[i];
                float distance2 = Vector3.Distance(position, ccc.transform.position);
                if(distance2 < distance){
                    distance = distance2;
                    closest = ccc;
                }
            }

            return closest;
        }
        
        return null;
    }

}
