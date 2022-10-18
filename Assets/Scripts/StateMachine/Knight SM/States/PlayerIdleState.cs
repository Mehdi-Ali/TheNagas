using FishNet.Object;

public class PlayerIdleState : IdleState
{
    //A reference for the Player State Manger
    PlayerStateManger _player;


    public override void Awake()
    {
        base.Awake();
        _player = GetComponent<PlayerStateManger>();

    }
    
    [Client(RequireOwnership = true)]
    public override void EnterState()
    {
        _player.Animator.CrossFade(_Idle, 0.15f);
    }

    public override void UpdateState()
    {
        if (_player.IsMovementPressed) _player.SwitchState(_player.RunningState);
        //Here wen can set the other idle each x number of seconds.
    }

    public override void ExitState()
    {

    }

}
