using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatics", menuName = "ScriptableObject/Enemies/EnemyStatics")]
public class EnemyStaticsScriptableObject : ScriptableObject
{
    // TODO groupe and organize the numbers better.
    public float MaxHealth ;
    public float VisionRange ;
    public float AttackRange ;
    public float DropAggroRange ;
    public float MinRoamingPause ;
    public float MaxRoamingPause ;
    public float RoamingSpeed ;
    public float ChasingSpeed ;
    public float BasicAttackDamage ;
    public float BasicAttackAnimationSpeed ;
    public float BasicAttackProjectionSpeed ;
    public float BasicAttackRotationSpeed ;
    public float SuperAttackDamage ;
    public float SuperAttackSpeed ;
    public float SuperAttackRotationSpeed ;

}
