using System;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerUltimateState : BaseState, IHasCooldown
{
    PlayerStateManger _player;

    public string Id => _player.Statics.UltimateAbilityAbilityName;
    public float CooldownDuration => _player.Statics.UltimateAbilityCooldown;

    int _ultimateHash;
    int _ultimateMultiplierHash;

    [SerializeField]
    private Material _dashingGlowMat ;
    private Material[] _originalMats ;

    [SerializeField]
    private SkinnedMeshRenderer[] _meshRenders;
    
    [SerializeField]
    private  VisualEffect _vfx;

    [SerializeField]
    private float _vFXLifeTime;
    private GameObject _spriteHolder;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _player = GetComponent<PlayerStateManger>();
        
        _ultimateHash = Animator.StringToHash("Ultimate");
        _ultimateMultiplierHash = Animator.StringToHash("Ultimate_Multiplier");

        if (!Owner.IsLocalClient) return ;
        _player.CooldownSystem.ImageDictionary.Add(Id,_player.CooldownUIManager.CooldownUIU.Image);


        // ! this part is diff

        _meshRenders = GetComponentsInChildren<SkinnedMeshRenderer>();
        _originalMats = new Material[_meshRenders.Length];

        _spriteHolder = SingletonSpriteManager.Instance.KnightUltCracks;
    }

    public override void EnterState()
    {
        var animSpeed = _player.Statics.UltimateAbilityAnimationSpeed ;
        var animDuration = _player.AnimationsLength.UltimateDuration;
        
        _player.CooldownSystem.PutOnCooldown(this);
        Invoke( nameof(AttackComplete),( animDuration / animSpeed ));
        _player.Animator.SetFloat(_ultimateMultiplierHash, animSpeed);
        
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
        _player.NeedsMoveAndRotate = true;
        
        if (IsServer)
        {
            _player.NetworkAnimator.CrossFade(_ultimateHash, 0.1f, 0);
            _player.HitBoxes.Targets.Clear();

            _player.HitBoxes.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _player.ActiveAttackCollider.Collider.enabled = true ;

        }

        // ! this part is diff dashing specific.

        var jumpTime = ( animDuration - ((41f - 28f) / 30f )) / animSpeed ;
        var start = transform.position ;
        var end = _player.TargetPosition ;

        var speed = (start - end).magnitude / jumpTime ;
        var directionVector = (end - start).normalized;

        _player.SetMoveAndRotateSpeed(speed, 0.0f);
        _player.SetMoveData(directionVector.x, directionVector.z);

        // ! this part is diff
        StartVFX();

    }

    private void StartVFX()
    {
        ChangingMat();
        _vfx.gameObject.SetActive(enabled);
        _vfx.Play();
    }

        private void ChangingMat()
    {
        int i = 0 ;
        foreach (SkinnedMeshRenderer meshRender in _meshRenders)
        {
            _originalMats[i] = meshRender.material;
            meshRender.material = _dashingGlowMat;
            i++;
        }
    }

    public override void UpdateState(){}
    public override void ExitState(){}

    void UltimateStartEvent()
    {
        _player.NeedsMoveAndRotate = false;
    }

    void UltimateEndEvent()
    {
        foreach(EnemyBase enemy in _player.HitBoxes.Targets)
        {
            if (!enemy.IsAlive) continue;
            enemy.TakeDamage(_player.Statics.UltimateAbilityDamage);
        }

        // ! this part is diff
        // TODO Separate VFX to Anticipation and VFX.
        RpcStartVFX();
        EndVFX();
    }

    private void EndVFX()
    {
        UnChangingMat();
        Invoke(nameof(DisableVfx), 0.4f);
    }

    private void UnChangingMat()
    {
        var i = 0;
        foreach (SkinnedMeshRenderer meshRender in _meshRenders)
        {
            meshRender.material = _originalMats[i];
            i++;
        }
    }

    private void DisableVfx()
    {
        _vfx.Stop();
        _vfx.gameObject.SetActive(false) ;
    }

    // TODO make VFX function only get called in observers
    [ObserversRpc]
    private void RpcStartVFX()
    {
        _spriteHolder.transform.position = this.transform.position;
        _spriteHolder.SetActive(true);
        Invoke(nameof(RpcStopVFX), _vFXLifeTime);
    }

    [ObserversRpc]
    private void RpcStopVFX()
    {
        _spriteHolder.SetActive(false);
    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
        _player.ActiveAttackCollider.Collider.enabled = false ; 
        _player.IsAutoAiming = false ;
    }

}
