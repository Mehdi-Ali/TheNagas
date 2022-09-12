using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : BaseState
{
    //Variables to store omptimized Setter / getter parameter IDs
    protected int _DeadHash;

    public virtual void Awake()
    {
        //caching Hashes
        _DeadHash = Animator.StringToHash("Dead");
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
