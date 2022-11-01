using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyBasicAttackState : BaseState
{    
    EnemyStateManger _enemy;

    // Cashing Instances
    private BasicAttackSpawner _spawner ;

    //prefabs
    [SerializeField] private EnemyProjectile _projectilePrefab ;

    private ObjectPool<EnemyProjectile> _pool ;


    //Variables to store optimized Setter / getter parameter IDs
    int _basicAttackHash;
    int _basicAttackMultiplierHash;


    // utilities 
    private int _attackCounter ;
    private Vector3 _direction ;
    private Quaternion _lookRotation ;
    private EnemyProjectile _projectile ;

    // utilities 
    private bool _doLookAt ;


    private void Awake()
    {
        //Pooling
        _pool = new ObjectPool<EnemyProjectile>(

        () => {return Instantiate(_projectilePrefab);},

        _projectile => {_projectile.gameObject.SetActive(true);},

        _projectile => {_projectile.gameObject.SetActive(false);},

        _projectile => {Destroy(_projectile.gameObject);},

        false, 2,2 );
    }

    
    public void Start()
    {

        _enemy = GetComponent<EnemyStateManger>();
        _spawner = GetComponentInChildren<BasicAttackSpawner>();

        //caching Hashes
        _basicAttackHash = Animator.StringToHash("BasicAttack");
        _basicAttackMultiplierHash = Animator.StringToHash("BasicAttack__Multiplier");

        //Init things
        _attackCounter = 0 ;

    }
    public override void EnterState()
    {
        _enemy.Animator.SetFloat(_basicAttackMultiplierHash, _enemy.Statics.BasicAttackAnimationSpeed);

        if (_attackCounter < 2)
        {
            _enemy.HitBoxes.BasicHitBox.gameObject.SetActive(true);
            Invoke(nameof(AttackComplete), _enemy.AnimationsLength.BasicAttack_Duration / _enemy.Statics.BasicAttackAnimationSpeed );

            _enemy.NetworkAnimator.CrossFade(_basicAttackHash, 0.15f, 0);
            _enemy.ReadyToSwitchState = false ;

            _attackCounter++ ;
        }
        else if (_attackCounter == 2)
        {
            _enemy.SwitchState(_enemy.SuperAttackState);
            _attackCounter = 0 ;
        }

        _doLookAt = true;

    }

    public override void UpdateState()
    {
        if (!_doLookAt) return;
        _direction = (_enemy.Player.transform.position - this.transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * _enemy.Statics.BasicAttackRotationSpeed);
    }

    public override void ExitState()
    {
        
    }

    void AttackComplete()
    {
        _enemy.ReadyToSwitchState = true ;
        _enemy.SwitchState(_enemy.IdleState);
    }

    void BasicAttackEvent()
    {
        _doLookAt = false;        

        _enemy.HitBoxes.BasicHitBox.gameObject.SetActive(false);

        _projectile = _pool.Get();


        _projectile.transform.position = _spawner.transform.position; 
        _projectile.transform.rotation = _spawner.transform.rotation; 
        _projectile.Damage = _enemy.Statics.BasicAttackDamage ;
        _projectile.Speed = _enemy.Statics.BasicAttackProjectionSpeed ;
        _projectile.Range = _enemy.Statics.AttackRange ;

        _projectile.Init(CollidedAction);


        //_enemy.HitBoxes.BasicCollider.Collider.enabled = false ; // we don't have any 
    }

    private void CollidedAction(EnemyProjectile _projectile)
    {
        _pool.Release(_projectile);
    }

}
