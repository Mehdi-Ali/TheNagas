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
    public PlayerAutoAttackState AutoAttackState ;
    public PlayerFirstAbilityState FirstAbilityState ;
    public PlayerSecondAbilityState SecondAbilityState ;
    public PlayerThirdAbilityState ThirdAbilityState ;
    public PlayerUltimateState UltimateState ;
    public PlayerDeadState DeadState ;


    //StateMachine Variables (logic and animation)
    public bool ReadyToSwitchState;
    public bool IsCastingAnAbility ;

    public bool _TempDeadStateSim = false;


    //Variables to cache Instances 
    public CharacterController CharactherController;
    private Player_Controls _playerControls;
    public Animator Animator;
    public NetworkAnimator NetworkAnimator;

    #endregion

    #region Execution

    private void Awake()
    {
        //Setup states
        IdleState = GetComponent<PlayerIdleState>();
        RuningState = GetComponent<PlayerRuningState>();
        AutoAttackState = GetComponent<PlayerAutoAttackState>();
        FirstAbilityState = GetComponent<PlayerFirstAbilityState>();
        SecondAbilityState = GetComponent<PlayerSecondAbilityState>();
        ThirdAbilityState = GetComponent<PlayerThirdAbilityState>();
        UltimateState = GetComponent<PlayerUltimateState>();
        DeadState = GetComponent<PlayerDeadState>();
        
        _currentState = IdleState;
        _currentState.EnterState();
        ReadyToSwitchState = true;
        IsCastingAnAbility = false ;

        //caching Instances 
        CharactherController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        NetworkAnimator = GetComponent<NetworkAnimator>();

        SubscriptionToPlayerControls();

    }

    private void SubscriptionToPlayerControls()
    {
        //player Input callbacks.
        _playerControls = new Player_Controls();

        _playerControls.DefaultMap.Move.started += OnMovementInput;
        _playerControls.DefaultMap.Move.canceled += OnMovementInput;
        _playerControls.DefaultMap.Move.performed += OnMovementInput;

        _playerControls.DefaultMap.AutoAttack.performed += OnAutoAttackInput;
        _playerControls.DefaultMap.FirstAbility.performed += OnFirstAbilityInput; 
        _playerControls.DefaultMap.SecondAbility.performed += OnSecondAbilityInput; 
        _playerControls.DefaultMap.ThirdAbility.performed += OnThirdAbilityInput; 
        _playerControls.DefaultMap.Ultimate.performed += OnUltimateInput; 
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        if (_currentState != RuningState) SwitchState(RuningState);
        RuningState.OnMovementInput(context);
    }

    private void OnAutoAttackInput(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true ;
        if (_currentState != AutoAttackState) SwitchState(AutoAttackState);
    }

    private void OnFirstAbilityInput(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (_currentState != FirstAbilityState) SwitchState(FirstAbilityState);
    }

    private void OnSecondAbilityInput(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (_currentState != SecondAbilityState) SwitchState(SecondAbilityState);
    }

    private void OnThirdAbilityInput(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (_currentState != ThirdAbilityState) SwitchState(ThirdAbilityState);
    }

    private void OnUltimateInput(InputAction.CallbackContext context)
    {
        IsCastingAnAbility = true;
        if (_currentState != UltimateState) SwitchState(UltimateState);
    }

    void Update()
    {
        _currentState.UpdateState();

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
        _currentState.ExitState();
        _currentState = state;
        _currentState.EnterState();

    }

    #endregion
}
