using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBoxesAndColliders : MonoBehaviour
{
    [SerializeField] public EnemyHitBox BasicHitBox;
    [SerializeField] public EnemyHitBox SuperHitBox;


    [SerializeField] public EnemyAttackCollider BasicCollider ;
    [SerializeField] public EnemyAttackCollider SuperCollider ;

    //Storing Variables
    public HashSet<PlayerBase> Targets = new HashSet<PlayerBase>();
    //create something like EnemyBase for PLayers.

}
