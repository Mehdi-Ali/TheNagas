using UnityEngine;

public class PlayerRunningState : PlayerBaseState
{
    public override void EnterState(PlayerStateManger player)
    {
        Debug.Log("Im Running");
    }

    public override void UpdateState(PlayerStateManger player)
    {
        //reading moving Iputs
    }

    public override void OnCollisionEnter(PlayerStateManger player)
    {

    }

}

