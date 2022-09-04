using UnityEngine;

public class PlayerSecondAbilityState : PlayerBaseState, IHasCooldown
{
    //Name of The Ability
    public string AbilityName = "Slash";

    //Game Design Vars, Mak a stat Script maybe
    [SerializeField] private float _animationSpeed = 2.0f;
    [SerializeField] float _cooldown = 5.0f;
    [SerializeField] float _attackRang = 1.5f;
    [SerializeField] float _damage = 20.0f;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables to store optimized Setter / getter parameter IDs
    int _secondAbilityHash;
    int _secondAbilityMultiplierHash ;

    // cooldown things
    public string Id => AbilityName;
    public float CooldownDuration => _cooldown;

    private void Awake()
    {       
         //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _secondAbilityHash = Animator.StringToHash("SecondAbility");
        _secondAbilityMultiplierHash = Animator.StringToHash("SecondAbility_Multiplier");
       
       _player.Animator.SetFloat(_secondAbilityMultiplierHash, _animationSpeed);
    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        //check cooldown
        _player.CooldownSystem.PutOnCooldown(this);

        Invoke(nameof(AttackComplete), _player.AnimationsLength.SecondAbilityDuration / _animationSpeed);
                
        _player.Animator.CrossFade(_secondAbilityHash, 0.1f);
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        //enable SwitchState


        // should go to the method that deals dmg
    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false ;
        _player.SwitchState(_player.IdleState);
    }

    void SecondAbilityEvent()
    {

        // TODO make the radius directly related to the hit box, or make custom colliders.
        Collider[] _hitEnemies =  Physics.OverlapSphere(_player.ActiveHitBox.transform.position, _attackRang);
        
        foreach (Collider enemy in _hitEnemies)
        {

            if (enemy.TryGetComponent<EnemyBase>(out EnemyBase damageableEnemy))
            {
                damageableEnemy.TakeDamage(_damage);
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (_player.ActiveHitBox == null) return;
        Gizmos.DrawSphere(_player.ActiveHitBox.transform.position, _attackRang);
    }

}
