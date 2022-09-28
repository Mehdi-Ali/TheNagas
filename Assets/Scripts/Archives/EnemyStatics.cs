using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatics : MonoBehaviour
{
    public float MaxHealth = 100.0f;
    public float VisionRange = 10.0f;
    public float AttackRange = 2.0f;
    public float DropAggroRange = 20.0f;
    public float MinRoamingPause = 3.0f;
    public float MaxRoamingPause = 6.0f;
    public float RoamingSpeed =  1.5f ;
    public float ChasingSpeed =  3.5f ;
    public float BasicAttackDamage =  15.0f ;
    public float BasicAttackAnimationSpeed =  1.0f ;
    public float BasicAttackProjectionSpeed =  5.0f ;
    public float BasicAttackRotationSpeed =  10f ;
    public float SuperAttackDamage =  15.0f ;
    public float SuperAttackSpeed =  1.0f ;
    public float SuperAttackRotationSpeed =  10f ;


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, VisionRange);
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.DrawWireSphere(transform.position, DropAggroRange);
    }
}
