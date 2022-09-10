using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class StateManger : NetworkBehaviour
{
    //Initiating the states.
    public BaseState CurrentState;
    public IdleState IdleState ;
    public RunningState RunningState ;
    public DeadState DeadState ;


    //Variables to cache Instances 
    public Animator Animator;
    public AnimationsLength AnimationsLength;

    //StateMachine Variables (logic and animation) 
    public bool ReadyToSwitchState;
    public bool IsCastingAnAbility ;

    
    
    //functions\\

    public virtual void Awake()
    {
        CashingStandardInstances();

        CurrentState = IdleState;
        CurrentState.EnterState();

        ReadyToSwitchState = true;
        IsCastingAnAbility = false;
    }

        private void CashingStandardInstances()
    {
        IdleState = GetComponent<IdleState>();
        RunningState = GetComponent<RunningState>();
        DeadState = GetComponent<DeadState>();

        Animator = GetComponent<Animator>();
        AnimationsLength = GetComponent<AnimationsLength>();
    }

    public virtual void Update()
    {
        CurrentState.UpdateState();
    }
    
    public void SwitchState(BaseState state)
    {   
        if (!ReadyToSwitchState) return;
        CurrentState.ExitState();
        CurrentState = state;
        CurrentState.EnterState();

    }
}
