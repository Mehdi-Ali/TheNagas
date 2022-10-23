using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : BaseState
{
    //A reference for the Player State Manger
    PlayerStateManger _player;
   
    //Variables to store optimized Setter / getter parameter IDs
    int _RunningHash;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _player = GetComponent<PlayerStateManger>();
        
        if (!Owner.IsLocalClient) return ;
            _RunningHash = Animator.StringToHash("Running");

    }

    public override void EnterState()
    {
        var statics = _player.Statics ;
        _player.ServerSetMoveAndRotateSpeed(statics.MovementSpeed, statics.RotationSpeed);

        if (!IsOwner) return;
        _player.NetworkAnimator.CrossFade(_RunningHash, 0.1f, 0);

    }

    public override void UpdateState()
    {

    }

    public override void OnTickState()
    {
       if (IsOwner) _player.ReadAndSetMovementInput();  
    }

    public override void ExitState()
    {

    }

   
    
    

   
}

