using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : PlayerBaseState
{
    //Game Designe Vars, Mak a stat Script maybe
    [SerializeField] private float _movementSpeed = 5.0f;
    [SerializeField] private float _rotationSpeed = 10.0f;
    
    //A refrence for the Player State Manger
    PlayerStateManger _player;
   
    //Variables to store omptimized Setter / getter parameter IDs
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
        _player.Move(_movementSpeed);
        _player.Rotate(_rotationSpeed);
    }

    public override void ExitState()
    {



    }

   
    
    

   
}

