using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManger : MonoBehaviour
{

    PlayerBaseState _currentState;

    public PlayerIdleState IdleState = new();
    public PlayerRunningState RuningState = new();
    public PlayerDeadState DeathState = new();


    // delete
    public bool input = false;
    //delete


    void Start()
    {
        _currentState = IdleState;

        _currentState.EnterState(this);
    }


    void Update()
    {
        _currentState.UpdateState(this);
    }

    public void SwitchState(PlayerBaseState state)
    {
        _currentState = state;
        state.EnterState(this);
    }
}
