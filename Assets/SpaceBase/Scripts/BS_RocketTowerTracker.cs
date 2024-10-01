using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_RocketTowerTracker : CMonoBehaviour
{
    public static BS_RocketTowerTracker Instance = new BS_RocketTowerTracker();
    HashSet<Transform> _damagable = new HashSet<Transform>();
    [SerializeField] List<Transform> _damagable2 = new List<Transform>();
    [SerializeField] List<GameObject> _trackery = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(!Guard.IsValid(other)) return;

        LF_ColliderSide side = other.GetComponent<LF_ColliderSide>();
        if(Guard.IsValid(side) && Guard.IsValid(side.GetParent())){
            if(side.GetParent().CompareTag("Player")) return;
            if(!side.CompareTag("HitBox")) return;
            ICanBeRocketTarget damagable = side.GetParent().GetComponent<ICanBeRocketTarget>();
            if(Guard.IsValid(damagable) && !_damagable.Contains(side.GetParent().transform)){
                _damagable.Add(side.GetParent().transform);
                _damagable2.Add(side.GetParent().transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(!Guard.IsValid(other)) return;

        LF_ColliderSide side = other.GetComponent<LF_ColliderSide>();
        if(Guard.IsValid(side) && Guard.IsValid(side.GetParent())){
            if(side.GetParent().CompareTag("Player")) return;
            if(!side.CompareTag("HitBox")) return;
            ICanBeRocketTarget damagable = side.GetParent().GetComponent<ICanBeRocketTarget>();
            if(Guard.IsValid(damagable) && _damagable.Contains(side.GetParent().transform)){
                _damagable.Remove(side.GetParent().transform);
                _damagable2.Remove(side.GetParent().transform);
            }
        }
    }

    public static bool HasTransform(Transform t) { return Instance._damagable.Contains(t); }
    public static List<Transform> GetTargets(){ return Instance._damagable2; }
    public static HashSet<Transform> GetTarget2s(){ return Instance._damagable; }
    public static List<GameObject> GetTracers() { return Instance._trackery; }
}
