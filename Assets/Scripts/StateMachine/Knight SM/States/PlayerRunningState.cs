using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : BaseState
{
    PlayerStateManger _player;
   
    #region Only Client Vars
    #if !UNITY_SERVER

    int _RunningHash;

    #endif
    #endregion

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
        _player.RpcSetMoveAndRotateSpeed(statics.MovementSpeed, statics.RotationSpeed);

        if (!IsOwner) return;
        _player.NetworkAnimator.CrossFade(_RunningHash, 0.1f, 0);
    }

    public override void UpdateState(){}

    public override void OnTickState()
    {
       if (IsOwner) _player.ReadAndSetMovementInput();  
    }

    public override void ExitState(){}
   
}

