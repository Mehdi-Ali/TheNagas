using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerHitBoxesAndColliders : MonoBehaviour
{
    [SerializeField] public PlayerHitBox HitBox1;

    [SerializeField] public PlayerHitBox HitBox2;

    [SerializeField] public PlayerHitBox HitBox3;

    [SerializeField] public PlayerHitBox HitBoxU;

    public RotationConstraint RotationConstraint;

    RotationConstraintSource rotationConstraintSource ;

    ConstraintSource ConstraintSource;

    [SerializeField] public AttackCollider AttackColliderAA ;
    [SerializeField] public AttackCollider AttackCollider1 ;
    [SerializeField] public AttackCollider AttackCollider2 ;
    [SerializeField] public AttackCollider AttackCollider3 ;
    [SerializeField] public AttackCollider AttackColliderU ;

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
