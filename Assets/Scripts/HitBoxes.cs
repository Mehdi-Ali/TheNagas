using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class HitBoxes : MonoBehaviour
{
    [SerializeField] public HitBox HitBox1;

    [SerializeField] public HitBox HitBox2;

    [SerializeField] public HitBox HitBox3;

    [SerializeField] public HitBox HitBoxU;

    public RotationConstraint RotationConstraint;

    RotationConstraintSource rotationConstraintSource ;

    ConstraintSource ConstraintSource;



    private void Awake()
    {
        RotationConstraint = GetComponentInParent<RotationConstraint>();

        rotationConstraintSource = FindObjectOfType<RotationConstraintSource>();

        ConstraintSource.sourceTransform = rotationConstraintSource.transform ;
        ConstraintSource.weight = 1;

        RotationConstraint.AddSource(ConstraintSource);
    }


}
