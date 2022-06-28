using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class Moving : NetworkBehaviour
{
    //Game Designe Vars
    [SerializeField]
    private float _mouvementSpeed = 5f;

    //Variables to cache Instances 
    private CharacterController _charactherController;
    private Player_Controls _playerControls ;

    //Variables to store player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovenemnt;
    bool _isMovementPressed;

    private void Awake() 
    {
        _charactherController = GetComponent<CharacterController>();
        
        _playerControls = new Player_Controls();
        _playerControls.DefaultMap.Move.started += OnMovementInput;
        _playerControls.DefaultMap.Move.canceled += OnMovementInput;
        _playerControls.DefaultMap.Move.performed += OnMovementInput;


    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovenemnt.x = _currentMovementInput.x;
        _currentMovenemnt.z = _currentMovementInput.y;
        _isMovementPressed = _currentMovenemnt.x != 0 || _currentMovenemnt.z != 0;
    }

    private void OnEnable() 
    {
        _playerControls.DefaultMap.Enable();
    }

    private void OnDisable() 
    {
        _playerControls.DefaultMap.Disable();
    }

    // the equivalante of "if (!base.IsOwner) return;"
    //is : 
    //[Client(RequireOwnership = true)]
    private void Update()
    {
        if (!base.IsOwner) return;
        if (_isMovementPressed) _charactherController.SimpleMove(_currentMovenemnt * _mouvementSpeed );
    }
}

