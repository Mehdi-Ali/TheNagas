using UnityEngine;
using UnityEngine.UI;

public class PlayerFirstAbilityState : BaseState, IHasCooldown
{
    //Utilities
    float _tickTimer ;
    float _tickPeriod ;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables to store optimized Setter / getter parameter IDs
    int _firstAbilityHash;
    int _firstAbilityMultiplierHash ;

    // cooldown things
    public string Id => _player.Statics.FirstAbilityAbilityName;
    public float CooldownDuration => _player.Statics.FirstAbilityCooldown ;
    public Image Image => _player.CooldownUIManager.CooldownUI1.Image;

    private void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _firstAbilityHash = Animator.StringToHash("FirstAbility");
        _firstAbilityMultiplierHash = Animator.StringToHash("FirstAbility_Multiplier");

    }

    public override void EnterState()
    {
        //check cooldown
        _player.Animator.SetFloat(_firstAbilityMultiplierHash, _player.Statics.FirstAbilityAnimationSpeed);
        _player.CooldownSystem.PutOnCooldown(this);
        
        Invoke(nameof(AttackComplete), _player.AnimationsLength.FirstAbilityDuration / _player.Statics.FirstAbilityAnimationSpeed);
        
        _player.NetworkAnimator.CrossFade(_firstAbilityHash, 0.1f, 0);
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
        FirstAbilityEvent();

        _tickPeriod = _player.AnimationsLength.FirstAbilityDuration / (float)_player.Statics.FirstAbilityTicks ;
        // so that first frame of the animation deals damage 
        _tickTimer = _tickPeriod ;
        
    }

    public override void UpdateState()
    {
        _player.SimpleMove(_player.Statics.FirstAbilityMovementSpeed);

        
        
        // a tick T period is the Animation length / the number of ticks 
        // each tick do this
        _tickTimer += Time.deltaTime ;
        if (_tickTimer >= -_tickPeriod)
        {
            foreach(EnemyBase enemy in _player.HitBoxes.Targets)
            {
                enemy.TakeDamage(_player.Statics.FirstAbilityDamage);
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
