using FishNet.Component.Animating;
using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateManger : NetworkBehaviour
{

    #region Scriptable Objects

    [Header("Scriptable Objects")]
    [SerializeField]
    public PlayerStaticsScriptableObject Statics ;

    [Space(10)]

    #endregion

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

    // Class references to cache Instances.
    public CharacterController CharacterController;
    private Player_Controls _playerControls;
    public CooldownSystem CooldownSystem;
    public CooldownUIManager CooldownUIManager;
    public PlayerAnimationsLength AnimationsLength;
    public PlayerHitBoxesAndColliders HitBoxes;
    public PlayerHitBox ActiveHitBox;
    public PlayerAttackCollider ActiveAttackCollider;
    public Animator Animator;
    public NetworkAnimator NetworkAnimator;

    //Variables to handle movement.
    private MoveData moveData;
    public  float MovementSpeed ;

    //Variables to handle Rotation
    Vector3 _positionToLookAt;
    Quaternion _targetRotation;
    public float RotationSpeed;

    //Variables to handle Aiming
    Vector2 _currentAimingInput ;
    Vector3 _currentAimingAt ;
    Quaternion _currentAimingRotation;
    private float _aimingRange ;

    //StateMachine Variables (logic and animation)
    public bool ReadyToSwitchState;
    public bool IsCastingAnAbility ;
    public bool IsMovementPressed ;
    private bool _isAimingPressed ;

    //Auto Aiming vars
    public bool IsAutoAiming ;
    private float _smallestDistance;
    public Transform TargetPos;
    private float _distance;

    // Variables to handle Gravity
    // Create a Static Class Called world's physics to store gravity and such.
    [SerializeField] Vector3 _gravity = new Vector3(0.0f , -9.8f , 0.0f);

    // Network Variables.
    private bool _subscribed;

    #endregion

    #region Execution

    public void Awake()
    {
        CashingPlayerInstances() ;

        CurrentState = IdleState;
        CurrentState.EnterState();

        
        ReadyToSwitchState = true;
        IsCastingAnAbility = false; 

        _isAimingPressed = false;

        CharacterController.enableOverlapRecovery = true ;

    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        SubscribeToTimeManager(true);

        if(IsServer || Owner.IsLocalClient)
            CharacterController.enabled = true ;

        if (!Owner.IsLocalClient) return ;
        SubscriptionToPlayerControls();
        _playerControls.DefaultMap.Enable();
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        SubscribeToTimeManager(false);

        if (Owner.IsLocalClient)
            _playerControls.DefaultMap.Disable();
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


    [Client(RequireOwnership = true)]
    private void OnMovementInput(InputAction.CallbackContext context)
    {
        ServerSwitchToRunningState();
    }

    [ServerRpc(RunLocally = true)]
    private void ServerSwitchToRunningState()
    {
        IsMovementPressed = true ;
        if (CurrentState != RunningState) SwitchState(RunningState);
    }

    #region  NOT YET

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

    #endregion

    [Client(RequireOwnership = true)]
    public void Update()
    {
        CurrentState.UpdateState();

        if (CurrentState != IdleState && !IsCastingAnAbility && !IsMovementPressed ) SwitchState(IdleState);
        
        if ( _isAimingPressed) HandleAiming();
        else if (!IsAutoAiming) HitBoxes.transform.localEulerAngles = Vector3.zero;

    }

    private void TimeManager_OnTick()
    {
        
        if (IsOwner)
        {
            Reconciliate(default,false);
            CurrentState.OnTickState();
            MoveAndRotate(moveData, false);
        }

        if (IsOwner || IsServer)
        {
            if (!CharacterController.isGrounded)
            {
                CharacterController.Move(_gravity * (float)base.TimeManager.TickDelta);
            }
        }

        if (IsServer)
        {
            MoveAndRotate(default, true);
            ReconcileMoveData reconData = new ReconcileMoveData(transform.position, transform.rotation);
            Reconciliate(reconData, true);
        }
    }

    [Replicate]
    private void MoveAndRotate(MoveData moveData, bool asServer, bool replaying = false)
    {
        if (!IsMovementPressed) return;

        Vector3 move = new Vector3(moveData.XAxis, 0f, moveData.ZAxis).normalized;
        CharacterController.Move(move * MovementSpeed * (float)base.TimeManager.TickDelta);

        Quaternion targetRotation = Quaternion.LookRotation(move);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * (float)base.TimeManager.TickDelta);

    }

    [Reconcile]
    private void Reconciliate(ReconcileMoveData recData, bool asServer)
    {
        transform.position = recData.Position;
        transform.rotation = recData.Rotation;
    }
    
    public void ReadAndSetMovementInput()
    {
        moveData = default;

        var movementInput = _playerControls.DefaultMap.Move.ReadValue<Vector2>();

        float xAxis = movementInput.x;
        float zAxis = movementInput.y;

        IsMovementPressed = xAxis != 0 || zAxis != 0 ;
        ServerSetMovementStatus(IsMovementPressed);

        if (!IsMovementPressed) return;
        moveData = new MoveData(xAxis, zAxis);
    }

    [ServerRpc(RunLocally = true)]
    private void ServerSetMovementStatus(bool isMovementPressed)
    {
        IsMovementPressed = isMovementPressed;
    }

    [ServerRpc(RunLocally = true)]
    public void ServerSetMoveAndRotateSpeed(float movementSpeed, float rotationSpeed)
    {
        MovementSpeed = movementSpeed;
        RotationSpeed = rotationSpeed;
    }
    
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

    #endregion

    #region Subscriptions

    private void SubscribeToTimeManager(bool subscribe)
    {
        if (base.TimeManager == null) return;
        if (subscribe == _subscribed) return;

        _subscribed = subscribe;

        if (subscribe)
            base.TimeManager.OnTick += TimeManager_OnTick;

        else
            base.TimeManager.OnTick -= TimeManager_OnTick;
    }

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
    
    #endregion

}
