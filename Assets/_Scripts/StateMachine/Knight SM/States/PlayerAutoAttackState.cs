using FishNet.Object;
using UnityEngine;
using UnityEngine.VFX;

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

    [SerializeField]
    private  VisualEffect _vfx1;
    [SerializeField]
    private  VisualEffect _vfx2;
    [SerializeField]
    private  VisualEffect _vfx3;

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

        if (!_player.IsAimingPressed)
            AutoAim();

    }

    public override void UpdateState(){}
    public override void OnTickState()
    {
        base.OnTickState();

        if (!_player.NeedsMoveAndRotate || !IsOwner) return ;

        if (_player.IsMovementPressed )
        {
            _player.ReadMovementInputAndSetMoveData();
        }

        else if (_distance < 0f )
        {
            RpcSetNeedsMoveAndRotateStatus(false);
            return ;
        }

        else if (_distance < _statics.AutoAttackStopDistance)
        {
            if (_player.AutoTargetTransform == null) return;
            var pos = _player.AutoTargetTransform.transform.position ;
            RpcRotatePlayerToHitBox(pos);
            RpcSetNeedsMoveAndRotateStatus(false);
        }
    }

    [ServerRpc(RunLocally=true)]
    private void RpcSetNeedsMoveAndRotateStatus(bool status)
    {
        _player.NeedsMoveAndRotate = status;
    }

    [ServerRpc(RunLocally=true)]
    private void RpcRotatePlayerToHitBox(Vector3 position)
    {
        _player.RotatePlayerToHitBox(position);
    }

    //[Client(RequireOwnership = true)]
    private void AutoAim() //if doesn't work we move to OnTick
    {
        if (!_player.IsMovementPressed)
        {
            _distance = _player.AutoAim();


            if (_distance > _player.Statics.AutoAttackStopDistance)
            {
                _offset = _player.AutoTargetTransform.position - transform.position;
            }
            _player.SetMoveData(_offset.x, _offset.z);

        }

        else _distance = -1f;
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

            if (!_player.IsAimingPressed)
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

            if (!_player.IsAimingPressed)
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

            if (!_player.IsAimingPressed)
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

    [Client]
    void AutoAttack1VFX()
    {
        _vfx1.Play();
    }
    [Client]
    void AutoAttack2VFX()
    {
        _vfx2.Play();
    }
    [Client]
    void AutoAttack3VFX()
    {
        _vfx3.Play();
    }
}

