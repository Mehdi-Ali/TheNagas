using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    //A refrence for the Player State Manger
    PlayerStateManger _player;

    //Variables to store omptimized Setter / getter parameter IDs
    int _Idle;

    private void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _Idle = Animator.StringToHash("Idle");
    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        _player.Animator.CrossFade(_Idle, 0.15f);
    }

    public override void UpdateState()
    {
        if (_player.IsMovementPressed) _player.SwitchState(_player.RunningState);
        //Here wen can set the other idle eache x number of seconds.
    }

    public override void ExitState()
    {

    }

}
