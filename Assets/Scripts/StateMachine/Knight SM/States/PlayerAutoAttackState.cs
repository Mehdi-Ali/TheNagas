using FishNet.Object;
using UnityEngine;

public class PlayerAutoAttackState : BaseState
{

    PlayerStateManger _player;
    public PlayerStaticsScriptableObject _statics ;


    int _autoAttack1Hash;
    int _autoAttack2Hash;
    int _autoAttack3Hash;
    int _AutoAttackMultiplierHash;

    public bool Continue;
    private float _distance ;
    private Vector3 _offset ;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _player = GetComponent<PlayerStateManger>();
        _statics = _player.Statics;

        _autoAttack1Hash = Animator.StringToHash("AutoAttack1");
        _autoAttack2Hash = Animator.StringToHash("AutoAttack2");
        _autoAttack3Hash = Animator.StringToHash("AutoAttack3");
        _AutoAttackMultiplierHash = Animator.StringToHash("AutoAttack_Multiplier");

    }

    public override void EnterState()
    {
        Invoke(nameof(Attack1Complete), _player.AnimationsLength.AutoAttack1Duration / _statics.AutoAttackAnimationSpeed);
        _player.Animator.SetFloat(_AutoAttackMultiplierHash, _statics.AutoAttackAnimationSpeed);

        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;

        if (IsServer)
        {
            _player.NetworkAnimator.CrossFade(_autoAttack1Hash, 0.2f, 0);
            _player.HitBoxes.Targets.Clear();

            _player.HitBoxes.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _player.ActiveAttackCollider.Collider.enabled = true;
        }

        _player.NeedsMoveAndRotate = true;
        Invoke(nameof(Dashed), _statics.AutoAttackDashingTime);
        _player.SetMoveAndRotateSpeed(_statics.AutoAttackDashingMovementSpeed, _statics.AutoAttackRotationSpeed);

        if (!IsClient) return;

        if (!_player.IsMovementPressed)
            AutoAim();

    }

    public override void UpdateState(){}
    public override void OnTickState()
    {
        base.OnTickState();

        if (!_player.NeedsMoveAndRotate) return ;

        if (_player.IsMovementPressed && IsOwner )
        {
            _player.ReadMovementInputAndSetMoveData();
        }

        if ( _distance < _statics.AutoAttackStopDistance ) return;
            //_player.NeedsMoveAndRotate = false;
    }

    //[Client(RequireOwnership = true)]
    private void AutoAim() //if doesn't work we move to OnTick
    {
        if (!_player.IsMovementPressed)
        {
            _distance = _player.AutoAim();

            RpcRotatePlayerToHitBox(_player.ActiveHitBox.transform.position);

            if (_distance > _player.Statics.AutoAttackStopDistance)
            {
                _offset = _player.AutoTargetTransform.position - transform.position;
            }
            _player.SetMoveData(_offset.x, _offset.z);

        }

        else _distance = -1f;
    }

    [ServerRpc(RunLocally=true)]
    private void RpcRotatePlayerToHitBox(Vector3 position)
    {
        _player.RotatePlayerToHitBox(position);
    }

    void Dashed()
    {
        _player.NeedsMoveAndRotate = false;
    }

    public void Attack1Complete()
    {
        if (Continue)
        {
            Invoke(nameof(Attack2Complete), _player.AnimationsLength.AutoAttack2Duration / _statics.AutoAttackAnimationSpeed);
            _player.NeedsMoveAndRotate = true;
            Invoke(nameof(Dashed), _statics.AutoAttackDashingTime);

            if (IsServer)
                _player.NetworkAnimator.CrossFade(_autoAttack2Hash, 0.0f, 0);

            if (!_player.IsMovementPressed)
                AutoAim();
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
            Invoke(nameof(Attack3Complete), _player.AnimationsLength.AutoAttack3Duration / _statics.AutoAttackAnimationSpeed);
            _player.NeedsMoveAndRotate = true;
            Invoke(nameof(Dashed), _statics.AutoAttackDashingTime);

            if (IsServer)
                _player.NetworkAnimator.CrossFade(_autoAttack3Hash, 0.0f, 0);

            if (!_player.IsMovementPressed)
                AutoAim();
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
            Invoke(nameof(Attack1Complete), _player.AnimationsLength.AutoAttack1Duration / _statics.AutoAttackAnimationSpeed);
            _player.NeedsMoveAndRotate = true;
            Invoke(nameof(Dashed), _statics.AutoAttackDashingTime);

            if (IsServer)
                _player.NetworkAnimator.CrossFade(_autoAttack1Hash, 0.2f, 0);

            if (!_player.IsMovementPressed)
                AutoAim();

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
        _player.IsAutoAiming = false ;
    }

    public override void ExitState(){}

    [Server]
    void AutoAttack1Event()
    {
        DoDamage(_statics.AutoAttackDamage1);
    }

    [Server]
    void AutoAttack2Event()
    {
        DoDamage(_statics.AutoAttackDamage2); 
    }

    [Server]
    void AutoAttack3Event()
    {
        DoDamage(_statics.AutoAttackDamage3);
    }

    [Server]
    private void DoDamage(float damage)
    {
        foreach (EnemyBase enemy in _player.HitBoxes.Targets)
        {
            if (!enemy.IsAlive) continue;
            enemy.TakeDamage(damage);
        }
    }


}

