using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBoxesAndColliders : MonoBehaviour
{
    [SerializeField] public EnemyHitBox BasicHitBox;
    [SerializeField] public EnemyHitBox SuperHitBox;


    [SerializeField] public EnemyAttackCollider BasicCollider ;
    [SerializeField] public EnemyAttackCollider SuperCollider ;

    public HashSet<PlayerBase> Targets = new HashSet<PlayerBase>();
    public HashSet<PlayerBase> TargetsToChase = new HashSet<PlayerBase>();

}
