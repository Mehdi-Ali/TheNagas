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


    //StateMachine Variables (logic and animation)
    // we`ll see if we don't use it in Enemy we move it to Player 
    public bool ReadyToSwitchState;
    public bool IsCastingAnAbility ;

    
    
    //functions\\

    public virtual void Awake()
    {
        CashingStandardInstances();

        CurrentState = IdleState;
        CurrentState.EnterState();

        // we`ll see if we don't use it in Enemy we move it to Player
        ReadyToSwitchState = true;
        IsCastingAnAbility = false; 
    }

    private void CashingStandardInstances()
    {
        IdleState = GetComponent<IdleState>();
        RunningState = GetComponent<RunningState>();
        DeadState = GetComponent<DeadState>();

        Animator = GetComponent<Animator>();
        
    }

    public virtual void Update()
    {
        CurrentState.UpdateState();
    }
    
    public void SwitchState(BaseState state)
    {   
        // I think it can be deleted because we never call it if CurrentState == DeadState
        if (CurrentState == DeadState) return ; 
        
        if (ReadyToSwitchState || state == DeadState)
        {
            CurrentState.ExitState();
            CurrentState = state;
            CurrentState.EnterState();
        }

    }
}
