using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LF_EnemyType{
    Knife,
    Rua,
    Rhino,
    KnifeThrover
}




public class LF_EnemyHPBarsController : MonoBehaviour
{
    [SerializeField] LF_EnemyHPBar[] _prefabs;

    private static LF_EnemyHPBarsController _controller;

    private List<LF_EnemyHPBar> _activePrefabs = new List<LF_EnemyHPBar>();

    private HashSet<IHasHpBar> _acitveEnemeies = new HashSet<IHasHpBar>();
    private Dictionary<IHasHpBar,LF_EnemyHPBar> _activeBars = new Dictionary<IHasHpBar,LF_EnemyHPBar>();
    private Dictionary<LF_EnemyHPBar,IHasHpBar> _reversedActiveBars = new Dictionary<LF_EnemyHPBar, IHasHpBar>();
    private Dictionary<LF_EnemyHPBar, float> _timeOfBeeingActive = new Dictionary<LF_EnemyHPBar, float>();

    private void Awake() {
        _controller = this;
    }

    public static void ShowHpBar(LF_EnemyType type, IHasHpBar item, int damage){
        if(_controller != null){
            _controller.ShowHpBarInternal(type, item, damage);
        }
    }

    public void ShowHpBarInternal(LF_EnemyType type, IHasHpBar item, int damage){
        if(_activeBars.TryGetValue(item, out LF_EnemyHPBar value)){
            value.SetupHp(item.GetCurrentHp()/(float)item.GetMaxHp());
            _timeOfBeeingActive[value] = 4.0f;
            Debug.Log("Found: Time reseted");
        }else{
            LF_EnemyHPBar bar = GetOrSpawnHpBar(type);
            bar.gameObject.SetActive(true);
            bar.SetupHp(
                (float)item.GetCurrentHp()/(float)item.GetMaxHp(),
                (float)(item.GetCurrentHp() + damage)/(float)item.GetMaxHp()
            );
            _acitveEnemeies.Add(item);
            _activeBars[item] = bar;
            _reversedActiveBars[bar] = item;
            _timeOfBeeingActive[bar] = 4.0f;
        }
    }

    private void Update() {

        string preview = "";
        for(int i = 0; i < _activePrefabs.Count; i++){
            LF_EnemyHPBar healthBar = _activePrefabs[i];
            if(healthBar.gameObject.activeSelf){
                float time = _timeOfBeeingActive[healthBar] - Time.deltaTime;
                preview +=  i + " : " + healthBar.name + " : Active : " + time;
                _timeOfBeeingActive[healthBar] = time;
                if(time < 0){
                    preview += " : Remove";
                    healthBar.gameObject.SetActive(false);
                    IHasHpBar item = _reversedActiveBars[healthBar];
                    _acitveEnemeies.Remove(item);
                    _activeBars.Remove(item);
                    _reversedActiveBars.Remove(healthBar);
                    _timeOfBeeingActive.Remove(healthBar);
                }
            }else{
                preview += i  + " : " + healthBar.name + " : Unactive";
            }
            preview += "\n";
        }

// /        Debug.Log(preview);
    }

    private LF_EnemyHPBar GetOrSpawnHpBar(LF_EnemyType type){
        for(int i = 0; i < _activePrefabs.Count; i++) {
            LF_EnemyHPBar hPBar = _activePrefabs[i];
            if(hPBar.gameObject.activeSelf) continue;
            if(hPBar.type != type) continue;

            return hPBar;
        }

        LF_EnemyHPBar hpBar1 = Instantiate(_prefabs[(int)type], new Vector3(), Quaternion.identity, transform);
        hpBar1.name += _activePrefabs.Count.ToString();
        _activePrefabs.Add(hpBar1);
        return hpBar1;
    }
}
