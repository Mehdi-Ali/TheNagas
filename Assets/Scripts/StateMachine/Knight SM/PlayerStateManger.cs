using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateManger : StateManger
{
    [Header("Scriptable Objects")]
    [SerializeField] 
    public PlayerStaticsScriptableObject Statics ;

    [Space(10)]
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
    private float _aimingRange ;
    public PlayerHitBoxesAndColliders HitBoxes;
    public PlayerHitBox ActiveHitBox;
    public PlayerAttackCollider ActiveAttackCollider;

    //Variables to handle Gravity
    [SerializeField] Vector3 _gravity = new Vector3(0.0f , -9.8f , 0.0f);




    //StateMachine Variables (logic and animation)
    public bool IsMovementPressed ;
    private bool _isAimingPressed ;
    private bool _isAutoAiming ;




    //Variables to cache Instances 
    public CharacterController CharacterController;
    private Player_Controls _playerControls;
    public CooldownSystem CooldownSystem;
    public PlayerAnimationsLength AnimationsLength;

    //Auto Aiming vars
    private float _smallestDistance;
    private Transform _targetPos;
    private float _distance;




    #endregion

    #region Execution

    public override void Awake()
    {
        base.Awake();
        CashingPlayerInstances() ;
        _isAimingPressed = false;
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
        HandleAiming();
    }

    private void HandleAiming()
    {
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

        _isAimingPressed =    _currentAimingInput.x != 0 ||
                            _currentAimingInput.y != 0 ;

    }

    private void HandleAimingRotation()
    {
        _currentAimingRotation = Quaternion.LookRotation(_currentAimingAt);
        HitBoxes.transform.rotation = Quaternion.Slerp(  HitBoxes.transform.rotation,
                                                        _currentAimingRotation,
                                                        100f );
    }

    private void HandleAimingLocation()
    {   
        HitBoxes.transform.localPosition = _currentAimingAt * _aimingRange ;
    } 

    void RotateToHitBox()
    {
        transform.LookAt(ActiveHitBox.transform);
        _isAutoAiming = false ;
        HitBoxes.transform.localPosition = Vector3.zero ;
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
        _aimingRange = Statics.AutoAttackRange;
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
        _aimingRange = Statics.FirstAbilityRange;
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
        _aimingRange = Statics.SecondAbilityRange;
        ActiveHitBox = HitBoxes.HitBox2;
        if (!_isAimingPressed) AutoAiming();
        HitBoxes.HitBox2.gameObject.SetActive(true);
        ActiveAttackCollider = HitBoxes.AttackCollider2 ;
    }
    private void OnSecondAbilityInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(SecondAbilityState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility) return;
        _isAutoAiming = false ;
        HitBoxes.HitBox2.gameObject.SetActive(true);
    }
    private void OnSecondAbilityInputCanceled(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(SecondAbilityState.Id)) return;
        RotateToHitBox();
        if (CurrentState != SecondAbilityState) SwitchState(SecondAbilityState);
        HitBoxes.HitBox2.gameObject.SetActive(false);
    }


    private void OnThirdAbilityInputStarted(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(ThirdAbilityState.Id)) return;
        _aimingRange = Statics.ThirdAbilityRange;
        ActiveHitBox = HitBoxes.HitBox3;
        if (!_isAimingPressed) AutoAiming();
        HitBoxes.HitBox3.gameObject.SetActive(true);
        ActiveAttackCollider = HitBoxes.AttackCollider3 ;
    }
    private void OnThirdAbilityInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(ThirdAbilityState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility) return;
                _isAutoAiming = false ;
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
        _aimingRange = Statics.UltimateAbilityRange;
        ActiveHitBox = HitBoxes.HitBoxU;
        if (!_isAimingPressed) AutoAiming();
        HitBoxes.HitBoxU.gameObject.SetActive(true);
          ActiveAttackCollider = HitBoxes.AttackColliderU ;
    }
    private void OnUltimateInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(UltimateState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility) return;
        HitBoxes.HitBoxU.gameObject.SetActive(true);
        _isAutoAiming = false ;
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
        if ( _isAimingPressed) HandleAiming();
        else if (!_isAutoAiming) HitBoxes.transform.localEulerAngles = Vector3.zero;
 
      
    }

    
    //Auto Aiming 
    private void AutoAiming()
    {
        _isAutoAiming = true ;
        _smallestDistance = _aimingRange;
        _targetPos = null ;

        Collider[] _nearbyEnemies = Physics.OverlapSphere(this.transform.position, _aimingRange);
        foreach (Collider enemy in _nearbyEnemies)
        {
            if (enemy.TryGetComponent<EnemyBase>(out EnemyBase target) )
            {
                _distance = Vector3.Distance(this.transform.position, target.transform.position);
                if (_distance > _smallestDistance ) continue ;
                _smallestDistance = _distance;
                _targetPos = target.transform;
                
            }
        }

        if (_targetPos == null) return ;
        Debug.Log("Auto aimed to:" + _targetPos.name );
        //transform.LookAt(_targetPos);
        HitBoxes.transform.LookAt(_targetPos);
        if (ActiveHitBox.Movable) HitBoxes.transform.position = _targetPos.position;

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
