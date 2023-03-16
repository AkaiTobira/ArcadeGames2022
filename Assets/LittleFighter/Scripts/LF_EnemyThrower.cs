using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LF_EnemyThrower : LF_EnemyCore
{
    [SerializeField] GameObject missle;
    [SerializeField] Transform misslebegin;
    [SerializeField] float _shotDelay;

    protected override void OnAttackEnter()
    {
        TimersManager.Instance.FireAfter(_shotDelay, Shoot);
    }

    private void Shoot(){

        LF_ColliderSide side = Instantiate( missle, misslebegin.position, Quaternion.identity, transform.parent).GetComponent<LF_ColliderSide>();
        side.SetParent(this);

        Vector3 direction = (LF_Player.Player.transform.position - transform.position).normalized;
        direction.x = Mathf.Sign(direction.x);
        direction.y = 0;
        side.GetComponent<BS_Missle>().Setup(direction);
    }

    public override int GetDamage()
    {
        base.GetDamage();
        return stats.Damage;
    }
}
