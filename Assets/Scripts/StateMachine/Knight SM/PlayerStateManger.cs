using System;
using FishNet.Component.Animating;
using FishNet.Connection;
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
    public CooldownSystem CooldownSystem;
    public PlayerHitBoxesAndColliders HitBoxes;
    public PlayerAttackCollider ActiveAttackCollider;
    public NetworkAnimator NetworkAnimator;
    public PlayerAnimationsLength AnimationsLength; 
    public Animator Animator;

    //Variables to handle movement.
    private MoveData moveData;
    public  float MovementSpeed ;

    //Variables to handle Abilities.
    private AbilityData abilityData;
    public Vector3 TargetPosition;

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
    public bool NeedsMoveAndRotate;
    private bool _isAimingPressed ;

    //Auto Aiming vars
    //public bool IsAutoAiming ;
    private float _smallestDistance;
    public Transform TargetPos;
    private float _distance;

    // Variables to handle Gravity
    // Create a Static Class Called world's physics to store gravity and such.
    [SerializeField] Vector3 _gravity = new Vector3(0.0f , -9.8f , 0.0f);

    // Network Variables.
    private bool _subscribed;
    

    
    #region Only Client Vars
    #if !UNITY_SERVER

    private Player_Controls _playerControls;
    public CooldownUIManager CooldownUIManager;
    public PlayerHitBox ActiveHitBox;

    #endif
    #endregion

    #endregion

    #region Execution

    public void Awake()
    {
        CashingPlayerInstances() ;
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        SubscribeToTimeManager(true);

        CurrentState = IdleState;
        CurrentState.EnterState();

        ReadyToSwitchState = true;
        IsCastingAnAbility = false;
        NeedsMoveAndRotate = false;
        _isAimingPressed = false;

        if(IsServer || Owner.IsLocalClient)
            CharacterController.enabled = true ;
            CharacterController.enableOverlapRecovery = true ;

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
        HandleAimingRotationAndLocation();
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

    private void HandleAimingRotationAndLocation()
    {
        _currentAimingRotation = Quaternion.LookRotation(_currentAimingAt);
        HitBoxes.transform.rotation = Quaternion.Slerp(  HitBoxes.transform.rotation,
                                                        _currentAimingRotation,
                                                        100f );

        if (ActiveHitBox.Movable) 
            HitBoxes.transform.localPosition = _currentAimingAt * _aimingRange ;
    }

    public void RotatePlayerToHitBox(Vector3 position)
    {
        transform.LookAt(position);
        //IsAutoAiming = false ;
        if (IsOwner)
            HitBoxes.transform.localPosition = Vector3.zero ;
    }

    #region Movement

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        StartRunningState();
    }

    [ServerRpc(RunLocally = true)]
    private void StartRunningState()
    {
        // TODO check if [ServerRpc(RunLocally = true)] only runs on server and owner
        if ( !IsOwner && !IsServer ) return ;

        IsMovementPressed = true ;
        if (CurrentState != RunningState) SwitchState(RunningState);
    }

    #endregion

    private void SetupOnInputStarted(string cooldownId, float aimingRange, PlayerHitBox activeHitBox)
    {
        if (CooldownSystem.IsOnCooldown(cooldownId)) return;
        _aimingRange = aimingRange;
        ActiveHitBox = activeHitBox;
        //if (!_isAimingPressed) AutoAim();
        ActiveHitBox.gameObject.SetActive(true);
    }
    
    private void SetupOnInputPerformed(string cooldownId)
    {
        if (CooldownSystem.IsOnCooldown(cooldownId) ||
                !ReadyToSwitchState || IsCastingAnAbility) return;
        //IsAutoAiming = false ;
    }

    private void SetupOnInputCanceled(Abilities ability)
    {
        // TODO the active HitBox is already set in the OnInputStart so maybe this block is useless.
        switch (ability)
        {
            case Abilities.First:
                ActiveHitBox = HitBoxes.HitBox1 ;
                break;
                
            case Abilities.Second:
                ActiveHitBox = HitBoxes.HitBox2 ;
                break;

            case Abilities.Third:
                ActiveHitBox = HitBoxes.HitBox3 ;
                break;

            case Abilities.Ultimate:
                ActiveHitBox = HitBoxes.HitBoxU ;
                break;
        }

        ActiveHitBox.gameObject.SetActive(true);
        RpcSetupOnInputCanceled(ability, ActiveHitBox.transform.position);
    }

    [ServerRpc]
    private void RpcSetupOnInputCanceled(Abilities ability, Vector3 target)
    {

        string cooldownId = null;
         
        switch (ability)
        {
            case Abilities.First:
                cooldownId = FirstAbilityState.Id;
                break;
                
            case Abilities.Second:
                cooldownId = SecondAbilityState.Id;
                break;

            case Abilities.Third:
                cooldownId = ThirdAbilityState.Id;
                break;

            case Abilities.Ultimate:
                cooldownId = UltimateState.Id;
                break;
        }

        RpcDoAbility(Owner, ability, target, CooldownSystem.IsOnCooldown(cooldownId));
    }

    [TargetRpc(RunLocally = true)]
    private void RpcDoAbility(NetworkConnection conn, Abilities ability, Vector3 target, bool isOnCooldown)
    {
        TargetPosition = target ;
        
        if (!isOnCooldown)
        {
            BaseState abilityState = null;
            switch (ability)
            {
                case Abilities.First:
                    ActiveAttackCollider = HitBoxes.AttackCollider1;
                    abilityState = FirstAbilityState;
                    break;
                    
                case Abilities.Second:
                    ActiveAttackCollider =  HitBoxes.AttackCollider2;
                    abilityState = SecondAbilityState;
                    break;

                case Abilities.Third:
                    ActiveAttackCollider =  HitBoxes.AttackCollider3;
                    abilityState = ThirdAbilityState;
                    break;

                case Abilities.Ultimate:
                    ActiveAttackCollider =  HitBoxes.AttackColliderU;
                    abilityState = UltimateState;
                    break;
            } 

            if (CurrentState != abilityState) SwitchState(abilityState);
            RotatePlayerToHitBox(target);

            if (IsOwner)
                ActiveHitBox.gameObject.SetActive(false);
        }
    }
    
    #region 1st Ability
    
    private void OnFirstAbilityInputStarted(InputAction.CallbackContext context)
    {
        SetupOnInputStarted(FirstAbilityState.Id, Statics.FirstAbilityRange, HitBoxes.HitBox1);
    }

    private void OnFirstAbilityInputPerformed(InputAction.CallbackContext context)
    {
        SetupOnInputPerformed(FirstAbilityState.Id);
    }
    
    private void OnFirstAbilityInputCanceled(InputAction.CallbackContext context)
    {
        SetupOnInputCanceled(Abilities.First);
    }

    #endregion

    #region 2nd Ability

    private void OnSecondAbilityInputStarted(InputAction.CallbackContext context)
    {
        SetupOnInputStarted(SecondAbilityState.Id, Statics.SecondAbilityRange, HitBoxes.HitBox2);
    }

    private void OnSecondAbilityInputPerformed(InputAction.CallbackContext context)
    {
        SetupOnInputPerformed(SecondAbilityState.Id);
    }

    private void OnSecondAbilityInputCanceled(InputAction.CallbackContext context)
    {
        SetupOnInputCanceled(Abilities.Second);
    }

    #endregion

    #region 3rd Ability

    private void OnThirdAbilityInputStarted(InputAction.CallbackContext context)
    {
        SetupOnInputStarted(ThirdAbilityState.Id, Statics.ThirdAbilityRange, HitBoxes.HitBox3);
    }
    private void OnThirdAbilityInputPerformed(InputAction.CallbackContext context)
    {
        SetupOnInputPerformed(ThirdAbilityState.Id);
    }
    private void OnThirdAbilityInputCanceled(InputAction.CallbackContext context)
    {
        SetupOnInputCanceled(Abilities.Third);
    }


    #endregion

    #region  NOT YET

    private void OnAutoAttackInputStarted(InputAction.CallbackContext context)
    {
        AutoAttackState.Continue = false;
        _aimingRange = Statics.AutoAttackRange;
        ActiveHitBox = HitBoxes.HitBoxAA;
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

    private void OnUltimateInputStarted(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(UltimateState.Id)) return;
        _aimingRange = Statics.UltimateAbilityRange;
        ActiveHitBox = HitBoxes.HitBoxU;
        // if (!_isAimingPressed) AutoAim();
        HitBoxes.HitBoxU.gameObject.SetActive(true);
          ActiveAttackCollider = HitBoxes.AttackColliderU ;
    }
    private void OnUltimateInputPerformed(InputAction.CallbackContext context)
    {
        if (    CooldownSystem.IsOnCooldown(UltimateState.Id) ||
                !ReadyToSwitchState || IsCastingAnAbility) return;
        HitBoxes.HitBoxU.gameObject.SetActive(true);
        //IsAutoAiming = false ;
    }
    private void OnUltimateInputCanceled(InputAction.CallbackContext context)
    {
        if (CooldownSystem.IsOnCooldown(UltimateState.Id)) return;
        if (CurrentState != SecondAbilityState) SwitchState(UltimateState);
        RotatePlayerToHitBox(ActiveHitBox.transform.position);
        HitBoxes.HitBoxU.gameObject.SetActive(false);
    }

    #endregion

    [Client(RequireOwnership = true)]
    public void Update()
    {
        CurrentState.UpdateState();

        
        if ( _isAimingPressed) HandleAiming();
        //else if (!IsAutoAiming) HitBoxes.transform.localEulerAngles = Vector3.zero;

    }

    private void TimeManager_OnTick()
    {
        if (!IsCastingAnAbility)
        {
            if(CurrentState != IdleState && !IsMovementPressed)
                SwitchState(IdleState);

            if (CurrentState != RunningState && IsMovementPressed )
                SwitchState(RunningState);
        }
                
        CurrentState.OnTickState();

        if (IsOwner) ClientSideLogic();
        if (IsOwner || IsServer) SharedLogic();
        if (IsServer) ServerSideLogic();
    }

    private void ClientSideLogic()
    {
        Reconciliate(default, false);
        MoveAndRotate(moveData, false);
    }

    private void SharedLogic()
    {
        if (!CharacterController.isGrounded)
            CharacterController.Move(_gravity * (float)base.TimeManager.TickDelta);
    }

    private void ServerSideLogic()
    {
        MoveAndRotate(default, true);
        ReconcileMoveData reconData = new ReconcileMoveData(transform.position, transform.rotation);
        Reconciliate(reconData, true);
    }

    [Replicate]
    private void MoveAndRotate(MoveData moveData, bool asServer, bool replaying = false)
    {
        if (!IsMovementPressed || (IsCastingAnAbility && !NeedsMoveAndRotate) ) return;

        Vector3 move = new Vector3(moveData.XAxis, 0f, moveData.ZAxis).normalized;
        CharacterController.Move(move * MovementSpeed * (float)base.TimeManager.TickDelta);

        Rotate(move);

    }

    private void Rotate(Vector3 move)
    {
        Quaternion targetRotation = Quaternion.LookRotation(move);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * (float)base.TimeManager.TickDelta);
    }

    [Reconcile]
    private void Reconciliate(ReconcileMoveData recData, bool asServer)
    {
        if (!IsMovementPressed || (IsCastingAnAbility && !NeedsMoveAndRotate) ) return;

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

    public void SetMoveAndRotateSpeed(float movementSpeed, float rotationSpeed )
    {
        if ( !IsOwner && !IsServer ) return ;

        MovementSpeed = movementSpeed;
        if (rotationSpeed == 0f) return ;
        RotationSpeed = rotationSpeed;
    }
    
    // public float AutoAim()
    // {
    //     _smallestDistance = _aimingRange;
    //     TargetPos = null ;

    //     //extract this as a variable ?
    //     Collider[] _nearbyEnemies = Physics.OverlapSphere(this.transform.position, _aimingRange);
    //     foreach (Collider enemy in _nearbyEnemies)
    //     {
    //         if (enemy.TryGetComponent<EnemyBase>(out EnemyBase target) )
    //         {
    //             _distance = Vector3.Distance(this.transform.position, target.transform.position);
    //             if (_distance > _smallestDistance ) continue ;
    //             _smallestDistance = _distance;
    //             TargetPos = target.transform;
                
    //         }
    //     }

    //     if (TargetPos == null) return -1f ;
        
    //     IsAutoAiming = true ;
    //     Debug.Log("Auto aimed to: " + TargetPos.name + " distance is: " + _smallestDistance);
    //     HitBoxes.transform.LookAt(TargetPos);
    //     if (ActiveHitBox.Movable) HitBoxes.transform.position = TargetPos.position;

    //     return _smallestDistance ;
    // }
 
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
