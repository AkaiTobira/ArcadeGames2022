using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GTerrainType{
    GTT_Empty,
    GTT_Solid,
}

public enum NeighbourSide{
    NS_Left,
    NS_Right,
    NS_Top,
    NS_Bottom
}

public enum DiggersSpawnPositions{
    DSP_Left_Down,
    DSP_Left_Up,
    DSP_Right_Down,
    DSP_Right_Up
}


[ExecuteInEditMode]
public class Floor2 : MonoBehaviour, IListenToGameplayEvents
{
    [SerializeField] private GTerrainType _terrainType;
    [SerializeField] private List<NeighbourSide> _lockedSides = new List<NeighbourSide>();
    [SerializeField] private List<DiggersSpawnPositions> _diggersPositions = new List<DiggersSpawnPositions>();
    
    [SerializeField] public List<Sprite> _spritesSolid = new List<Sprite>();
    [SerializeField] private List<Sprite> _spritesEmpty = new List<Sprite>();

    [SerializeField] GameObject _digger;
    [SerializeField] GameObject[] _lockVisual;
    [SerializeField] bool _bindPosition = false;
    [SerializeField] bool _repopulateProperties = false;
    [SerializeField] List<Floor2> _tempToSee = new List<Floor2>();

    public Dictionary<NeighbourSide, Floor2> Neighbours = new Dictionary<NeighbourSide, Floor2>();
    
    //Depends on scale in editor
    const float TILE_SIZE = 3.75f;

    private void Start() {
        Events.Gameplay.RegisterListener(this, GameplayEventType.RefreshConections);

        if(_terrainType == GTerrainType.GTT_Empty) {
            ConvertToEmptyTile();
            GetComponent<Image>().sprite = _spritesEmpty[Random.Range(0, _spritesEmpty.Count)];
        }
        if(_terrainType == GTerrainType.GTT_Solid) {
            GetComponent<BoxCollider2D>().enabled = false;
        //    GetComponent<Image>().sprite = _spritesSolid[Random.Range(0, _spritesSolid.Count)];
        }
        EnableSides();
        
        _lockVisual[0].GetComponent<Image>().fillAmount = (_lockVisual[0].activeSelf) ? 1 : 0;
        _lockVisual[1].GetComponent<Image>().fillAmount = (_lockVisual[1].activeSelf) ? 1 : 0;
    }

    private void ResetUpSolidTile(){
        if(_terrainType == GTerrainType.GTT_Solid) {
            GetComponent<Image>().sprite = _spritesSolid[Random.Range(0, _spritesSolid.Count)];
        }
    }

    public void RefreshSideAndDigger(){
        ResetUpSolidTile();
        foreach( NeighbourSide side in _lockedSides){
            LockSideInternal(side);
        }

        (Floor2, Floor2) neighbors = ReconstructNeighbours(this);
        SetDiggers(neighbors);
        RepopulateProperites();
    }

    public bool IsSolid(){
        return _terrainType == GTerrainType.GTT_Solid;
    }

    public bool IsSideLocked(NeighbourSide side){
        return _lockedSides.Contains(side);
    }

    public void LockSideInternal(NeighbourSide side){
        if(!IsSideLocked(side))_lockedSides.Add(side);
        if(Neighbours.TryGetValue(side, out Floor2 neighbour)){
            if(!neighbour.IsSideLocked(ReverseSide(side)))
                neighbour._lockedSides.Add(ReverseSide(side));
        }

        EnableSides();
    }

    bool isLeftSet;
    bool isTopSet;

    public void LockSide(NeighbourSide sideToBeLocked, NeighbourSide playerSide){

        if(!isLeftSet && sideToBeLocked == NeighbourSide.NS_Left){
            //Setup up/down break looking from digger perspective; 
            ImageFiller left = _lockVisual[0].GetComponent<ImageFiller>();
            bool lefSide   = (sideToBeLocked == NeighbourSide.NS_Left) && (playerSide == NeighbourSide.NS_Top);
            left.Setup(1, (lefSide) ? FillOrigin.Left : FillOrigin.Right );
            isLeftSet = true;
        }

        if(!isTopSet && sideToBeLocked == NeighbourSide.NS_Top){
            ImageFiller top  = _lockVisual[1].GetComponent<ImageFiller>();
            bool rightSide = (sideToBeLocked == NeighbourSide.NS_Top)  && (playerSide == NeighbourSide.NS_Right);
            top.Setup (1, (rightSide) ? FillOrigin.Right : FillOrigin.Left);
            isTopSet = true;
        }

        LockSideInternal(sideToBeLocked);
    }

    public void SetupDiggersFloors(){
        if(!IsSolid()) return;
        _digger.GetComponent<Digger>().Setup(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsSolid()){
            Neighbours.TryGetValue(NeighbourSide.NS_Left, out Floor2 f1);
            Neighbours.TryGetValue(NeighbourSide.NS_Right, out Floor2 f2);
            Neighbours.TryGetValue(NeighbourSide.NS_Top, out Floor2 f3);
            Neighbours.TryGetValue(NeighbourSide.NS_Bottom, out Floor2 f4);

            _tempToSee = new List<Floor2>{
                f1,f2,f3,f4
            };
        }

        #if UNITY_EDITOR
            if(_repopulateProperties) RepopulateProperites();
        //    if(_bindPosition) BindPositions();
        #endif
    }

    private void BindPositions(){

        Vector3 newPosition = (transform.position - new Vector3(TILE_SIZE*0.5f, TILE_SIZE*0.5f, 0)) /TILE_SIZE;
        newPosition.x = Mathf.Ceil(newPosition.x);
        newPosition.y = Mathf.Ceil(newPosition.y);
        newPosition.z = Mathf.Ceil(newPosition.z);
        newPosition *= TILE_SIZE;
        transform.position = newPosition;

        _bindPosition = false;
    }

    private void SetBreaks((Floor2, Floor2) neighbors){
        if(Guard.IsValid(neighbors.Item1)){
            if(!neighbors.Item1.IsSideLocked(NeighbourSide.NS_Left)){
                if(IsSideLocked(NeighbourSide.NS_Right)){
                    neighbors.Item1.LockSideInternal(NeighbourSide.NS_Left);
                    neighbors.Item1.RepopulateProperites();
                }
            }
        }

        if(Guard.IsValid(neighbors.Item2)){
            if(!neighbors.Item2.IsSideLocked(NeighbourSide.NS_Top)){
                if(IsSideLocked(NeighbourSide.NS_Bottom)){
                    neighbors.Item2.LockSideInternal(NeighbourSide.NS_Top);
                    neighbors.Item2.RepopulateProperites();
                }
            }
        }
    }

// R, B
    private void SetDiggers((Floor2, Floor2) neighbors){

        for(int i = 0; i < _diggersPositions.Count; i++) {
            switch (_diggersPositions[i]) {
            case DiggersSpawnPositions.DSP_Left_Up:
                _digger.gameObject.SetActive(true);
                break;
            case DiggersSpawnPositions.DSP_Left_Down:
                if(Guard.IsValid(neighbors.Item2)){
                    neighbors.Item2._digger.gameObject.SetActive(true);
                }
                break;
            case DiggersSpawnPositions.DSP_Right_Down:
                if(Guard.IsValid(neighbors.Item1)){
                    (Floor2, Floor2) neigbours2 = ReconstructNeighbours(neighbors.Item1);
                    if(Guard.IsValid(neigbours2.Item2)){
                        neigbours2.Item2._digger.gameObject.SetActive(true);
                    }
                }
                break;
            case DiggersSpawnPositions.DSP_Right_Up:
                if(Guard.IsValid(neighbors.Item1)){
                    neighbors.Item1._digger.gameObject.SetActive(true);
                }
                break;
            }
        }
    }

    public void OnGameEvent(GameplayEvent gameplayEvent){

        if(enabled == false) return;

        if(gameplayEvent.type == GameplayEventType.RefreshConections){
            _digger.GetComponent<Digger>().DisableIfAnyTileIsEmpty();
            EnableSides();
        }
    }



    private void RepopulateProperites(){
        if(!IsSolid()){
            SetToEmpty();
            return;
        }

        (Floor2, Floor2) neighbors = ReconstructNeighbours(this);
        SetBreaks(neighbors);
        SetDiggers(neighbors);
        EnableSides();

        _repopulateProperties = false;
    }

    public void EnableSides(){
        _lockVisual[0].gameObject.SetActive(IsSideLocked(NeighbourSide.NS_Left));
        _lockVisual[1].gameObject.SetActive(IsSideLocked(NeighbourSide.NS_Top));
    }

    //public void OnDrawGizmosSelected() {
    //    if(!IsSolid()){
    //        _lockVisual[0].gameObject.SetActive(false);
    //        _lockVisual[1].gameObject.SetActive(false);
    //        return;
    //    }

    //    EnableSides();
    //}

    public void SetToEmpty(){

        if(_terrainType != GTerrainType.GTT_Empty){
            GetComponent<Image>().sprite = _spritesEmpty[Random.Range(0, _spritesEmpty.Count)];
        }

        _terrainType = GTerrainType.GTT_Empty;

        
        //GetComponent<Image>().color = _inactiveColor;

        _lockedSides.Clear();

        _lockVisual[0].gameObject.SetActive(false);
        _lockVisual[1].gameObject.SetActive(false);

        _digger.gameObject.SetActive(false);
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void SetNeighbours(Dictionary<NeighbourSide, Floor2> neighbours){ 
        Neighbours = neighbours; 
//        Debug.Log("Neighours set :" + name);
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

    public void ConvertToEmptyTile(){
        for(NeighbourSide i = NeighbourSide.NS_Left; i <= NeighbourSide.NS_Bottom; i++) {
            if(Neighbours.TryGetValue(i, out Floor2 neighbour)){
                if(Guard.IsValid(neighbour)){
                    neighbour.Neighbours[ReverseSide(i)] = null;
                    neighbour._lockedSides.Remove(ReverseSide(i));
                    neighbour.EnableSides();
                }
            }
        }
        SetToEmpty();
    }

    private (Floor2, Floor2) ReconstructNeighbours( Floor2 current){

        (Floor2, Floor2) neighbour = (null, null);

        Vector3 distances = new Vector3(TILE_SIZE, TILE_SIZE, 0);

        Floor2[] floors = transform.parent.GetComponentsInChildren<Floor2>();

        for(int j = 0; j < floors.Length; j++){
            Floor2 canBeNeighbour = floors[j];
            if(current == canBeNeighbour) continue;
            if(!canBeNeighbour.IsSolid()) continue;
            if(distances.magnitude * 1.2f < Vector3.Distance(current.transform.position, canBeNeighbour.transform.position)) continue;

            bool right = ( Mathf.Abs((current.transform.position.x - canBeNeighbour.transform.position.x) + distances.x) < 0.01f);
            bool left  = ( Mathf.Abs((current.transform.position.x - canBeNeighbour.transform.position.x) - distances.x) < 0.01f);
            bool up    = ( Mathf.Abs((current.transform.position.y - canBeNeighbour.transform.position.y) + distances.y) < 0.01f);
            bool down  = ( Mathf.Abs((current.transform.position.y - canBeNeighbour.transform.position.y) - distances.y) < 0.01f);

            if(right && !up    && !down){ neighbour.Item1 = canBeNeighbour;}
            if(!left && !right && down ){ neighbour.Item2 = canBeNeighbour;}
        }

        return neighbour;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        
        if(other.tag.Contains("yer")){
            other.GetComponent<DigDugger>().SetupBlink(true);
        }

        if(other.tag.Contains("cle")){
            other.GetComponent<Enemy1>().SetupBlink(true);
        }

    }


    private void OnTriggerExit2D(Collider2D other) {
        
        if(other.tag.Contains("yer")){
            other.GetComponent<DigDugger>().SetupBlink(false);
        }

        if(other.tag.Contains("cle")){
            other.GetComponent<Enemy1>().SetupBlink(false);
        }
    }

}
