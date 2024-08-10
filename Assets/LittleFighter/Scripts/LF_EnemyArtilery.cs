using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LF_EnemyArtilery : LF_EnemyCore
{
    [SerializeField] GameObject missle;
    [SerializeField] GameObject altilerySpawnPoint;
    [SerializeField] Transform misslebegin;
    [SerializeField] float _shotDelay;

    protected override void OnAttackEnter()
    {
        TimersManager.Instance.FireAfter(_shotDelay, Shoot);
    }

    private void Shoot(){

        LF_ColliderSide side = Instantiate( missle, misslebegin.position, Quaternion.identity, transform.parent).GetComponent<LF_ColliderSide>();
        side.SetParent(this);
        side.GetComponent<BS_Missle>().Setup(Vector3.up);
        side.GetComponent<CircleCollider2D>().enabled = false;
    
        Vector3 targetPos = GetCloserPlayer();
        Instantiate(altilerySpawnPoint, targetPos, Quaternion.identity, transform.parent.parent);
    }

    public override float GetDamage()
    {
        base.GetDamage();
        return stats.Damage;
    }
}
