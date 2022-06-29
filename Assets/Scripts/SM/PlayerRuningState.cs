using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRuningState : PlayerBaseState
{
    //Variables to store player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovenemnt;

    //Variables to handle Rotation
    Vector3 _positionTolookAt;
    Quaternion _targetRotation;

    //StateMachine Variables (logic and animation)
    bool _isMovementPressed;

    //Variables to store omptimized Setter / getter parameter IDs
    int _isRunningHash;

    // not sure if the mono behavior part will work (Awake and IsOwner)
    private void Awake() 
    {
        //caching Hashes
        _isRunningHash = Animator.StringToHash("isRunning");

    }

    public override void EnterState(PlayerStateManger player)
    {
        if (!base.IsOwner) return;
        _isMovementPressed = true;
        player.Animator.SetBool(_isRunningHash, true);

    }

    public override void UpdateState(PlayerStateManger player)
    {
        if (!base.IsOwner) return;
        HandleMovemenet(player);
        HandleRotation(player);
        if (!_isMovementPressed || player.IsStationary) player.SwitchState(player.IdleState);
    }

    public override void ExitState(PlayerStateManger player)
    {
        if (!base.IsOwner) return;
        player.Animator.SetBool(_isRunningHash, false);

    }

    public void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovenemnt.x = _currentMovementInput.x;
        _currentMovenemnt.z = _currentMovementInput.y;
        _isMovementPressed = _currentMovenemnt.x != 0 || _currentMovenemnt.z != 0;

    }
    
    private void HandleMovemenet(PlayerStateManger player)
    {
        player.CharactherController.SimpleMove(_currentMovenemnt * player.MovementSpeed);
    }

    private void HandleRotation(PlayerStateManger player)
    {

        //the change in position our character should point to
        _positionTolookAt.x = _currentMovenemnt.x;
        _positionTolookAt.y = 0.0f;
        _positionTolookAt.z = _currentMovenemnt.z;

        //creates a new rotation bases on where the player is currently moving
        _targetRotation = Quaternion.LookRotation(_positionTolookAt);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, player.RotationSpeed * Time.deltaTime);

    }
}

