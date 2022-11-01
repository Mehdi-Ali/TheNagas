using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerHitBoxesAndColliders : MonoBehaviour
{
    #region Only Client Vars
    #if !UNITY_SERVER
   
    [SerializeField] public PlayerHitBox HitBoxAA;
    [SerializeField] public PlayerHitBox HitBox1;

    [SerializeField] public PlayerHitBox HitBox2;

    [SerializeField] public PlayerHitBox HitBox3;

    [SerializeField] public PlayerHitBox HitBoxU;


    #endif
    #endregion

    public RotationConstraint RotationConstraint;

    RotationConstraintSource rotationConstraintSource ;

    ConstraintSource ConstraintSource;

    [SerializeField] public PlayerAttackCollider AttackColliderAA ;
    [SerializeField] public PlayerAttackCollider AttackCollider1 ;
    [SerializeField] public PlayerAttackCollider AttackCollider2 ;
    [SerializeField] public PlayerAttackCollider AttackCollider3 ;
    [SerializeField] public PlayerAttackCollider AttackColliderU ;

    public HashSet<EnemyBase> Targets = new();

    private void Awake()
    {
        RotationConstraint = GetComponentInParent<RotationConstraint>();

        rotationConstraintSource = FindObjectOfType<RotationConstraintSource>();

        ConstraintSource.sourceTransform = rotationConstraintSource.transform ;
        ConstraintSource.weight = 1;

        RotationConstraint.AddSource(ConstraintSource);
    }


}
