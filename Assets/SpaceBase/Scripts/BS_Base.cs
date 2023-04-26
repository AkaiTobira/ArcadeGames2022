using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_Base : ESM.SMC_1D<BS_TowerState>,
    ITakeDamage,
    IDealDamage,
    IUseDetector,
    IIsTrackable
{

    [SerializeField] BoxCollider2D _hitBox;
    [SerializeField] int _MaxHealthPoints = 30;
    [SerializeField] GameObject[] _parts;
    [SerializeField] GameObject   _additionalAnimation;
    [SerializeField] int _points;
    [SerializeField] GameObject _explodeAnimation;

    private int _health;

    private void Awake() {
        _health = _MaxHealthPoints;
        _hitBox.gameObject.SetActive(true);
    }


    private const float TIME_OF_ATTACK = 15f;

    public void SignalLost(MonoBehaviour item){}

    BS_Player player = null;
    public void Detected(MonoBehaviour item){

        if(player != null) return;
        player = item.GetComponent<BS_Player>();

        for(int i = 0; i < _parts.Length; i++){
            IUseDetector detector = _parts[i].GetComponent<IUseDetector>();
            if(detector != null) detector.Detected(item);
        }
    }

    float colorChangeTimer = 0.2f;
    bool isRed = false;

    protected override void UpdateState()
    {
        colorChangeTimer -= Time.deltaTime;
        if(colorChangeTimer < 0){
            if(isRed){ 
                Graphicals.color = Color.white;
                colorChangeTimer = (_health > 0) ? (_health/_MaxHealthPoints) : 999999;
                isRed = false;
            }else{
                if(_health > _MaxHealthPoints * 0.3f){
                    Graphicals.color = Color.yellow;
                    colorChangeTimer = 0.1f;
                }else{
                    Graphicals.color = Color.red;
                    colorChangeTimer = 0.2f;
                }
                isRed = true;
            }
        }

        switch(ActiveState){
            case BS_TowerState.Patrol:
            break;
            case BS_TowerState.PlayerDetected:
            break;
            case BS_TowerState.Dead: break;
        }
    }


    protected override void OnStateEnter(BS_TowerState enteredState)
    {
        switch(ActiveState){
            case BS_TowerState.Patrol: 
                player = null;
                break;
            case BS_TowerState.Dead: 
                //RequestDisable(1f);
                _hitBox.gameObject.SetActive(false);
            //    TimersManager.Instance.FireAfter(5, () => {
            //        _endScene.OnSceneLoadAsync();
            //    });
                _explodeAnimation.SetActive(true);
                PointsCounter.Score += _points;

                Debug.Log("BAse enetereed death");
                if(Guard.IsValid(_additionalAnimation)){
                    _additionalAnimation.SetActive(false);
                }

                AudioSystem.PlaySample("SpaceBase_Explode", 1, true);

                for(int i = 0; i < _parts.Length; i++) {
                    ITakeDamage damage = _parts[i].GetComponent<ITakeDamage>();
                    damage?.TakeDamage(999);
                }

            break;
        }
    }

    protected override void OnStateExit(BS_TowerState exitedState){}

    protected override BS_TowerState CheckStateTransitions()
    {

        if(_health <0) return BS_TowerState.Dead;

        switch(ActiveState){
            case BS_TowerState.Patrol:
                if(_health < 0 ) return BS_TowerState.Dead;
                else if(player != null) return BS_TowerState.PlayerDetected;
            break;
            case BS_TowerState.PlayerDetected:
                if(_health < 0 ) return BS_TowerState.Dead;
                else if(Guard.IsValid(player) && (player.transform.position - transform.position).magnitude > 20 ){
                    return BS_TowerState.Patrol;
                }
            break;
            case BS_TowerState.Dead: break;
        }

        return ActiveState;
    }

    public void TakeDamage(int amount, MonoBehaviour source = null){
        _health -= amount;
        //_playerDetectedTimer = TIME_OF_ATTACK;
        Detected(source);

        Debug.Log("Base take damamge " + amount);
    }

    public int GetDamage(){
        return 1;
    }

    public bool ShouldBeTracked(){
        return ActiveState != BS_TowerState.Dead;
    }
}