using UnityEngine;
using FishNet.Object;
public class EnemyBasicAttackState : BaseState
{    
    EnemyStateManger _enemy;
    EnemyStaticsScriptableObject _statics;

    private BasicAttackSpawner _spawner ;

    [SerializeField] private EnemyProjectile _projectilePrefab ;

    int _basicAttackHash;
    int _basicAttackMultiplierHash;

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
    }

    public override void EnterState()
    {
        _enemy.Animator.SetFloat(_basicAttackMultiplierHash, _statics.BasicAttackAnimationSpeed);

        if (_attackCounter < 2)
        {
            RpcSetBasicHitBox();
            _enemy.RpcHitBoxDisplay(true);

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

    [ObserversRpc]
    private void RpcSetBasicHitBox()
    {
        _enemy.ActiveHitBox = _enemy.HitBoxes.BasicHitBox;
    }

    public override void OnTickState()
    {
        base.OnTickState();
        Rotate();
    }

    private void Rotate()
    {
        if (!_doLookAt || _enemy.TargetPlayer == null ) return;

        _direction = (_enemy.TargetPlayer.transform.position - this.transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction);

        var rotationSpeed = (float)TimeManager.TickDelta * _statics.BasicAttackRotationSpeed;
        
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, rotationSpeed);
    }

    public override void UpdateState(){}

    public override void ExitState()
    {
        if (_enemy.TargetPlayer != null && !_enemy.TargetPlayer.IsAlive)
            _enemy.TargetPlayer = null;
    }

    [Server]
    void BasicAttackEvent()
    {
        _doLookAt = false;

        _enemy.RpcHitBoxDisplay(false);

        SpawningProjectile();

    }

    private void SpawningProjectile()
    {
        GameObject projectilePrefab = Instantiate(_projectilePrefab.gameObject);
        ServerManager.Spawn(projectilePrefab.gameObject);

        _projectile = projectilePrefab.GetComponent<EnemyProjectile>();

        _projectile.transform.position = _spawner.transform.position;
        _projectile.transform.rotation = _spawner.transform.rotation;
        _projectile.Damage = _statics.BasicAttackDamage;
        _projectile.Speed = _statics.BasicAttackProjectionSpeed;
        _projectile.Range = _statics.AttackRange;

        _projectile.StartCoroutine(_projectile.OnSpawned());
    }

    void AttackComplete()
    {
        _enemy.ReadyToSwitchState = true ;

        if (!_enemy.TargetPlayer.IsAlive)
        {
            _enemy.TargetPlayer = null;
            _enemy.SwitchState(_enemy.ChasingState);
        }
        
        else 
            _enemy.SwitchState(_enemy.IdleState);
    }

}
