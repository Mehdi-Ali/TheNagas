using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    //A refrence for the Player State Manger
    PlayerStateManger player;

    private void Awake()
    {
        //Caching The Player State Manger
        player = GetComponent<PlayerStateManger>();
    }

    public override void EnterState()
    {

    }

    public override void UpdateState()
    {
        //Here wen can set the other idle eache x number of seconds.
    }

    public override void ExitState()
    {

    }

}
