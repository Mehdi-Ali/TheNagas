using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerHitBoxesAndColliders : MonoBehaviour
{
    [SerializeField] public PlayerHitBox HitBoxAA;
    [SerializeField] public PlayerHitBox HitBox1;

    [SerializeField] public PlayerHitBox HitBox2;

    [SerializeField] public PlayerHitBox HitBox3;

    [SerializeField] public PlayerHitBox HitBoxU;

    public RotationConstraint RotationConstraint;

    RotationConstraintSource rotationConstraintSource ;

    ConstraintSource ConstraintSource;

    [SerializeField] public PlayerAttackCollider AttackColliderAA ;
    [SerializeField] public PlayerAttackCollider AttackCollider1 ;
    [SerializeField] public PlayerAttackCollider AttackCollider2 ;
    [SerializeField] public PlayerAttackCollider AttackCollider3 ;
    [SerializeField] public PlayerAttackCollider AttackColliderU ;

    //Storing Variables
    public HashSet<EnemyBase> Targets = new HashSet<EnemyBase>();

    private void Awake()
    {
        RotationConstraint = GetComponentInParent<RotationConstraint>();

        rotationConstraintSource = FindObjectOfType<RotationConstraintSource>();

        ConstraintSource.sourceTransform = rotationConstraintSource.transform ;
        ConstraintSource.weight = 1;

        RotationConstraint.AddSource(ConstraintSource);
    }


}
