using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBoxesAndColliders : MonoBehaviour
{
    [SerializeField] public EnemyHitBox BasicHitBox;
    [SerializeField] public EnemyHitBox SuperHitBox;


    [SerializeField] public AttackCollider BasicCollider ;
    [SerializeField] public AttackCollider SuperCollider ;

    //Storing Variables
    public HashSet<EnemyBase> Targets = new HashSet<EnemyBase>();
    //create something like EnemyBase for PLayers.

}
