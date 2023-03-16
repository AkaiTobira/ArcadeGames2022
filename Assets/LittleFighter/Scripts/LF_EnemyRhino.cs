using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LF_EnemyRhino : LF_EnemyCore
{
    private Vector3 _changeTargetPoint;

    static string[] customColisionObjects = new string[]{"Enviroment_Obstacle"};
    static string[] collisionObjects = new string[]{"Enviroment_Obstacle","Solid_Collider"};

    private void SwitchCollisionDetection(bool enable){
        _hitParameters[LF_EnemyBase<LF_EnemyCoreState>.RayPoints.Left].ColliderName = (enable) ? customColisionObjects : collisionObjects;
        _hitParameters[LF_EnemyBase<LF_EnemyCoreState>.RayPoints.Right].ColliderName = (enable) ? customColisionObjects : collisionObjects;
        _hitParameters[LF_EnemyBase<LF_EnemyCoreState>.RayPoints.Bottom].ColliderName = (enable) ? customColisionObjects : collisionObjects;
        _hitParameters[LF_EnemyBase<LF_EnemyCoreState>.RayPoints.Top].ColliderName = (enable) ? customColisionObjects : collisionObjects;
    }

    protected override void UpdateAttack(){
        ProcessMoveRequirements(_changeTargetPoint);
        ProcessMove(_directions * 2.3f);
    }

    protected override void OnAttackEnter(){
        HitBox.SetActive(false);
    }


    protected override void OnAttackBreakEnter(){
        SwitchCollisionDetection(false);
    }

    protected override void OnAttackPrepEnter(){
        _changeTargetPoint = LF_Player.Player.transform.position;
        Vector3 difference = _changeTargetPoint - transform.position;
        Vector3 direction  = difference.normalized;
        _changeTargetPoint = transform.position + (direction * difference.magnitude * 5);

        SwitchCollisionDetection(true);
    }
}
