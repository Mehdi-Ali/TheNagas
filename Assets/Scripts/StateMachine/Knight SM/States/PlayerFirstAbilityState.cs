using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFirstAbilityState : BaseState, IHasCooldown
{
    PlayerStateManger _player;

    public string Id => _player.Statics.FirstAbilityAbilityName;
    public float CooldownDuration => _player.Statics.FirstAbilityCooldown ;

    int _firstAbilityHash;
    int _firstAbilityMultiplierHash ;

    float _tickTimer ;
    float _tickPeriod ;


    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _player = GetComponent<PlayerStateManger>();

        _firstAbilityHash = Animator.StringToHash("FirstAbility");
        _firstAbilityMultiplierHash = Animator.StringToHash("FirstAbility_Multiplier");

        if (!Owner.IsLocalClient) return ;
        _player.CooldownSystem.ImageDictionary.Add(Id,_player.CooldownUIManager.CooldownUI1.Image);
    }

    public override void EnterState()
    {
        _player.CooldownSystem.PutOnCooldown(this);
        Invoke(nameof(AttackComplete), _player.AnimationsLength.FirstAbilityDuration / _player.Statics.FirstAbilityAnimationSpeed);
        _player.Animator.SetFloat(_firstAbilityMultiplierHash, _player.Statics.FirstAbilityAnimationSpeed);
        
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;

        if (IsServer)
        {
            _player.NetworkAnimator.CrossFade(_firstAbilityHash, 0.1f, 0);
            _player.HitBoxes.Targets.Clear();

            _player.HitBoxes.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _player.ActiveAttackCollider.Collider.enabled = true ;

            // ! this part is diff
            _tickPeriod = _player.AnimationsLength.FirstAbilityDuration / (float)_player.Statics.FirstAbilityTicks ;
            // so that first frame of the animation deals damage 
            _tickTimer = _tickPeriod - 2*(float)base.TimeManager.TickDelta ;
        }

        // ! this part is diff
        _player.NeedsMoveAndRotate = true;
        _player.SetMoveAndRotateSpeed(_player.Statics.FirstAbilityMovementSpeed, 0f);
        
    }

    public override void UpdateState()
    {
        if (IsOwner)
            _player.ReadAndSetMovementInput();
    }
    public override void OnTickState()
    {
        base.OnTickState();

        if (IsServer)
            FirstAbilityStartEvent();
    }
    public override void ExitState(){}

    [Server]
    private void FirstAbilityStartEvent()
    {
        // a tick T period is the Animation length / the number of ticks 
        // each tick do this
        _tickTimer += (float)base.TimeManager.TickDelta;
        if (_tickTimer >= _tickPeriod)
        {
            foreach (EnemyBase enemy in _player.HitBoxes.Targets)
            {
                enemy.TakeDamage(_player.Statics.FirstAbilityDamage);
            }

            _tickTimer -= _tickPeriod;
        }
    }


    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
        _player.ActiveAttackCollider.Collider.enabled = false ;

        // ! this part is diff
        _player.NeedsMoveAndRotate = false;

    }
}
