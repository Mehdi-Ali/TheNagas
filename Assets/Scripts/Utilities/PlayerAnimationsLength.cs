using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsLength : MonoBehaviour
{
    Animator _animator ;

    AnimationClip[] _clips ;

    //Animations
    public float IdleDuration ;
    public float RunningDuration ;
    public float AutoAttack1Duration ;
    public float AutoAttack2Duration ;
    public float AutoAttack3Duration ;
    public float FirstAbilityDuration ;
    public float SecondAbilityDuration ;
    public float ThirdAbilityDuration ;
    public float UltimateDuration ;
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
                case "AutoAttack1_State":
                    AutoAttack1Duration = clip.length;
                    break;
                case "AutoAttack2_State":
                    AutoAttack2Duration = clip.length;
                    break;
                case "AutoAttack3_State":
                    AutoAttack3Duration = clip.length;
                    break;
                case "FirstAbility_State":
                    FirstAbilityDuration = clip.length;
                    break;
                case "SecondAbility_State":
                    SecondAbilityDuration = clip.length;
                    break;
                case "ThirdAbility_State":
                    ThirdAbilityDuration = clip.length;
                    break;
                case "Ultimate_State": 
                    UltimateDuration = clip.length ; // 
                    break;
                case "Dead_State":
                    DeadDuration = clip.length;
                    break;
            } 
        }
    }

}
