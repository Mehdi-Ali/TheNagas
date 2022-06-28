using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public override void EnterState(PlayerStateManger player)
    {
        Debug.Log("Im Dead");
    }

    public override void UpdateState(PlayerStateManger player)
    {
        //Do not read moving Iputs ( same as the other four 5 Abbilites)
    }

    public override void OnCollisionEnter(PlayerStateManger player)
    {

    }

}
