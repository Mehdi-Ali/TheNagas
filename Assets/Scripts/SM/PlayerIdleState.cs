using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{

    public override void EnterState(PlayerStateManger player)
    {
        Debug.Log("Enter Default Idel State");
    }

    public override void UpdateState(PlayerStateManger player)
    {
        //Here wen can set the other idle eache x number of seconds.
    }

    public override void ExitState(PlayerStateManger player)
    {

    }

}
