using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateManger : NetworkBehaviour
{

    #region Fields and Proporties

    //Initiating the states.
    public PlayerBaseState CurrentState;

    public PlayerIdleState IdleState ;
    public PlayerRuningState RuningState ;
    public PlayerAutoAttackState AutoAttackState ;
    public PlayerFirstAbilityState FirstAbilityState ;
    public PlayerSecondAbilityState SecondAbilityState ;
    public PlayerThirdAbilityState ThirdAbilityState ;
    public PlayerUltimateState UltimateState ;
    public PlayerDeadState DeadState ;

    //Variables to store player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovenemnt;

    //Variables to handle Rotation
    Vector3 _positionTolookAt;
    Quaternion _targetRotation;

    //Variables to handle Aiming
    Vector2 _currentAimingInput ;
    Vector3 _currentAimingAt ;
    Quaternion _currentAimingRotation;


    //StateMachine Variables (logic and animation)
    public bool IsMovementPressed ;
    public bool ReadyToSwitchState;
    public bool IsCastingAnAbility ;
    public bool IsAmingPressed ;

    public bool _TempDeadStateSim = false;


    //Variables to cache Instances 
    public CharacterController CharactherController;
    private Player_Controls _playerControls;
    public Animator Animator;
    public NetworkAnimator NetworkAnimator;
    public AnimationsLength AnimationsLength;
    HitBox _hitBox;


    #endregion

    #region Execution

    private void Awake()
    {
        CashingStates();

        CurrentState = IdleState;
        CurrentState.EnterState();
        ReadyToSwitchState = true;
        IsCastingAnAbility = false;
        IsAmingPressed = false;

        //caching Instances 
        CharactherController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        NetworkAnimator = GetComponent<NetworkAnimator>();
        AnimationsLength = GetComponent<AnimationsLength>();
        _hitBox = FindObjectOfType<HitBox>();
        SubscriptionToPlayerControls();

    }

    private void CashingStates()
    {
        IdleState = GetComponent<PlayerIdleState>();
        RuningState = GetComponent<PlayerRuningState>();
        AutoAttackState = GetComponent<PlayerAutoAttackState>();
        FirstAbilityState = GetComponent<PlayerFirstAbilityState>();
        SecondAbilityState = GetComponent<PlayerSecondAbilityState>();
        ThirdAbilityState = GetComponent<PlayerThirdAbilityState>();
        UltimateState = GetComponent<PlayerUltimateState>();
        DeadState = GetComponent<PlayerDeadState>();
    }

    private void SubscriptionToPlayerControls()
    {
        //player Input callbacks.
        _playerControls = new Player_Controls();

        _playerControls.DefaultMap.Move.started += OnMovementInput;
        _playerControls.DefaultMap.Move.canceled += OnMovementInput;
        _playerControls.DefaultMap.Move.performed += OnMovementInput;

        _playerControls.DefaultMap.Aim.started += OnAimingInput;
        _playerControls.DefaultMap.Aim.canceled += OnAimingInput;
        _playerControls.DefaultMap.Aim.performed += OnAimingInput;

        _playerControls.DefaultMap.AutoAttack.performed += OnAutoAttackInput;
        _playerControls.DefaultMap.FirstAbility.canceled += OnFirstAbilityInput; 

        _playerControls.DefaultMap.SecondAbility.started += OnSecondAbilityInput_started; 
        _playerControls.DefaultMap.SecondAbility.performed += OnSecondAbilityInput_performed; 
        _playerControls.DefaultMap.SecondAbility.canceled += OnSecondAbilityInput_canceled; 
        
        _playerControls.DefaultMap.ThirdAbility.performed += OnThirdAbilityInput; 
        _playerControls.DefaultMap.Ultimate.performed += OnUltimateInput; 

        
    }

    private void OnAimingInput(InputAction.CallbackContext context)
    {
        ReadAimingtInput();
        HandleAmingRotation();        
    }

    private void ReadAimingtInput()
    {
        _currentAimingInput = _playerControls.DefaultMap.Aim.ReadValue<Vector2>();
        _currentAimingAt.x = _currentAimingInput.x;
        _currentAimingAt.y = 0.0f;
        _currentAimingAt.z = _currentAimingInput.y;

        IsAmingPressed =    _currentAimingAt.x != 0 ||
                            _currentAimingAt.z != 0 ;

    }

    private void HandleAmingRotation()
    {
        _currentAimingRotation = Quaternion.LookRotation(_currentAimingAt);
        _hitBox.transform.rotation = Quaternion.Slerp(  _hitBox.transform.rotation,
                                                        _currentAimingRotation,
                                                        10f * Time.deltaTime);
    }

    void RotateToHitBox()
    {
        transform.LookAt(_hitBox._2.transform);
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        if (CurrentState != RuningState) SwitchState(RuningState);
    }


    public void ReadMovementInput()
    {
        _currentMovementInput = _playerControls.DefaultMap.Move.ReadValue<Vector2>();
        _currentMovenemnt.x = _currentMovementInput.x;
        _currentMovenemnt.z = _currentMovementInput.y;

        IsMovementPressed = _currentMovenemnt.x != 0 ||
                            _currentMovenemnt.z != 0 ;
    }

    public void Move(float movementSpeed)
    {
        ReadMovementInput();
        CharactherController.SimpleMove(_currentMovenemnt * movementSpeed);
    }

    public void Rotate(float rotationSpeed)
    {

        //the change in position our character should point to
        _positionTolookAt.x = _currentMovenemnt.x;
        _positionTolookAt.y = 0.0f;
        _positionTolookAt.z = _currentMovenemnt.z;

        //creates a new rotation bases on where the player is currently moving
        _targetRotation = Quaternion.LookRotation(_positionTolookAt);
        transform.rotation = Quaternion.Slerp(  transform.rotation,
                                                _targetRotation,
                                                rotationSpeed * Time.deltaTime);

    }



    private void OnAutoAttackInput(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true ;
        if (CurrentState != AutoAttackState) SwitchState(AutoAttackState);
    }

    private void OnFirstAbilityInput(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (CurrentState != FirstAbilityState) SwitchState(FirstAbilityState);
    }


    private void OnSecondAbilityInput_started(InputAction.CallbackContext context)
    {
        _hitBox._2.gameObject.SetActive(true);
    }


    private void OnSecondAbilityInput_performed(InputAction.CallbackContext context)
    {
        if (!ReadyToSwitchState || IsCastingAnAbility) return;
        _hitBox._2.gameObject.SetActive(true);
    }


    private void OnSecondAbilityInput_canceled(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (CurrentState != SecondAbilityState) SwitchState(SecondAbilityState);
        RotateToHitBox();
        _hitBox._2.gameObject.SetActive(false);
    }






    private void OnThirdAbilityInput(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (CurrentState != ThirdAbilityState) SwitchState(ThirdAbilityState);
    }

    private void OnUltimateInput(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (CurrentState != UltimateState) SwitchState(UltimateState);
    }

    void Update()
    {
        CurrentState.UpdateState();
        if (CurrentState != IdleState && !IsCastingAnAbility && !IsMovementPressed ) SwitchState(IdleState);
        if ( IsAmingPressed) {ReadAimingtInput(); HandleAmingRotation();}
        else _hitBox.transform.localEulerAngles = Vector3.zero;

        if (base.IsOwner && _TempDeadStateSim) SwitchState(DeadState);       
    }
 

    private void OnEnable()
    {
        _playerControls.DefaultMap.Enable();
    }

    private void OnDisable()
    {
        _playerControls.DefaultMap.Disable();
    }

    #endregion

    #region State Specific Behavior

    
    //for extreme state switches like stunn, we use SuperSwitchState.
    public void SwitchState(PlayerBaseState state)
    {   
        if (!ReadyToSwitchState) return;
        CurrentState.ExitState();
        CurrentState = state;
        CurrentState.EnterState();

    }

    #endregion
}
