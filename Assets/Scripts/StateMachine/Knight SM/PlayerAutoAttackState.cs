using UnityEngine;

public class PlayerAutoAttackState : BaseState
{
    //Name of The Ability
    public string AbilityName = "Auto Attack";

    //Game Design Vars, Mak a stat Script maybe
    [SerializeField] private float _animationSpeed = 1.5f;
    [SerializeField] private float _dashingMovementSpeed = 10f;
    [SerializeField] private float _dashingTime = 0.25f;
    [SerializeField] private float _rotationSpeed = 10.0f;
    [SerializeField] float _damage1 = 10.0f;
    [SerializeField] float _damage2 = 5.0f;
    [SerializeField] float _damage3 = 20.0f;
    
    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables 
    public bool Continue;
    bool _dashed ;

    //Variables to store optimized Setter / getter parameter IDs
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
        _dashed = false;
        Invoke(nameof(Dashed), _dashingTime);

        _player.Animator.CrossFade(_autoAttack1Hash, 0.2f);
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;

        _player.HitBoxes.Targets.Clear();
        _player.ActiveAttackCollider.Collider.enabled = true ;
    }

    public override void UpdateState()
    {
        if (_dashed || !_player.IsMovementPressed) return ;
        _player.Move(_dashingMovementSpeed);
        _player.Rotate(_rotationSpeed);
    }

    public override void ExitState()
    {
        //enable SwitchState
    }

    void Dashed()
    {
        _dashed = true;
    }

    public void Attack1Complete()
    {
        if (Continue)
        {
            Invoke(nameof(Attack2Complete), _player.AnimationsLength.AutoAttack2Duration / _animationSpeed);
            _dashed = false;
            Invoke(nameof(Dashed), _dashingTime);

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
            _dashed = false;
            Invoke(nameof(Dashed), _dashingTime);

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
            _dashed = false;
            Invoke(nameof(Dashed), _dashingTime);

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

        _player.ActiveAttackCollider.Collider.enabled = false ;
    }

    void AutoAttack1Event()
    {
        DoDamage(_damage1);
    }
    void AutoAttack2Event()
    {
        DoDamage(_damage2); 
    }
    void AutoAttack3Event()
    {
        DoDamage(_damage3);
    }

    private void DoDamage(float damage)
    {
        foreach (EnemyBase enemy in _player.HitBoxes.Targets)
        {
            enemy.TakeDamage(damage);
        }
    }


}

