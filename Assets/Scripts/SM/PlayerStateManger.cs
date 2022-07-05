using System;
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
    public HitBoxes HitBoxes;
    public HitBox ActiveHitBox;




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




    #endregion

    #region Execution

    private void Awake()
    {
        CashingInstances();

        CurrentState = IdleState;
        CurrentState.EnterState();
        ReadyToSwitchState = true;
        IsCastingAnAbility = false;
        IsAmingPressed = false;

        SubscriptionToPlayerControls();

    }

    private void CashingInstances()
    {
        IdleState = GetComponent<PlayerIdleState>();
        RuningState = GetComponent<PlayerRuningState>();
        AutoAttackState = GetComponent<PlayerAutoAttackState>();
        FirstAbilityState = GetComponent<PlayerFirstAbilityState>();
        SecondAbilityState = GetComponent<PlayerSecondAbilityState>();
        ThirdAbilityState = GetComponent<PlayerThirdAbilityState>();
        UltimateState = GetComponent<PlayerUltimateState>();
        DeadState = GetComponent<PlayerDeadState>();

        CharactherController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        NetworkAnimator = GetComponent<NetworkAnimator>();
        AnimationsLength = GetComponent<AnimationsLength>();
        HitBoxes = FindObjectOfType<HitBoxes>();

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

        _playerControls.DefaultMap.FirstAbility.started += OnFirstAbilityInputStarted;
        _playerControls.DefaultMap.FirstAbility.performed += OnFirstAbilityInputPerformed;
        _playerControls.DefaultMap.FirstAbility.canceled += OnFirstAbilityInputCancled;

        _playerControls.DefaultMap.SecondAbility.started += OnSecondAbilityInputStarted; 
        _playerControls.DefaultMap.SecondAbility.performed += OnSecondAbilityInputPerformed; 
        _playerControls.DefaultMap.SecondAbility.canceled += OnSecondAbilityInputCanceled; 
        
        _playerControls.DefaultMap.ThirdAbility.started += OnThirdAbilityInputStarted;
        _playerControls.DefaultMap.ThirdAbility.performed += OnThirdAbilityInputPerformed;
        _playerControls.DefaultMap.ThirdAbility.canceled += OnThirdAbilityInputCanceled;

        _playerControls.DefaultMap.Ultimate.started += OnUltimateInputStarted;
        _playerControls.DefaultMap.Ultimate.performed += OnUltimateInputPerformed;
        _playerControls.DefaultMap.Ultimate.canceled += OnUltimateInputCanceld;

        
    }

    private void OnAimingInput(InputAction.CallbackContext context)
    {
        ReadAimingtInput();
        if (ActiveHitBox != null && !ActiveHitBox.Movable) HandleAmingRotation();

        if (ActiveHitBox != null && ActiveHitBox.Movable) HandleAmingLocation();

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

    private void HandleAmingLocation()
    {   
        HitBoxes.transform.localPosition = _currentAimingAt * UltimateState.Range ;
    } 

    private void HandleAmingRotation()
    {
        _currentAimingRotation = Quaternion.LookRotation(_currentAimingAt);
        HitBoxes.transform.rotation = Quaternion.Slerp(  HitBoxes.transform.rotation,
                                                        _currentAimingRotation,
                                                        100f );
    }

    void RotateToHitBox()
    {
        transform.LookAt(HitBoxes.HitBox2.transform);
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


    private void OnFirstAbilityInputStarted(InputAction.CallbackContext context)
    {
        HitBoxes.HitBox1.gameObject.SetActive(true);
        ActiveHitBox = HitBoxes.HitBox1;
    }
    private void OnFirstAbilityInputPerformed(InputAction.CallbackContext context)
    {
        if (!ReadyToSwitchState || IsCastingAnAbility) return;
        HitBoxes.HitBox1.gameObject.SetActive(true);
    }
    private void OnFirstAbilityInputCancled(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (CurrentState != FirstAbilityState) SwitchState(FirstAbilityState);
        HitBoxes.HitBox1.gameObject.SetActive(false);
    }


    private void OnSecondAbilityInputStarted(InputAction.CallbackContext context)
    {
        HitBoxes.HitBox2.gameObject.SetActive(true);
        ActiveHitBox = HitBoxes.HitBox2;
    }
    private void OnSecondAbilityInputPerformed(InputAction.CallbackContext context)
    {
        if (!ReadyToSwitchState || IsCastingAnAbility) return;
        HitBoxes.HitBox2.gameObject.SetActive(true);
    }
    private void OnSecondAbilityInputCanceled(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (CurrentState != SecondAbilityState) SwitchState(SecondAbilityState);
        RotateToHitBox();
        HitBoxes.HitBox2.gameObject.SetActive(false);
    }


    private void OnThirdAbilityInputStarted(InputAction.CallbackContext context)
    {
        HitBoxes.HitBox3.gameObject.SetActive(true);
        ActiveHitBox = HitBoxes.HitBox3;
    }
    private void OnThirdAbilityInputPerformed(InputAction.CallbackContext context)
    {
        if (!ReadyToSwitchState || IsCastingAnAbility) return;
        HitBoxes.HitBox3.gameObject.SetActive(true);
    }
    private void OnThirdAbilityInputCanceled(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (CurrentState != SecondAbilityState) SwitchState(ThirdAbilityState);
        RotateToHitBox();
        HitBoxes.HitBox3.gameObject.SetActive(false);
    }


    private void OnUltimateInputStarted(InputAction.CallbackContext context)
    {
        HitBoxes.HitBoxU.gameObject.SetActive(true);
        ActiveHitBox = HitBoxes.HitBoxU;
    }
    private void OnUltimateInputPerformed(InputAction.CallbackContext context)
    {
        if (!ReadyToSwitchState || IsCastingAnAbility) return;
        HitBoxes.HitBoxU.gameObject.SetActive(true);
    }
    private void OnUltimateInputCanceld(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (CurrentState != SecondAbilityState) SwitchState(UltimateState);
        RotateToHitBox();
        HitBoxes.HitBoxU.gameObject.SetActive(false);
    }

    void Update()
    {
        CurrentState.UpdateState();
        if (CurrentState != IdleState && !IsCastingAnAbility && !IsMovementPressed ) SwitchState(IdleState);
        if ( IsAmingPressed) {ReadAimingtInput(); HandleAmingRotation();}
        else HitBoxes.transform.localEulerAngles = Vector3.zero;

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
