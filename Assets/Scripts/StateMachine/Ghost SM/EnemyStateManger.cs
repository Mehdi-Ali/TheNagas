using System;
using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI ;

public class EnemyStateManger : NetworkBehaviour
{
    #region Scriptable Objects

    [Header("Scriptable Objects")]
    [SerializeField]
    public EnemyStaticsScriptableObject Statics ;

    [Space(10)]

    #endregion

    #region Fields and Properties 

    //Initiating the states.
    public BaseState CurrentState;
    public EnemyIdleState IdleState ;
    public EnemyDeadState DeadState ;
    public EnemyRoamingState RoamingState ;
    public EnemyChasingState ChasingState ;
    public EnemyBasicAttackState BasicAttackState ;
    public EnemySuperAttackState SuperAttackState ;


    //Variables to cache Instances 
    public Animator Animator;
    public NetworkAnimator NetworkAnimator;
    public EnemyAnimationsLength AnimationsLength;
    public NavMeshAgent NavAgent;
    public EnemyHitBoxesAndColliders HitBoxes ;
    public EnemyHitBox ActiveHitBox;
    public PlayerAttackCollider ActiveAttackCollider;
    //get animator component here and get it in other instance from here


    // TODO find object when the client is ON
    public PlayerStateManger Player ;

    public PlayerBase TargetPlayer;


    //StateMachine Variables (logic and animation)
    public bool ReadyToSwitchState;
    private bool _subscribed;

    #endregion

    #region Execution

    public void Awake()
    {
        CashingEnemyInstances();
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        SubscribeToTimeManager(true);

        CurrentState = IdleState;
        CurrentState.EnterState();

        ReadyToSwitchState = true;
    }

    private void CashingEnemyInstances()
    {
        IdleState = GetComponent<EnemyIdleState>();
        DeadState = GetComponent<EnemyDeadState>();

        Animator = GetComponent<Animator>();
        NetworkAnimator = GetComponent<NetworkAnimator>();

        RoamingState = GetComponent<EnemyRoamingState>();
        ChasingState = GetComponent<EnemyChasingState>();
        BasicAttackState = GetComponent<EnemyBasicAttackState>();
        SuperAttackState = GetComponent<EnemySuperAttackState>();

        AnimationsLength = GetComponent<EnemyAnimationsLength>();
        NavAgent = GetComponent<NavMeshAgent>();
        HitBoxes = GetComponentInChildren<EnemyHitBoxesAndColliders>();
    }

    public void Update()
    {
        CurrentState.UpdateState();

        //TODO turn the target into an array Targets and make this a for each loop
        
        if (Player == null) {GettingTarget();}
        else if (ReadyToSwitchState &&
                 Vector3.Distance(transform.position, Player.transform.position) < Statics.VisionRange)
        {
            SwitchState(ChasingState);
        }

    }

    private void TimeManager_OnTick()
    {
        CurrentState.OnTickState();
    }

    public void GettingTarget()
    {
        Player = FindObjectOfType<PlayerStateManger>();
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

    #endregion
}
