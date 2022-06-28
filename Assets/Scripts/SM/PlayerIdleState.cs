using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{




    public override void EnterState(PlayerStateManger player)
    {
        Debug.Log("Im Idle");
    }

    public override void UpdateState(PlayerStateManger player)
    {
        //reading moving Iputs
        // if running switch to running
        if (player.input)
        {
            player.SwitchState(player.RuningState);
        }
        
    }

    public override void OnCollisionEnter(PlayerStateManger player)
    {

    }

}
