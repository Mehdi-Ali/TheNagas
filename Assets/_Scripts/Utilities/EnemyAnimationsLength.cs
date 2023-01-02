using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationsLength : MonoBehaviour
{
    Animator _animator ;

    AnimationClip[] _clips ;

    //Animations
    public float IdleDuration ;
    public float RunningDuration ;
    public float BasicAttack_Duration ;
    public float SuperAttack_Duration ;
    public float DeadDuration ;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();

        _clips = _animator.runtimeAnimatorController.animationClips ;

        foreach(AnimationClip clip in _clips)
        {
            //if (clip.name == "FirstAbility_State") FirstAttackDuration = clip.length ;
            switch (clip.name)
            {
                case "Idle_State":
                    IdleDuration = clip.length;
                    break;
                case "Running_State":
                    RunningDuration = clip.length;
                    break;
                case "BasicAttack_State":
                    BasicAttack_Duration = clip.length;
                    break;
                case "SuperAttack_State":
                    SuperAttack_Duration = clip.length;
                    break;
                case "Dead_State":
                    DeadDuration = clip.length;
                    break;
            } 
        }
    }

}
