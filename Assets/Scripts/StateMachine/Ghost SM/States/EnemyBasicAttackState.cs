using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using FishNet.Object;
using FishNet.Connection;

public class EnemyBasicAttackState : BaseState
{    
    EnemyStateManger _enemy;
    EnemyStaticsScriptableObject _statics;

    private BasicAttackSpawner _spawner ;

    [SerializeField] private EnemyProjectile _projectilePrefab ;

    int _basicAttackHash;
    int _basicAttackMultiplierHash;

    private ObjectPool<EnemyProjectile> _pool ;
    private int _attackCounter ;
    private Vector3 _direction ;
    private Quaternion _lookRotation ;
    private EnemyProjectile _projectile ;
    private bool _doLookAt ;


    private void Awake()
    {
        _enemy = GetComponent<EnemyStateManger>();
        _spawner = GetComponentInChildren<BasicAttackSpawner>();
        _statics = _enemy.Statics;

        _basicAttackHash = Animator.StringToHash("BasicAttack");
        _basicAttackMultiplierHash = Animator.StringToHash("BasicAttack__Multiplier");

        _attackCounter = 0 ;

        PoolingSetUp();
    }

    private void PoolingSetUp()
    {
        _pool = new ObjectPool<EnemyProjectile>(

        () => { return Instantiate(_projectilePrefab); },

        _projectile => { _projectile.gameObject.SetActive(true); },

        _projectile => { _projectile.gameObject.SetActive(false); },

        _projectile => { Destroy(_projectile.gameObject); },

        false, 2, 2);
    }

    public override void EnterState()
    {
        _enemy.Animator.SetFloat(_basicAttackMultiplierHash, _statics.BasicAttackAnimationSpeed);

        if (_attackCounter < 2)
        {
            RpcHitBoxDisplay(Owner, true);
            Invoke(nameof(AttackComplete), _enemy.AnimationsLength.BasicAttack_Duration / _statics.BasicAttackAnimationSpeed);

            _enemy.NetworkAnimator.CrossFade(_basicAttackHash, 0.15f, 0);
            _enemy.ReadyToSwitchState = false;

            _attackCounter++;
        }
        else if (_attackCounter == 2)
        {
            _enemy.SwitchState(_enemy.SuperAttackState);
            _attackCounter = 0 ;
        }

        _doLookAt = true;
    }

    [TargetRpc]
    private void RpcHitBoxDisplay(NetworkConnection conn, bool status)
    {
        _enemy.HitBoxes.BasicHitBox.gameObject.SetActive(status);
    }

    public override void OnTickState()
    {
        base.OnTickState();
        Rotate();
    }

    private void Rotate()
    {
        if (!_doLookAt) return;

        _direction = (_enemy.TargetPlayer.transform.position - this.transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * _statics.BasicAttackRotationSpeed);
    }

    public override void UpdateState(){}

    public override void ExitState(){}

    void AttackComplete()
    {
        _enemy.ReadyToSwitchState = true ;
        _enemy.SwitchState(_enemy.IdleState);
    }

    void BasicAttackEvent()
    {
        _doLookAt = false;        

        RpcHitBoxDisplay(Owner, false);

        _projectile = _pool.Get();
        
        _projectile.transform.position = _spawner.transform.position; 
        _projectile.transform.rotation = _spawner.transform.rotation; 
        _projectile.Damage = _statics.BasicAttackDamage ;
        _projectile.Speed = _statics.BasicAttackProjectionSpeed ;
        _projectile.Range = _statics.AttackRange ;

        _projectile.Init(CollidedAction);

    }

    private void CollidedAction(EnemyProjectile _projectile)
    {
        _pool.Release(_projectile);
    }

}
