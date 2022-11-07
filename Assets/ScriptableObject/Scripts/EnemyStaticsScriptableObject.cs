using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatics", menuName = "ScriptableObject/Enemies/EnemyStatics")]
public class EnemyStaticsScriptableObject : ScriptableObject
{
    public string MonsterName ;
    [Space(10)]

    [Header("General")]

    public float MaxHealth ;
    public float VisionRange ;
    //public float DropAggroRange = VisionRange * 1.5f ;
    public float AttackRange ;
    public float MinRoamingPause ;
    public float MaxRoamingPause ;
    public float RoamingSpeed ;
    public float ChasingSpeed ;

    [Space(10)]

    [Header("Basic Attack")]

    public float BasicAttackDamage ;
    public float BasicAttackAnimationSpeed ;
    public float BasicAttackRotationSpeed ;
    public float BasicAttackProjectionSpeed ;

    [Space(10)]

    [Header("Super Attack")]

    public float SuperAttackDamage ;
    public float SuperAttackAnimationSpeed ;
    public float SuperAttackRotationSpeed ;

}
