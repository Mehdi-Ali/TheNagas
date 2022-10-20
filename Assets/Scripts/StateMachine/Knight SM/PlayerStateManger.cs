using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateManger : NetworkBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField]
    public PlayerStaticsScriptableObject Statics ;

    [Space(10)]
    #region Fields and Properties

    //Initiating the states.    
    public BaseState CurrentState;
    public PlayerIdleState IdleState ;
    public PlayerRunningState RunningState ;
    public PlayerDeadState DeadState ;
    public PlayerAutoAttackState AutoAttackState ;
    public PlayerFirstAbilityState FirstAbilityState ;
    public PlayerSecondAbilityState SecondAbilityState ;
    public PlayerThirdAbilityState ThirdAbilityState ;
    public PlayerUltimateState UltimateState ;


    //Variables to store player input values
    Vector2 _currentMovementInput;
    public Vector3 CurrentMovement;

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
    public Animator Animator;
    public NetworkAnimator NetworkAnimator;

    // Variables to handle Gravity
    // Create a Static Class Called world's physics to store gravity and such.
    [SerializeField] Vector3 _gravity = new Vector3(0.0f , -9.8f , 0.0f); 




    //StateMachine Variables (logic and animation)
    public bool ReadyToSwitchState;
    public bool IsCastingAnAbility ;
    public bool IsMovementPressed ;
    private bool _isAimingPressed ;
    public bool IsAutoAiming ;




    //Variables to cache Instances 
    public CharacterController CharacterController;
    private Player_Controls _playerControls;
    public CooldownSystem CooldownSystem;
    public CooldownUIManager CooldownUIManager;
    public PlayerAnimationsLength AnimationsLength;

    //Auto Aiming vars
    private float _smallestDistance;
    public Transform TargetPos;
    private float _distance;




    #endregion

    #region Execution

    public void Awake()
    {
        CashingPlayerInstances() ;

        CurrentState = IdleState;
        CurrentState.EnterState();

        // we`ll see if we don't use it in Enemy we move it to Player
        ReadyToSwitchState = true;
        IsCastingAnAbility = false; 

        _isAimingPressed = false;
        SubscriptionToPlayerControls();

        CharacterController.enableOverlapRecovery = true ;

    }

    private void CashingPlayerInstances()
    {
        IdleState = GetComponent<PlayerIdleState>();
        RunningState = GetComponent<PlayerRunningState>();
        DeadState = GetComponent<PlayerDeadState>();

        Animator = GetComponent<Animator>();
        NetworkAnimator = GetComponent<NetworkAnimator>();

        AutoAttackState = GetComponent<PlayerAutoAttackState>();
        FirstAbilityState = GetComponent<PlayerFirstAbilityState>();
        SecondAbilityState = GetComponent<PlayerSecondAbilityState>();
        ThirdAbilityState = GetComponent<PlayerThirdAbilityState>();
        UltimateState = GetComponent<PlayerUltimateState>();

        CharacterController = GetComponent<CharacterController>();
        HitBoxes = GetComponentInChildren<PlayerHitBoxesAndColliders>();
        CooldownSystem = GetComponent<CooldownSystem>();
        CooldownUIManager = FindObjectOfType<CooldownUIManager>();

        AnimationsLength = GetComponent<PlayerAnimationsLength>();

    }

    private void OnAimingInput(InputAction.CallbackContext context)
    {
        if (ActiveHitBox == null ) return;
        HandleAiming();
    }

    private void HandleAiming()
    {
        ReadAndSetAimingInput();
        HandleAimingRotation();
        if (ActiveHitBox.Movable) HandleAimingLocation();
    }

    private void ReadAndSetAimingInput()
    {
        _currentAimingInput = _playerControls.DefaultMap.Aim.ReadValue<Vector2>();
        _currentAimingAt.x = _currentAimingInput.x;
        _currentAimingAt.y = 0.0f;
        _currentAimingAt.z = _currentAimingInput.y;

        _isAimingPressed =  _currentAimingInput.x != 0 ||
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

    public void RotatePlayerToHitBox()
    {
        transform.LookAt(ActiveHitBox.transform);
        IsAutoAiming = false ;
        HitBoxes.transform.localPosition = Vector3.zero ;
    }

    public void SimpleMove(float movementSpeed)
    {
        ReadAndSetMovementInput();
        CharacterController.SimpleMove(CurrentMovement * movementSpeed);
    }

    public void ReadAndSetMovementInput()
    {
        // TODO make it a local variable.
        _currentMovementInput = _playerControls.DefaultMap.Move.ReadValue<Vector2>();
        CurrentMovement.x = _currentMovementInput.x;
        CurrentMovement.z = _currentMovementInput.y;

        IsMovementPressed = _currentMovementInput.x != 0 ||
                            _currentMovementInput.y != 0 ;
    }

    public void RotatePlayer(float rotationSpeed, Vector3 movementDirection)
    {
        //the change in position our character should point to
        _positionToLookAt.x = movementDirection.x;
        _positionToLookAt.y = 0.0f;
        _positionToLookAt.z = movementDirection.z;

        //creates a new rotation bases on where the player is currently moving
        _targetRotation = Quaternion.LookRotation(_positionToLookAt);
        transform.rotation = Quaternion.Slerp(  transform.rotation,
                                                _targetRotation,
                                                rotationSpeed * Time.deltaTime);

    }

    [Client(RequireOwnership = true)]
    private void OnMovementInput(InputAction.CallbackContext context)
    {
        if (CurrentState != RunningState) SwitchState(RunningState);
    }

    [Client(RequireOwnership = true)]
    private void OnAutoAttackInputStarted(InputAction.CallbackContext context)
    {
        AutoAttackState.Continue = false;
        _aimingRange = Statics.AutoAttackRange;
        ActiveHitBox = HitBoxes.HitBoxAA;
        ActiveAttackCollider = HitBoxes.AttackColliderAA ;
        if (CurrentState != AutoAttackState) SwitchState(AutoAttackState);
    }
    [Client(RequireOwnership = true)]
    private void OnAutoAttackInputPerformed(InputAction.CallbackContext context)
    {
        if (AutoAttackState.Continue == false) AutoAttackState.Continue = true ;
    }
    [Client(RequireOwnership = true)]
    private void OnAutoAttackInputcanceled(InputAction.CallbackContext context)
    {
        AutoAttackState.Continue = false ;
    }

    [Client(RequireOwnership = true)]
    private void OnFirstAbilityInputStarted(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(FirstAbilityState.Id)) return;
        _aimingRange = Statics.FirstAbilityRange;
        HitBoxes.HitBox1.gameObject.SetActive(true);
        ActiveHitBox = HitBoxes.HitBox1;
        ActiveAttackCollider = HitBoxes.AttackCollider1 ;
    }
    [Client(RequireOwnership = true)]
    private void OnFirstAbilityInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(FirstAbilityState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility ) return;

        HitBoxes.HitBox1.gameObject.SetActive(true);
    }
    [Client(RequireOwnership = true)]
    private void OnFirstAbilityInputCanceled(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(FirstAbilityState.Id)) return ;
        if (CurrentState != FirstAbilityState) SwitchState(FirstAbilityState);
        HitBoxes.HitBox1.gameObject.SetActive(false);
    }

    [Client(RequireOwnership = true)]
    private void OnSecondAbilityInputStarted(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(SecondAbilityState.Id)) return;
        _aimingRange = Statics.SecondAbilityRange;
        ActiveHitBox = HitBoxes.HitBox2;
        if (!_isAimingPressed) AutoAim();
        HitBoxes.HitBox2.gameObject.SetActive(true);
        ActiveAttackCollider = HitBoxes.AttackCollider2 ;
    }
    [Client(RequireOwnership = true)]
    private void OnSecondAbilityInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(SecondAbilityState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility) return;
        IsAutoAiming = false ;
        HitBoxes.HitBox2.gameObject.SetActive(true);
    }
    [Client(RequireOwnership = true)]
    private void OnSecondAbilityInputCanceled(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(SecondAbilityState.Id)) return;
        RotatePlayerToHitBox();
        if (CurrentState != SecondAbilityState) SwitchState(SecondAbilityState);
        HitBoxes.HitBox2.gameObject.SetActive(false);
    }

    [Client(RequireOwnership = true)]
    private void OnThirdAbilityInputStarted(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(ThirdAbilityState.Id)) return;
        _aimingRange = Statics.ThirdAbilityRange;
        ActiveHitBox = HitBoxes.HitBox3;
        if (!_isAimingPressed) AutoAim();
        HitBoxes.HitBox3.gameObject.SetActive(true);
        ActiveAttackCollider = HitBoxes.AttackCollider3 ;
    }
    [Client(RequireOwnership = true)]
    private void OnThirdAbilityInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(ThirdAbilityState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility) return;
                IsAutoAiming = false ;
        HitBoxes.HitBox3.gameObject.SetActive(true);
    }
    [Client(RequireOwnership = true)]
    private void OnThirdAbilityInputCanceled(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(ThirdAbilityState.Id)) return;
        if (CurrentState != SecondAbilityState) SwitchState(ThirdAbilityState);
        RotatePlayerToHitBox();
        HitBoxes.HitBox3.gameObject.SetActive(false);
    }

    [Client(RequireOwnership = true)]
    private void OnUltimateInputStarted(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(UltimateState.Id)) return;
        _aimingRange = Statics.UltimateAbilityRange;
        ActiveHitBox = HitBoxes.HitBoxU;
        if (!_isAimingPressed) AutoAim();
        HitBoxes.HitBoxU.gameObject.SetActive(true);
          ActiveAttackCollider = HitBoxes.AttackColliderU ;
    }
    [Client(RequireOwnership = true)]
    private void OnUltimateInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(UltimateState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility) return;
        HitBoxes.HitBoxU.gameObject.SetActive(true);
        IsAutoAiming = false ;
    }
    [Client(RequireOwnership = true)]
    private void OnUltimateInputCanceled(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(UltimateState.Id)) return;
        if (CurrentState != SecondAbilityState) SwitchState(UltimateState);
        RotatePlayerToHitBox();
        HitBoxes.HitBoxU.gameObject.SetActive(false);
    }

    [Client(RequireOwnership = true)]
    public void Update()
    {
        CurrentState.UpdateState();

        if (!CharacterController.isGrounded)
        {
            CharacterController.Move(_gravity * Time.deltaTime);
        }

        if (CurrentState != IdleState && !IsCastingAnAbility && !IsMovementPressed ) SwitchState(IdleState);
        if ( _isAimingPressed) HandleAiming();
        else if (!IsAutoAiming) HitBoxes.transform.localEulerAngles = Vector3.zero;
 

    }

    public void SwitchState(BaseState state)
    {   
        // I think it can be deleted because we never call it if CurrentState == DeadState
        if (CurrentState == DeadState) return ; 
        
        if (ReadyToSwitchState || state == DeadState)
        {
            CurrentState.ExitState();
            CurrentState = state;
            CurrentState.EnterState();
        }

    }

    
    //Auto Aiming 
    public float AutoAim()
    {
        _smallestDistance = _aimingRange;
        TargetPos = null ;

        //extract this as a variable ?
        Collider[] _nearbyEnemies = Physics.OverlapSphere(this.transform.position, _aimingRange);
        foreach (Collider enemy in _nearbyEnemies)
        {
            if (enemy.TryGetComponent<EnemyBase>(out EnemyBase target) )
            {
                _distance = Vector3.Distance(this.transform.position, target.transform.position);
                if (_distance > _smallestDistance ) continue ;
                _smallestDistance = _distance;
                TargetPos = target.transform;
                
            }
        }

        if (TargetPos == null) return -1f ;
        
        IsAutoAiming = true ;
        Debug.Log("Auto aimed to: " + TargetPos.name + " distance is: " + _smallestDistance);
        HitBoxes.transform.LookAt(TargetPos);
        if (ActiveHitBox.Movable) HitBoxes.transform.position = TargetPos.position;

        return _smallestDistance ;
    }
 
    // PlayerControls Setup
    private void SubscriptionToPlayerControls()
    {
        _playerControls = new Player_Controls();
        var map = _playerControls.DefaultMap ;

        var action = map.Move ;
        action.started += OnMovementInput;
        action.canceled += OnMovementInput;
        action.performed += OnMovementInput;

        action = map.Aim ;
        action.started += OnAimingInput;
        action.canceled += OnAimingInput;
        action.performed += OnAimingInput;

        action = map.AutoAttack;
        action.started += OnAutoAttackInputStarted;
        action.performed += OnAutoAttackInputPerformed;
        action.canceled += OnAutoAttackInputcanceled;

        action = map.FirstAbility;
        action.started += OnFirstAbilityInputStarted;
        action.performed += OnFirstAbilityInputPerformed;
        action.canceled += OnFirstAbilityInputCanceled;

        action = map.SecondAbility;
        action.started += OnSecondAbilityInputStarted; 
        action.performed += OnSecondAbilityInputPerformed; 
        action.canceled += OnSecondAbilityInputCanceled; 
        
        action = map.ThirdAbility;
        action.started += OnThirdAbilityInputStarted;
        action.performed += OnThirdAbilityInputPerformed;
        action.canceled += OnThirdAbilityInputCanceled;

        action = map.Ultimate;
        action.started += OnUltimateInputStarted;
        action.performed += OnUltimateInputPerformed;
        action.canceled += OnUltimateInputCanceled;        
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

}
