using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateManger : StateManger
{

    #region Fields and Properties

    //Initiating the states.
    public PlayerAutoAttackState AutoAttackState ;
    public PlayerFirstAbilityState FirstAbilityState ;
    public PlayerSecondAbilityState SecondAbilityState ;
    public PlayerThirdAbilityState ThirdAbilityState ;
    public PlayerUltimateState UltimateState ;


    //Variables to store player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovement;

    //Variables to handle Rotation
    Vector3 _positionToLookAt;
    Quaternion _targetRotation;

    //Variables to handle Aiming
    Vector2 _currentAimingInput ;
    Vector3 _currentAimingAt ;
    Quaternion _currentAimingRotation;
    public PlayerHitBoxesAndColliders HitBoxes;
    public PlayerHitBox ActiveHitBox;
    public PlayerAttackCollider ActiveAttackCollider;

    //Variables to handle Gravity
    [SerializeField] Vector3 _gravity = new Vector3(0.0f , -9.8f , 0.0f);




    //StateMachine Variables (logic and animation)
    public bool IsMovementPressed ;
    public bool IsAimingPressed ;
    public bool _TempDeadStateSim = false;



    //Variables to cache Instances 
    public CharacterController CharacterController;
    private Player_Controls _playerControls;
    public CooldownSystem CooldownSystem;
    public PlayerAnimationsLength AnimationsLength;




    #endregion

    #region Execution

    public override void Awake()
    {
        base.Awake();
        CashingPlayerInstances() ;
        IsAimingPressed = false;
        SubscriptionToPlayerControls();

        CharacterController.enableOverlapRecovery = true ;

    }

    private void CashingPlayerInstances()
    {

        AutoAttackState = GetComponent<PlayerAutoAttackState>();
        FirstAbilityState = GetComponent<PlayerFirstAbilityState>();
        SecondAbilityState = GetComponent<PlayerSecondAbilityState>();
        ThirdAbilityState = GetComponent<PlayerThirdAbilityState>();
        UltimateState = GetComponent<PlayerUltimateState>();

        CharacterController = GetComponent<CharacterController>();
        HitBoxes = GetComponentInChildren<PlayerHitBoxesAndColliders>();
        CooldownSystem = FindObjectOfType<CooldownSystem>();

        AnimationsLength = GetComponent<PlayerAnimationsLength>();
        
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

        _playerControls.DefaultMap.AutoAttack.started += OnAutoAttackInputStarted;
        _playerControls.DefaultMap.AutoAttack.performed += OnAutoAttackInputPerformed;
        _playerControls.DefaultMap.AutoAttack.canceled += OnAutoAttackInputcanceled;

        _playerControls.DefaultMap.FirstAbility.started += OnFirstAbilityInputStarted;
        _playerControls.DefaultMap.FirstAbility.performed += OnFirstAbilityInputPerformed;
        _playerControls.DefaultMap.FirstAbility.canceled += OnFirstAbilityInputCanceled;

        _playerControls.DefaultMap.SecondAbility.started += OnSecondAbilityInputStarted; 
        _playerControls.DefaultMap.SecondAbility.performed += OnSecondAbilityInputPerformed; 
        _playerControls.DefaultMap.SecondAbility.canceled += OnSecondAbilityInputCanceled; 
        
        _playerControls.DefaultMap.ThirdAbility.started += OnThirdAbilityInputStarted;
        _playerControls.DefaultMap.ThirdAbility.performed += OnThirdAbilityInputPerformed;
        _playerControls.DefaultMap.ThirdAbility.canceled += OnThirdAbilityInputCanceled;

        _playerControls.DefaultMap.Ultimate.started += OnUltimateInputStarted;
        _playerControls.DefaultMap.Ultimate.performed += OnUltimateInputPerformed;
        _playerControls.DefaultMap.Ultimate.canceled += OnUltimateInputCanceled;

        
    }

    private void OnAimingInput(InputAction.CallbackContext context)
    {
        if (ActiveHitBox == null ) return;

        ReadAimingInput();
        HandleAimingRotation();

        if (ActiveHitBox.Movable) HandleAimingLocation();

    }

    private void ReadAimingInput()
    {
        _currentAimingInput = _playerControls.DefaultMap.Aim.ReadValue<Vector2>();
        _currentAimingAt.x = _currentAimingInput.x;
        _currentAimingAt.y = 0.0f;
        _currentAimingAt.z = _currentAimingInput.y;

        IsAimingPressed =    _currentAimingAt.x != 0 ||
                            _currentAimingAt.z != 0 ;

    }

    private void HandleAimingLocation()
    {   
        HitBoxes.transform.localPosition = _currentAimingAt * UltimateState.Range ;
    } 

    private void HandleAimingRotation()
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
        if (CurrentState != RunningState) SwitchState(RunningState);
    }


    public void ReadMovementInput()
    {
        _currentMovementInput = _playerControls.DefaultMap.Move.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;

        IsMovementPressed = _currentMovement.x != 0 ||
                            _currentMovement.z != 0 ;
    }

    public void Move(float movementSpeed)
    {
        ReadMovementInput();
        CharacterController.SimpleMove(_currentMovement * movementSpeed);
    }

    public void Rotate(float rotationSpeed)
    {
        //the change in position our character should point to
        _positionToLookAt.x = _currentMovement.x;
        _positionToLookAt.y = 0.0f;
        _positionToLookAt.z = _currentMovement.z;

        //creates a new rotation bases on where the player is currently moving
        _targetRotation = Quaternion.LookRotation(_positionToLookAt);
        transform.rotation = Quaternion.Slerp(  transform.rotation,
                                                _targetRotation,
                                                rotationSpeed * Time.deltaTime);

    }



    private void OnAutoAttackInputStarted(InputAction.CallbackContext context)
    {
        AutoAttackState.Continue = false;
        HitBoxes.AttackColliderAA.gameObject.SetActive(true); //to check
        ActiveAttackCollider = HitBoxes.AttackColliderAA ;
        if (CurrentState != AutoAttackState) SwitchState(AutoAttackState);
    }
    private void OnAutoAttackInputPerformed(InputAction.CallbackContext context)
    {
        if (AutoAttackState.Continue == false) AutoAttackState.Continue = true ;
    }
    private void OnAutoAttackInputcanceled(InputAction.CallbackContext context)
    {
        AutoAttackState.Continue = false ;
    }


    private void OnFirstAbilityInputStarted(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(FirstAbilityState.Id)) return;
        HitBoxes.HitBox1.gameObject.SetActive(true);
        ActiveHitBox = HitBoxes.HitBox1;
        ActiveAttackCollider = HitBoxes.AttackCollider1 ;
    }
    private void OnFirstAbilityInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(FirstAbilityState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility ) return;

        HitBoxes.HitBox1.gameObject.SetActive(true);
    }
    private void OnFirstAbilityInputCanceled(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(FirstAbilityState.Id)) return ;
        if (CurrentState != FirstAbilityState) SwitchState(FirstAbilityState);
        HitBoxes.HitBox1.gameObject.SetActive(false);
    }


    private void OnSecondAbilityInputStarted(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(SecondAbilityState.Id)) return;
        HitBoxes.HitBox2.gameObject.SetActive(true);
        ActiveHitBox = HitBoxes.HitBox2;
        ActiveAttackCollider = HitBoxes.AttackCollider2 ;
    }
    private void OnSecondAbilityInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(SecondAbilityState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility) return;

        HitBoxes.HitBox2.gameObject.SetActive(true);
    }
    private void OnSecondAbilityInputCanceled(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(SecondAbilityState.Id)) return;
        if (CurrentState != SecondAbilityState) SwitchState(SecondAbilityState);
        RotateToHitBox();
        HitBoxes.HitBox2.gameObject.SetActive(false);
    }


    private void OnThirdAbilityInputStarted(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(ThirdAbilityState.Id)) return;
        HitBoxes.HitBox3.gameObject.SetActive(true);
        ActiveHitBox = HitBoxes.HitBox3;
        ActiveAttackCollider = HitBoxes.AttackCollider3 ;
    }
    private void OnThirdAbilityInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(ThirdAbilityState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility) return;
        HitBoxes.HitBox3.gameObject.SetActive(true);
    }
    private void OnThirdAbilityInputCanceled(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(ThirdAbilityState.Id)) return;
        if (CurrentState != SecondAbilityState) SwitchState(ThirdAbilityState);
        RotateToHitBox();
        HitBoxes.HitBox3.gameObject.SetActive(false);
    }


    private void OnUltimateInputStarted(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(UltimateState.Id)) return;
        HitBoxes.HitBoxU.gameObject.SetActive(true);
        ActiveHitBox = HitBoxes.HitBoxU;
          ActiveAttackCollider = HitBoxes.AttackColliderU ;
    }
    private void OnUltimateInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(UltimateState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility) return;
        HitBoxes.HitBoxU.gameObject.SetActive(true);
    }
    private void OnUltimateInputCanceled(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(UltimateState.Id)) return;
        if (CurrentState != SecondAbilityState) SwitchState(UltimateState);
        RotateToHitBox();
        HitBoxes.HitBoxU.gameObject.SetActive(false);
    }

    public override void Update()
    {
        base.Update();

        if (!CharacterController.isGrounded)
        {
            CharacterController.Move(_gravity * Time.deltaTime);
        }

        if (CurrentState != IdleState && !IsCastingAnAbility && !IsMovementPressed ) SwitchState(IdleState);
        if ( IsAimingPressed) {ReadAimingInput(); HandleAimingRotation();}
        else HitBoxes.transform.localEulerAngles = Vector3.zero;

        if (base.IsOwner && _TempDeadStateSim) SwitchState(DeadState);   
      
    }
 
    // PlayerControls Enable / Disable 
    private void OnEnable()
    {
        _playerControls.DefaultMap.Enable();
    }
    private void OnDisable()
    {
        _playerControls.DefaultMap.Disable();
    }

    #endregion

}
