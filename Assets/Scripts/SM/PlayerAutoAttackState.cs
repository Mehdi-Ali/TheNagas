using UnityEngine;

public class PlayerAutoAttackState : PlayerBaseState
{
    //Name of The Abbility
    public string AbbilityName = "Auto Attack";

    //Game Designe Vars, Mak a stat Script maybe
    [SerializeField] private float _animationSpeed = 1.0f;
    [SerializeField] private float _dashingMovementSpeed = 7.5f;
    
    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables 
    public bool Continue;

    //Variables to store omptimized Setter / getter parameter IDs
    int _autoAttackHash;
    int _AutoAttackMultiplierHash;

    private void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _autoAttackHash = Animator.StringToHash("AutoAttack");
        _AutoAttackMultiplierHash = Animator.StringToHash("AutoAttack_Multiplier");

        _player.Animator.SetFloat(_AutoAttackMultiplierHash, _animationSpeed);
    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;

        Invoke(nameof(AttackComplete), _player.AnimationsLength.AutoAttackDuration / _animationSpeed);

        _player.Animator.CrossFade(_autoAttackHash, 0.1f);
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        //enable SwitchState
    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
    }

    void AutoAttack1Event()
    {
        Debug.Log("1");
        //Activat Collider + deal damage 
    }
    void AutoAttack2Event()
    {
        Debug.Log("2");
        //Activat Collider + deal damage 
    }
    void AutoAttack3Event()
    {
        Debug.Log("3");
        //Activat Collider + deal damage 
    }

        void AutoAttack1BreakEvent()
    {
        Debug.Log("B1");
        Debug.Log(Continue);
        if (!Continue) AttackComplete() ;
    }
        void AutoAttack2BreakEvent()
    {
        Debug.Log("B2");
        if (!Continue) AttackComplete();
    }
}

