using System;
using System.Collections.Generic;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerFirstAbilityState : BaseState, IHasCooldown
{
    PlayerStateManger _player;

    public string Id => _player.Statics.FirstAbilityAbilityName;
    public float CooldownDuration => _player.Statics.FirstAbilityCooldown ;
    private Dictionary<string, Image> imageDictionary = new();

    int _firstAbilityHash;
    int _firstAbilityMultiplierHash ;
    
    [SerializeField]
    private  VisualEffect _vfx;
    float _tickTimer ;
    float _tickPeriod ;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _player = GetComponent<PlayerStateManger>();

        _firstAbilityHash = Animator.StringToHash("FirstAbility");
        _firstAbilityMultiplierHash = Animator.StringToHash("FirstAbility_Multiplier");
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
            return;

        imageDictionary =_player.CooldownSystem.ImageDictionary;
        imageDictionary.Add(Id, _player.CooldownUIManager.CooldownUI1.Image);
        SceneManager.OnLoadEnd += OnLoadStage;
    }

    private void OnLoadStage(SceneLoadEndEventArgs obj)
    {
        if (imageDictionary.ContainsKey(Id))
            imageDictionary.Remove(Id);

        imageDictionary.Add(Id, _player.CooldownUIManager.CooldownUI1.Image);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        SceneManager.OnLoadEnd -= OnLoadStage;

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
        _player.NeedsRotate = false ;
        _player.SetMoveAndRotateSpeed(_player.Statics.FirstAbilityMovementSpeed, 0.0f);
        
    }

    public override void UpdateState(){}
    public override void OnTickState()
    {
        base.OnTickState();

        if (IsOwner)
            _player.ReadMovementInputAndSetMoveData();

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
                if (!enemy.IsAlive) continue;
                enemy.TakeDamage(_player.Statics.FirstAbilityDamage);
            }

            _tickTimer -= _tickPeriod;
        }
    }

    [Client]
    void FirstAbilityVFX()
    {
        _vfx.Play();
    }

    [Client]
    void FirstAbilityVFXStop()
    {
       // _vfx.Stop();
        _vfx.SendEvent("OnStop");
        //Debug.Log("Stop");
    }
    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
        _player.ActiveAttackCollider.Collider.enabled = false ;

        // ! this part is diff
        _player.NeedsMoveAndRotate = false;
        _player.NeedsRotate = true ;
        _player.IsAutoAiming = false ;
    }
}
