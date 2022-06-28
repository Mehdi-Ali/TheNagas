using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class Moving : NetworkBehaviour
{
    //Game Designe Vars
    [SerializeField] private float _mouvementSpeed = 5.0f;
    [SerializeField] private float _rotationSpeed = 10.0f;
    

    //Variables to cache Instances 
    private CharacterController _charactherController;
    private Player_Controls _playerControls ;
    Animator _animmator ;

    //Variables to store player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovenemnt;

    //Variables to handle Rotation
    Vector3 _positionTolookAt;
    Quaternion _targetRotation;


    //StateMachine Variables (logic and animation)
    bool _isMovementPressed;
    bool _isRunning = false;
    public bool _isDead = false;
    bool alreadyDead = false ;

    //Variables to store omptimized Setter / getter parameter IDs
    int _isRunningHash;
    int _AutoAttackHash;
    int _FirstAbilityHash;
    int _SecondAbilityHash;
    int _ThirdAbilityHash;
    int _UltimateHash;
    int _DeadHash;

    //Camera Stuff
    CameraFollowController _camera;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner) return;

        _camera = FindObjectOfType<CameraFollowController>();
        _camera._moving = this;
        _camera.ClinetConnected = true ;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (base.IsOwner) _camera.ClinetConnected = false ;
    }
    //End Camera stuff

    private void Awake() 
    {
        //caching Instances 
        _charactherController = GetComponent<CharacterController>();
        _animmator = GetComponent<Animator>();

        //caching Hashes
        _isRunningHash = Animator.StringToHash("isRunning");
        _AutoAttackHash = Animator.StringToHash("AutoAttack");
        _FirstAbilityHash = Animator.StringToHash("FirstAbility");
        _SecondAbilityHash = Animator.StringToHash("SecondAbility");
        _ThirdAbilityHash = Animator.StringToHash("ThirdAbility");
        _UltimateHash = Animator.StringToHash("Ultimate");
        _DeadHash = Animator.StringToHash("Dead");
        
        //player Input callbacks.
        _playerControls = new Player_Controls();

        _playerControls.DefaultMap.Move.started += OnMovementInput;
        _playerControls.DefaultMap.Move.canceled += OnMovementInput;
        _playerControls.DefaultMap.Move.performed += OnMovementInput;

        _playerControls.DefaultMap.SecondAbility.performed += OnSecondAbilityUnput; // x 5


    }

    private void OnSecondAbilityUnput(InputAction.CallbackContext context)
    {
        //logic
        _animmator.SetTrigger(_SecondAbilityHash);
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovenemnt.x = _currentMovementInput.x;
        _currentMovenemnt.z = _currentMovementInput.y;
        _isMovementPressed = _currentMovenemnt.x != 0 || _currentMovenemnt.z != 0;
        _isRunning = _isMovementPressed ;
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
        HandleAnimation();
        HandleMovemenet();
        HandleRotation();
    }

    private void HandleMovemenet()
    {
        if (!base.IsOwner) return;
        if (_isMovementPressed) _charactherController.SimpleMove(_currentMovenemnt * _mouvementSpeed);
    }

    private void HandleAnimation()
    {
        if (!base.IsOwner) return;

        if (_isRunning) _animmator.SetBool(_isRunningHash, true);
        if (!_isRunning) _animmator.SetBool(_isRunningHash, false);

        if (_isDead && !alreadyDead)
        {
            _animmator.SetTrigger(_DeadHash);
            alreadyDead = true;

        } 
    }

    private void HandleRotation()
    {
        if (!base.IsOwner || !_isRunning) return;

        //the change in position our character should point to
        _positionTolookAt.x = _currentMovenemnt.x ;
        _positionTolookAt.y = 0.0f ;
        _positionTolookAt.z = _currentMovenemnt.z ;

        //creates a new rotation bases on where the player is currently moving
        _targetRotation = Quaternion.LookRotation(_positionTolookAt);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);

    }
}

