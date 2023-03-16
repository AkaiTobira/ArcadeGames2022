using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LF_EnemyStats", order = 1)]
public class LF_EnemyStats : ScriptableObject
{
    [SerializeField] public LF_EnemyType Type;
    [SerializeField] public int MaxHealthPoints = 10;
    [SerializeField] public int AdditionaHpPerLevel = 2;
    [SerializeField] public int Damage = 1;
    [SerializeField] public int PointsAquire = 50;

    [SerializeField] public float HitTimeDuration = 0.5f;
    [SerializeField] public float MoveTimeDuration = 0.5f;
    
    [SerializeField] public float AttackTimeDuration = 0.5f;
    [SerializeField] public float AttackHitboxActivationTime = 0.5f;
    [SerializeField] public float AttackPreparationTimeDelay = 0.5f;
    [SerializeField] public float AttackBreakTimeDelay = 0.5f;

    [SerializeField] public string[] hitSounds;
    [SerializeField] public string[] hurtSounds;
    [SerializeField] public string[] attackSounds;
    [SerializeField] public string[] afterAttackSound;
    [SerializeField] public string[] beforeAttackSound;
}
