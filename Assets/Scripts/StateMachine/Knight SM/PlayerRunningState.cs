using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : RunningState
{
    //A reference for the Player State Manger
    PlayerStateManger _player;
   
    //Variables to store optimized Setter / getter parameter IDs
    
    int _RunningHash;

    private void Awake() 
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();
        
        //caching Hashes
        _RunningHash = Animator.StringToHash("Running");

    }

    public override void EnterState()
    {
        _player.Animator.CrossFade(_RunningHash, 0.1f);
    }

    public override void UpdateState()
    {
        _player.SimpleMove(_player.Statics.MovementSpeed);
        _player.RotatePlayer(_player.Statics.RotationSpeed, _player.CurrentMovement);
    }

    public override void ExitState()
    {



    }

   
    
    

   
}

