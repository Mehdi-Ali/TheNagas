using UnityEngine;

public class PlayerAutoAttackState : BaseState
{

    PlayerStateManger _player;

    int _autoAttack1Hash;
    int _autoAttack2Hash;
    int _autoAttack3Hash;
    int _AutoAttackMultiplierHash;

    public bool Continue;
    bool _dashed ;
    private float _distance ;
    private Vector3 _offset ;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _player = GetComponent<PlayerStateManger>();

        _autoAttack1Hash = Animator.StringToHash("AutoAttack1");
        _autoAttack2Hash = Animator.StringToHash("AutoAttack2");
        _autoAttack3Hash = Animator.StringToHash("AutoAttack3");
        _AutoAttackMultiplierHash = Animator.StringToHash("AutoAttack_Multiplier");

    }

    public override void EnterState()
    {
        Invoke(nameof(Attack1Complete), _player.AnimationsLength.AutoAttack1Duration / _player.Statics.AutoAttackAnimationSpeed);
        _player.Animator.SetFloat(_AutoAttackMultiplierHash, _player.Statics.AutoAttackAnimationSpeed);

        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;

        if (IsServer)
        {
            _player.NetworkAnimator.CrossFade(_autoAttack1Hash, 0.2f, 0);
            _player.HitBoxes.Targets.Clear();

            _player.HitBoxes.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _player.ActiveAttackCollider.Collider.enabled = true;
        }

        _dashed = false;
        Invoke(nameof(Dashed), _player.Statics.AutoAttackDashingTime);

        _player.NeedsMoveAndRotate = true;
        _player.SetMoveAndRotateSpeed(_player.Statics.AutoAttackDashingMovementSpeed, 0f);

        //AutoAim();
        // checkpoint 

    }

    public override void UpdateState()
    {

    }
    public override void OnTickState()
    {
        base.OnTickState();

        if (_dashed) return ;

        //if (_player.IsMovementPressed && !_player.IsAutoAiming )
        if (_player.IsMovementPressed && IsOwner )
        {
            _player.ReadAndSetMovementInput();
        }

        // if ( _distance > _player.Statics.AutoAttackStopDistance )
        // {
        //     // dash
        //     _player.CharacterController.SimpleMove( _offset.normalized * _player.Statics.AutoAttackDashingMovementSpeed);
        //    //_player.RotatePlayer(_player.Statics.AutoAttackRotationSpeed, _offset);

        // }
    }

    public override void ExitState(){}

    private void AutoAim()
    {
        if (!_player.IsMovementPressed)
        {
            //_distance = _player.AutoAim();

            _player.RotatePlayerToHitBox(_player.ActiveHitBox.transform.position);

            if (_distance > _player.Statics.AutoAttackStopDistance)
            {
                _offset = _player.TargetPos.position - transform.position;
            }

        }
        else _distance = -1f;
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

            _player.NetworkAnimator.CrossFade(_autoAttack2Hash, 0.0f, 0);

            //AutoAim();
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

            _player.NetworkAnimator.CrossFade(_autoAttack3Hash, 0.0f, 0);

            //AutoAim();

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

            _player.NetworkAnimator.CrossFade(_autoAttack1Hash, 0.2f, 0);

            //AutoAim();

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
         
        _player.NeedsMoveAndRotate = false; 
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

