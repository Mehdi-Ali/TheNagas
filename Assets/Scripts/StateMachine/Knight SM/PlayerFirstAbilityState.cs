using UnityEngine;

public class PlayerFirstAbilityState : BaseState, IHasCooldown
{
    //Name of The Abbility
    public string AbilityName = "Spin" ;

    //Game Designe Vars, Mak a stat Script maybe
    [SerializeField] float _movementSpeed = 7.5f;
    [SerializeField] float _cooldown = 5.0f ;
    [SerializeField] float _damage = 20.0f;
    [SerializeField] int _tick = 7;

    //Utilities
    float _tickTimer ;
    float _tickPeriod ;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables to store optimized Setter / getter parameter IDs
    int _firstAbility;

    // cooldown things
    public string Id => AbilityName ;
    public float CooldownDuration => _cooldown ;

    private void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _firstAbility = Animator.StringToHash("FirstAbility");

    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        //check cooldown
        _player.CooldownSystem.PutOnCooldown(this);
        
        Invoke(nameof(AttackComplete), _player.AnimationsLength.FirstAbilityDuration);
        
        _player.Animator.CrossFade(_firstAbility, 0.1f);
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
        FirstAbilityEvent();

        _tickPeriod = _player.AnimationsLength.FirstAbilityDuration / (float)_tick ;
        // so that first frame of the animation deals damage 
        _tickTimer = _tickPeriod ;
        
    }

    public override void UpdateState()
    {
        if (!base.IsOwner) return;
        _player.Move(_movementSpeed);

        
        
        // a tick T period is the Animation length / the number of ticks 
        // each tick do this
        _tickTimer += Time.deltaTime ;
        if (_tickTimer >= -_tickPeriod)
        {
            foreach(EnemyBase enemy in _player.HitBoxes.Targets)
            {
                enemy.TakeDamage(_damage);
            }

            _tickTimer -= _tickPeriod  ;

        }

    }

    public override void ExitState()
    {

    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
        _player.ActiveAttackCollider.Collider.enabled = false ;

    }

    void FirstAbilityEvent()
    {
        _player.HitBoxes.Targets.Clear();
        _player.ActiveAttackCollider.Collider.enabled = true ;       
    }
}
