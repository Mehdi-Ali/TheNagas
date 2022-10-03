using UnityEngine;

public class PlayerAutoAttackState : BaseState
{

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

    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        _player.Animator.SetFloat(_AutoAttackMultiplierHash, _player.Statics.AutoAttackAnimationSpeed);

        Invoke(nameof(Attack1Complete), _player.AnimationsLength.AutoAttack1Duration / _player.Statics.AutoAttackAnimationSpeed);
        _dashed = false;
        Invoke(nameof(Dashed), _player.Statics.AutoAttackDashingTime);

        _player.Animator.CrossFade(_autoAttack1Hash, 0.2f);
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;

        _player.HitBoxes.Targets.Clear();
        _player.ActiveAttackCollider.Collider.enabled = true ;
    }

    public override void UpdateState()
    {
        if (_dashed || !_player.IsMovementPressed) return ;
        _player.Move(_player.Statics.AutoAttackDashingMovementSpeed);
        _player.Rotate(_player.Statics.AutoAttackRotationSpeed);
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
            Invoke(nameof(Attack2Complete), _player.AnimationsLength.AutoAttack2Duration / _player.Statics.AutoAttackAnimationSpeed);
            _dashed = false;
            Invoke(nameof(Dashed), _player.Statics.AutoAttackDashingTime);

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
            Invoke(nameof(Attack3Complete), _player.AnimationsLength.AutoAttack3Duration / _player.Statics.AutoAttackAnimationSpeed);
            _dashed = false;
            Invoke(nameof(Dashed), _player.Statics.AutoAttackDashingTime);

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
            Invoke(nameof(Attack1Complete), _player.AnimationsLength.AutoAttack1Duration / _player.Statics.AutoAttackAnimationSpeed);
            _dashed = false;
            Invoke(nameof(Dashed), _player.Statics.AutoAttackDashingTime);

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
        DoDamage(_player.Statics.AutoAttackDamage1);
    }
    void AutoAttack2Event()
    {
        DoDamage(_player.Statics.AutoAttackDamage2); 
    }
    void AutoAttack3Event()
    {
        DoDamage(_player.Statics.AutoAttackDamage3);
    }

    private void DoDamage(float damage)
    {
        foreach (EnemyBase enemy in _player.HitBoxes.Targets)
        {
            enemy.TakeDamage(damage);
        }
    }


}

