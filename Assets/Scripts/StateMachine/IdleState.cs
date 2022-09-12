using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    //A reference for the Player State Manger


    //Variables to store optimized Setter / getter parameter IDs
    protected int _Idle;

    public virtual void Awake()
    {
        //caching Hashes
        _Idle = Animator.StringToHash("Idle");


    }
    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }
}
