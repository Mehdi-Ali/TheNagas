using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : BaseState
{
    PlayerStateManger _player;

    int _RunningHash;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _player = GetComponent<PlayerStateManger>();
        _RunningHash = Animator.StringToHash("Running");
    }

    public override void EnterState()
    {
        var statics = _player.Statics ;
        _player.SetMoveAndRotateSpeed(statics.MovementSpeed, statics.RotationSpeed);

        _player.NetworkAnimator.CrossFade(_RunningHash, 0.1f, 0);
    }

    public override void UpdateState(){}

    public override void OnTickState()
    {
       if (IsOwner) _player.ReadAndSetMovementInput();  
    }

    public override void ExitState(){}
   
}

