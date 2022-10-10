using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : RunningState
{
    //A reference for the Player State Manger
    PlayerStateManger _player;
   
    //Variables to store optimized Setter / getter parameter IDs
    
    int _RunningHash;

    // not sure if the mono behavior part will work (Awake and IsOwner)
    private void Awake() 
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();
        
        //caching Hashes
        _RunningHash = Animator.StringToHash("Running");

    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        _player.Animator.CrossFade(_RunningHash, 0.1f);

    }

    public override void UpdateState()
    {
        if (!base.IsOwner) return;
        _player.SimpleMove(_player.Statics.MovementSpeed);
        _player.Rotate(_player.Statics.RotationSpeed, _player.CurrentMovement);
    }

    public override void ExitState()
    {



    }

   
    
    

   
}

