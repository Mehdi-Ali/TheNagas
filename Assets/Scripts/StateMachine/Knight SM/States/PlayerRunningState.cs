using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : BaseState
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
        _player.NetworkAnimator.CrossFade(_RunningHash, 0.1f, 0);
    }

    public override void UpdateState()
    {
       _player.ReadAndSetMovementInput(); 
    }

    public override void ExitState()
    {

    }

   
    
    

   
}

