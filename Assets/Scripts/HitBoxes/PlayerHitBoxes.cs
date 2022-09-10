using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerHitBoxes : MonoBehaviour
{
    [SerializeField] public HitBox HitBox1;

    [SerializeField] public HitBox HitBox2;

    [SerializeField] public HitBox HitBox3;

    [SerializeField] public HitBox HitBoxU;

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
