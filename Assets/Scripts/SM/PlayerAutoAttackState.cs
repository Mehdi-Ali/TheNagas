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
    int _autoAttack1Hash;
    int _autoAttack2Hash;
    int _autoAttack3Hash;
    int _AutoAttackMultiplierHash;

    private void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _autoAttack1Hash = Animator.StringToHash("AutoAttack1");
        _autoAttack2Hash = Animator.StringToHash("AutoAttack2");
        _autoAttack3Hash = Animator.StringToHash("AutoAttack3");
        _AutoAttackMultiplierHash = Animator.StringToHash("AutoAttack_Multiplier");

        _player.Animator.SetFloat(_AutoAttackMultiplierHash, _animationSpeed);
    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;

        Invoke(nameof(Attack1Complete), _player.AnimationsLength.AutoAttack1Duration / _animationSpeed);

        _player.Animator.CrossFade(_autoAttack1Hash, 0.2f);
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

    public void Attack1Complete()
    {
        if (Continue)
        {
            Invoke(nameof(Attack2Complete), _player.AnimationsLength.AutoAttack2Duration / _animationSpeed);

            _player.Animator.CrossFade(_autoAttack2Hash, 0.0f);
        }
        else
        {
            AttackComplete();   
        }
    }

    public void Attack2Complete()
    {
        if (Continue)
        {
            Invoke(nameof(Attack3Complete), _player.AnimationsLength.AutoAttack3Duration / _animationSpeed);

            _player.Animator.CrossFade(_autoAttack3Hash, 0.0f);
        }
        else
        {
            AttackComplete();
        }
    }

    public void Attack3Complete()
    {
        if (Continue)
        {
            Invoke(nameof(Attack1Complete), _player.AnimationsLength.AutoAttack1Duration / _animationSpeed);

            _player.Animator.CrossFade(_autoAttack1Hash, 0.2f);
        }
        else
        {
            AttackComplete();
        }
    }

    public void AttackComplete()
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



}

