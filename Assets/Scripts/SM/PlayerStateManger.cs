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
    PlayerBaseState _currentState;


    public PlayerIdleState IdleState ;
    public PlayerRuningState RuningState ;
    public PlayerDeadState DeadState ;
    public PlayerSecondAbilityState SecondAbilityState ;

    //Imported -------------------------
    //Game Designe Vars, Mak a stat Script maybe
    [SerializeField] public float MovementSpeed = 5.0f;
    [SerializeField] public float RotationSpeed = 10.0f;


    //Variables to cache Instances 
    public CharacterController CharactherController;
    private Player_Controls _playerControls;
    public Animator Animator;
    public NetworkAnimator NetworkAnimator;






    //StateMachine Variables (logic and animation)
    public bool ReadyToSwitchState;
    public bool IsStationary ;
    bool _isMovementPressed;
    bool _isRunning = false;
    public bool _TempDeadStateSim = false;
    bool alreadyDead = false;

    //Variables to store omptimized Setter / getter parameter IDs
    int _AutoAttackHash;
    int _FirstAbilityHash;
    int _SecondAbilityHash;
    int _ThirdAbilityHash;
    int _UltimateHash;

    #endregion

    #region Execution

    private void Awake()
    {
        //Setup states
        IdleState = GetComponent<PlayerIdleState>();
        RuningState = GetComponent<PlayerRuningState>();
        DeadState = GetComponent<PlayerDeadState>();
        SecondAbilityState = GetComponent<PlayerSecondAbilityState>();
        
        _currentState = IdleState;
        _currentState.EnterState(this);
        ReadyToSwitchState = true;
        IsStationary = false ;

        //caching Instances 
        CharactherController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        NetworkAnimator = GetComponent<NetworkAnimator>();

        //caching Hashes
        _AutoAttackHash = Animator.StringToHash("AutoAttack");
        _FirstAbilityHash = Animator.StringToHash("FirstAbility");
        _ThirdAbilityHash = Animator.StringToHash("ThirdAbility");
        _UltimateHash = Animator.StringToHash("Ultimate");


        SubscriptionToPlayerControls();

    }

    private void SubscriptionToPlayerControls()
    {
        //player Input callbacks.
        _playerControls = new Player_Controls();

        _playerControls.DefaultMap.Move.started += OnMovementInput;
        _playerControls.DefaultMap.Move.canceled += OnMovementInput;
        _playerControls.DefaultMap.Move.performed += OnMovementInput;

        _playerControls.DefaultMap.SecondAbility.performed += OnSecondAbilityUnput; //TODO add the othersx 5
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        if (_currentState != RuningState) SwitchState(RuningState);
        RuningState.OnMovementInput(context);
    }

    private void OnSecondAbilityUnput(InputAction.CallbackContext context)
    {
        IsStationary = true ;
        if (_currentState != SecondAbilityState) SwitchState(SecondAbilityState);
    }

    void Update()
    {
        _currentState.UpdateState(this);

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
        _currentState.ExitState(this);
        _currentState = state;
        _currentState.EnterState(this);

    }

    #endregion
}
